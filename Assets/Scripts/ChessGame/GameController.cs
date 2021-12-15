using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] Camera currentCamera;

    [Header("BoardSetup")]
    [SerializeField] ChessBoard chessBoard;

    [Header("ChessPieces Setup")]
    [SerializeField] GameObject[] blackMainChessPieces;
    [SerializeField] GameObject[] blackPawns;
    [SerializeField] GameObject[] whiteMainChessPieces;
    [SerializeField] GameObject[] whitePawns;

    int X_Size = 0;
    int Y_Size = 0;
    private Vector2Int currentHover;

    private ChessPiece[,] chessPiecesMap;
    private List<ChessPiece> deadWhitePieces = new List<ChessPiece>();
    private List<ChessPiece> deadBlackPieces = new List<ChessPiece>();

    private ChessPiece currentlyDraggingChessPiece;

    #region UnityBuiltinFunctions
    private void Awake()
    {
        X_Size = chessBoard.TILE_COUNT_X;
        Y_Size = chessBoard.TILE_COUNT_Y;
        mapChessPieces();

    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        Ray ray = new Ray();
        switch (SystemInfo.deviceType)
        {
            case DeviceType.Unknown:
                break;
            case DeviceType.Handheld:
                ray = this.currentCamera.ScreenPointToRay(Input.GetTouch(0).position);
                break;
            case DeviceType.Console:
                break;
            case DeviceType.Desktop:
                ray = this.currentCamera.ScreenPointToRay(Input.mousePosition);
                break;
            default:
                break;
        }

        this.handleInputsTransitions(ray);

        if (currentlyDraggingChessPiece)
        {
            this.dragChessPiece(ray);
        }
    }
    #endregion


    /** Maps the chessPieces to the grid
     * 
     */
    private void mapChessPieces()
    {
        this.chessPiecesMap = new ChessPiece[X_Size, Y_Size];
        for (int x = 0; x < X_Size; x++)
        {
            this.chessPiecesMap[x, 0] = this.blackMainChessPieces[x].GetComponent<ChessPiece>();
        }
        for (int x = 0; x < X_Size; x++)
        {
            this.chessPiecesMap[x, 1] = this.blackPawns[x].GetComponent<ChessPiece>();
        }

        for (int x = 0; x < X_Size; x++)
        {
            int endIndex = (X_Size - 1);
            this.chessPiecesMap[endIndex - x, Y_Size - 1] = this.whiteMainChessPieces[x].GetComponent<ChessPiece>();
        }
        for (int x = 0; x < X_Size; x++)
        {
            int endIndex = (X_Size - 1);
            this.chessPiecesMap[endIndex - x, Y_Size - 2] = this.whitePawns[x].GetComponent<ChessPiece>();
        }
    }

    private void handleInputsTransitions(Ray ray)
    {
        RaycastHit info;

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")))
        {
            this.whithinBoardHandler(info);
        }
        else
        {
            this.outsideBoardHandler();
        }
    }

    /** Handles logic if mouse withing the boards bounds
     * 
     */
    private void whithinBoardHandler(RaycastHit info)
    {
        Vector2Int hitPos = chessBoard.lookupTileIndex(info);

        //Transition from not hovering to hover
        if (this.currentHover == -Vector2Int.one)
        {
            this.currentHover = hitPos;
            this.chessBoard.tiles[this.currentHover.x, this.currentHover.y].GetComponent<Renderer>().sharedMaterial = chessBoard.hoverMaterial;
        }
        //Transition from Hovering one tile to the next one
        if (this.currentHover != hitPos)
        {
            //On Leave
            this.chessBoard.tiles[this.currentHover.x, this.currentHover.y].GetComponent<Renderer>().sharedMaterial = this.chessBoard.tileMaterial;
            this.currentHover = hitPos;
            //OnEnter
            this.chessBoard.tiles[this.currentHover.x, this.currentHover.y].GetComponent<Renderer>().sharedMaterial = this.chessBoard.hoverMaterial;
        }

        //Selectiong ChessPiece
        if (Input.GetMouseButtonDown(0))
        {
            if (this.chessPiecesMap[hitPos.x, hitPos.y] != null)
            {
                //Check Turn
                bool isMyTurn = true;
                if (isMyTurn)
                {
                    this.currentlyDraggingChessPiece = this.chessPiecesMap[hitPos.x, hitPos.y];
                }
            }
        }
        //Letting Go of the ChessPiece
        if (this.currentlyDraggingChessPiece != null && Input.GetMouseButtonUp(0))
        {
            Vector3 previousPosition = this.floorToIntVector3(this.currentlyDraggingChessPiece.currentPosition);
            bool validMove = this.moveTo(this.currentlyDraggingChessPiece, hitPos, previousPosition);

            if (!validMove)
            {
                this.moveChessPieceBack();
            }
            else
            {
                this.currentlyDraggingChessPiece = null;
            }
        }
    }

    /** Handles Logic if mouse is outside the boards bounds
     * 
     */
    private void outsideBoardHandler()
    {
        if (this.currentHover != -Vector2Int.one)
        {
            //On Leave
            this.chessBoard.tiles[this.currentHover.x, this.currentHover.y].GetComponent<Renderer>().sharedMaterial = this.chessBoard.tileMaterial;
            this.currentHover = -Vector2Int.one;
        }

        if (this.currentlyDraggingChessPiece && Input.GetMouseButtonUp(0))
        {
            this.moveChessPieceBack();
        }
    }

    /** Drags selected chessPiece along an xz-plane
     * 
     */ 
    private void dragChessPiece(Ray ray)
    {
        Plane plane = new Plane(Vector3.up, Vector3.up * chessBoard.yOffset);
        float dist = 0f;
        if (plane.Raycast(ray, out dist))
        {
            Vector3 intersection = ray.GetPoint(dist);
            intersection.x -= 0.5f;
            intersection.z -= 0.5f;
            intersection.y += 0.3f;
            currentlyDraggingChessPiece.SetPosition(intersection);
        }
    }

    /** Validates Movement of the ChessPiece
     * 
     */
    private bool moveTo(ChessPiece draggingChessPiece, Vector2Int hitPos, Vector3 prevPos)
    {
        //Another Piece is on hitPos
        if (this.chessPiecesMap[hitPos.x, hitPos.y] != null)
        {
            ChessPiece otherChessPiece = this.chessPiecesMap[hitPos.x, hitPos.y]; 
            if(otherChessPiece.team == draggingChessPiece.team)
            {
                return false;
            }
            else
            {
                this.killOponent(otherChessPiece);
            }

        }

        chessPiecesMap[hitPos.x, hitPos.y] = draggingChessPiece;
        chessPiecesMap[(int)prevPos.x, (int)prevPos.z] = null;
        this.moveChessPiece(hitPos.x, hitPos.y);
        return true;
    }

    /** Sends the OtherChessPiece to the graveyard
     * 
     */ 
    private void killOponent(ChessPiece otherChessPiece)
    {
        float yDeathOffset = 0.25709f;
        //If Enemy Is Being Killed
        if (otherChessPiece.team == Team.BLACK)
        {
            this.deadBlackPieces.Add(otherChessPiece);
            otherChessPiece.SetScale(0.5f);

            Vector3 initDeathPosition = new Vector3(-0.559f, yDeathOffset, -1.0f);
            Vector3 removeCenteredOffset = new Vector3(-0.5f, 0, -0.5f);
            Vector3 deathPosAlignment = (Vector3.forward * 0.5f) * deadBlackPieces.Count;

            Vector3 deathPos = initDeathPosition + removeCenteredOffset + deathPosAlignment;

            otherChessPiece.SetPosition(deathPos);

        }
        else if (otherChessPiece.team == Team.WHITE)
        {
            this.deadWhitePieces.Add(otherChessPiece);
            otherChessPiece.SetScale(0.5f);

            Vector3 initDeathPosition = new Vector3(8.562f, yDeathOffset, 8f);
            Vector3 removeCenteredOffset = new Vector3(-0.5f, 0, 0.5f);
            Vector3 deathPosAlignment = (Vector3.back * 0.5f) * deadWhitePieces.Count;

            Vector3 deathPos = initDeathPosition + removeCenteredOffset + deathPosAlignment;

            otherChessPiece.SetPosition(deathPos);
        }
    }

    /** Moves the chess piece back to its previous position
     * 
     */ 
    private void moveChessPieceBack()
    {
        Vector3 previousPosition = this.floorToIntVector3(currentlyDraggingChessPiece.currentPosition);
        currentlyDraggingChessPiece.SetPosition(previousPosition);
        currentlyDraggingChessPiece = null;
    }

    /** Moves the Chess Piece
     * 
     */ 
    private void moveChessPiece(int x, int y)
    {
        Vector3 newPos = new Vector3(x, 0, y);
        this.chessPiecesMap[x, y].currentPosition = newPos;
        this.chessPiecesMap[x, y].SetPosition(newPos);
    }

    /** Returns floored xy Coordinates for chess Pieces
     * 
     */ 
    Vector3 floorToIntVector3(Vector3 vec3)
    {
        int x = Mathf.FloorToInt(vec3.x);
        int y = Mathf.FloorToInt(vec3.z);
        return new Vector3(x, 0, y);
    }
}