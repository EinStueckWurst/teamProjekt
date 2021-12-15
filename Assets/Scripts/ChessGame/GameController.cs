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
    private List<Vector2Int> possibleMoves;

    private bool isWhiteTurn = true;

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

        if (Physics.Raycast(ray, out info, 100))
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

        //Transition from not hovering at all --> to hovering one Tile
        if (this.currentHover == -Vector2Int.one)
        {
            this.currentHover = hitPos;

            this.onEnterTile(this.currentHover.x, this.currentHover.y);

        }
        //Transition from Hovering one tile --> to the next tile
        if (this.currentHover != hitPos)
        {
            //On Leave A Tile
            this.onLeaveTile(this.currentHover.x, this.currentHover.y);
            this.currentHover = hitPos;
            //OnEnter A Tile
            this.onEnterTile(this.currentHover.x, this.currentHover.y);
        }

        //Selectiong ChessPiece
        if (Input.GetMouseButtonDown(0))
        {
            if (this.chessPiecesMap[hitPos.x, hitPos.y] != null)
            {
                if (this.chessPiecesMap[hitPos.x, hitPos.y].team == Team.WHITE && this.isWhiteTurn)
                {
                    this.currentlyDraggingChessPiece = this.chessPiecesMap[hitPos.x, hitPos.y];
                    this.possibleMoves = this.currentlyDraggingChessPiece.GetPossibleMoves(ref this.chessPiecesMap, X_Size, Y_Size);
                    this.highlightTiles();
                }
                
                if (this.chessPiecesMap[hitPos.x, hitPos.y].team == Team.BLACK && !this.isWhiteTurn)
                {
                    this.currentlyDraggingChessPiece = this.chessPiecesMap[hitPos.x, hitPos.y];
                    this.possibleMoves = this.currentlyDraggingChessPiece.GetPossibleMoves(ref this.chessPiecesMap, X_Size, Y_Size);
                    this.highlightTiles();
                
                }
            }
        }
        //Letting Go of the ChessPiece
        if (this.currentlyDraggingChessPiece != null && Input.GetMouseButtonUp(0))
        {
            Vector3 previousPosition =ChessGameUtil.floorToIntVector3(this.currentlyDraggingChessPiece.currentPosition);
            bool validMove = this.moveTo(this.currentlyDraggingChessPiece, hitPos, previousPosition);
            this.removeHighlightTiles();
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

    /** Triggers on entering a tile at x,y position
     * 
     */ 
    private void onEnterTile(int x, int y)
    {
        this.chessBoard.tiles[x,y].GetComponent<Renderer>().sharedMaterial = this.chessBoard.hoverMaterial;
    }

    /** Triggers on leaving a tile at x,y position
     * 
     */
    private void onLeaveTile(int x, int y)
    {
        bool isAPossibleMoveTile = this.validateMove(this.possibleMoves, new Vector2Int(x, y));

        if(isAPossibleMoveTile)
        {
            this.chessBoard.tiles[x, y].GetComponent<Renderer>().sharedMaterial = chessBoard.possibleMoveMaterial;
        }
        else
        {
            this.chessBoard.tiles[x, y].GetComponent<Renderer>().sharedMaterial = this.chessBoard.tileMaterial;
        }
    }

    /** Highlights the possible moves
     * 
     */ 
    private void highlightTiles()
    {
        for (int i = 0; i < this.possibleMoves.Count; i++)
        {
            int x = this.possibleMoves[i].x;
            int y = this.possibleMoves[i].y;
            this.chessBoard.tiles[x,y].GetComponent<Renderer>().sharedMaterial = chessBoard.possibleMoveMaterial;
        }
    }

    /** Unhighlights the possible moves
     * 
     */
    private void removeHighlightTiles()
    {
        for (int i = 0; i < this.possibleMoves.Count; i++)
        {
            int x = this.possibleMoves[i].x;
            int y = this.possibleMoves[i].y;
            this.chessBoard.tiles[x,y].GetComponent<Renderer>().sharedMaterial = chessBoard.tileMaterial;
        }
        this.possibleMoves.Clear();
    }

    /** Handles Logic if mouse is outside the boards bounds
     * 
     */
    private void outsideBoardHandler()
    {
        if (this.currentHover != -Vector2Int.one)
        {
            this.onLeaveTile(this.currentHover.x, this.currentHover.y);
            this.currentHover = -Vector2Int.one;
        }

        if (this.currentlyDraggingChessPiece && Input.GetMouseButtonUp(0))
        {
            this.moveChessPieceBack();
            this.removeHighlightTiles();
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
        //If move not valid --> abort
        if (!this.validateMove(this.possibleMoves, hitPos))
        {
            return false;
        }

        //selecting a tile with another chesspiece on it
        if (this.chessPiecesMap[hitPos.x, hitPos.y] != null)
        {
            ChessPiece otherChessPiece = this.chessPiecesMap[hitPos.x, hitPos.y];

            if (otherChessPiece.team == draggingChessPiece.team)
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

        this.isWhiteTurn = !this.isWhiteTurn;
        return true;
    }

    /** Checks wether the newly selcted position (move) is a valid move
     * 
     */ 
    private bool validateMove(List<Vector2Int> validMoves, Vector2Int newPos)
    {
        if(newPos != null && validMoves != null)
        {
            for (int i = 0; i < validMoves.Count; i++)
            {
                if(validMoves[i].x == newPos.x && validMoves[i].y == newPos.y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /** Sends the OtherChessPiece to the graveyard
     * 
     */ 
    private void killOponent(ChessPiece otherChessPiece)
    {
        float yDeathOffset = 0.25709f;

        if(otherChessPiece.type == ChessPieceType.KING)
        {
            this.checkMate(otherChessPiece.team);
        }

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

    private void checkMate(Team team)
    {
        Debug.Log("CheckMate for " + team.ToString());
    }

    /** Moves the chess piece back to its previous position
     * 
     */
    private void moveChessPieceBack()
    {
        Vector3 previousPosition = ChessGameUtil.floorToIntVector3(currentlyDraggingChessPiece.currentPosition);
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
}