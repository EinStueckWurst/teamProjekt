using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum SpecialMove
{
    NONE,
    CASTLING,
    PROMOTION,
}

public class GameController : MonoBehaviour
{
    [SerializeField] Camera currentCamera;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] TextMeshProUGUI winningText;
    
    [Header("BoardSetup")]
    [SerializeField] ChessBoard chessBoard;

    [Header("ChessPieces Setup")]
    [SerializeField] GameObject spawnBlackQueenForPawnPromotion;
    [SerializeField] GameObject spawnWhiteQueenForPawnPromotion;

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
    private List<ChessPiece> spawnedQueens = new List<ChessPiece>();

    private ChessPiece currentlyDraggingChessPiece;
    private List<Vector2Int> possibleMoves;

    private bool isWhiteTurn = true;
    private SpecialMove specialMove; 
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();

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
                    this.specialMove = currentlyDraggingChessPiece.GetSpecialMoves(ref this.chessPiecesMap, ref this.moveList, ref this.possibleMoves);
                    this.highlightTiles();
                }

                if (this.chessPiecesMap[hitPos.x, hitPos.y].team == Team.BLACK && !this.isWhiteTurn)
                {
                    this.currentlyDraggingChessPiece = this.chessPiecesMap[hitPos.x, hitPos.y];
                    this.possibleMoves = this.currentlyDraggingChessPiece.GetPossibleMoves(ref this.chessPiecesMap, X_Size, Y_Size);
                    this.specialMove = currentlyDraggingChessPiece.GetSpecialMoves(ref this.chessPiecesMap, ref this.moveList, ref this.possibleMoves);
                    this.highlightTiles();
                }

                //this.currentlyDraggingChessPiece = this.chessPiecesMap[hitPos.x, hitPos.y];
                //this.possibleMoves = this.currentlyDraggingChessPiece.GetPossibleMoves(ref this.chessPiecesMap, X_Size, Y_Size);
                //this.specialMove = currentlyDraggingChessPiece.GetSpecialMoves(ref this.chessPiecesMap, ref this.moveList, ref this.possibleMoves);
                //this.highlightTiles();
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
        Vector2Int prevPosVec2 = ChessGameUtil.floorToIntVector2Int(prevPos);

        //Tracking every move 
        this.moveList.Add(new Vector2Int[] { prevPosVec2, new Vector2Int(hitPos.x, hitPos.y) });

        this.handleSpecialMove();

        return true;
    }

    private void handleSpecialMove()
    {
        switch (this.specialMove)
        {
            case SpecialMove.NONE:
                break;
            case SpecialMove.CASTLING:
                this.handleCastlingMove();
                break;
            case SpecialMove.PROMOTION:
                this.handlePromotionMove();
                break;
            default:
                break;
        }
    }

    private void handlePromotionMove()
    {
        Vector2Int[] lastMove = this.moveList[this.moveList.Count - 1];
        ChessPiece pawn = this.chessPiecesMap[lastMove[1].x, lastMove[1].y];

        if(pawn.type == ChessPieceType.PAWN)
        {
            if(pawn.team == Team.BLACK && lastMove[1].y == 7)
            {
                GameObject blackQueenObj= Instantiate(spawnBlackQueenForPawnPromotion, this.spawnBlackQueenForPawnPromotion.transform.parent.transform);
                ChessPiece newBlackQueen = blackQueenObj.GetComponent<ChessPiece>();
                this.spawnedQueens.Add(newBlackQueen);
                this.killOponent(this.chessPiecesMap[lastMove[1].x, lastMove[1].y]);
                this.chessPiecesMap[lastMove[1].x, lastMove[1].y] = newBlackQueen;
                this.moveChessPiece(lastMove[1].x, lastMove[1].y);
            }
            
            if(pawn.team == Team.WHITE && lastMove[1].y == 0)
            {
                GameObject blackQueenObj= Instantiate(spawnWhiteQueenForPawnPromotion, this.spawnWhiteQueenForPawnPromotion.transform.parent.transform);
                ChessPiece newWhiteQueen = blackQueenObj.GetComponent<ChessPiece>();
                this.spawnedQueens.Add(newWhiteQueen);
                this.killOponent(this.chessPiecesMap[lastMove[1].x, lastMove[1].y]);
                this.chessPiecesMap[lastMove[1].x, lastMove[1].y] = newWhiteQueen;
                this.moveChessPiece(lastMove[1].x, lastMove[1].y);
            }
        }
    }

    private void handleCastlingMove()
    {
        Vector2Int[] lastMove = this.moveList[moveList.Count - 1];

        if(lastMove[1].x == 1)
        {
            if(lastMove[1].y == 0)
            {
                ChessPiece rook = this.chessPiecesMap[0, 0];
                this.chessPiecesMap[2, 0] = rook;
                this.moveChessPiece(2, 0);
                this.chessPiecesMap[0, 0] = null;
            }
            if(lastMove[1].y == 7)
            {
                ChessPiece rook = this.chessPiecesMap[0, 7];
                this.chessPiecesMap[2, 7] = rook;
                this.moveChessPiece(2, 7);
                this.chessPiecesMap[0, 7] = null;
            }
        } 
        else if(lastMove[1].x == 5)
        {
            if (lastMove[1].y == 0)
            {
                ChessPiece rook = this.chessPiecesMap[7, 0];
                this.chessPiecesMap[4, 0] = rook;
                this.moveChessPiece(4, 0);
                this.chessPiecesMap[7, 0] = null;
            }
            if (lastMove[1].y == 7)
            {
                ChessPiece rook = this.chessPiecesMap[7, 7];
                this.chessPiecesMap[4, 7] = rook;
                this.moveChessPiece(4, 7);
                this.chessPiecesMap[7, 7] = null;
            }
        }
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
        this.displayVictory(team);
    }

    /** Displays the Victory UI
     * 
     */ 
    private void displayVictory(Team team)
    {
        this.winningText.SetText(team.ToString() + " WINS");
        this.victoryScreen.SetActive(true);
    }

    /** Resets the game
     * 
     */ 
    public void onResetButton()
    {
        this.victoryScreen.SetActive(false);

        this.currentlyDraggingChessPiece = null;
        this.possibleMoves.Clear();
        this.moveList.Clear();

        this.destroyAllSpawnedQueens();
        this.mapChessPieces();
        for (int x = 0; x < X_Size; x++)
        {
            for (int y = 0; y < Y_Size; y++)
            {
                if(this.chessPiecesMap[x,y] != null)
                {
                    Vector3 floored = ChessGameUtil.floorToIntVector3(this.chessPiecesMap[x, y].originalPosition);
                    this.chessPiecesMap[x, y].currentPosition = this.chessPiecesMap[x, y].originalPosition;
                    this.chessPiecesMap[x, y].SetPosition(floored);
                    this.chessPiecesMap[x, y].SetScale(1);
                }
            }
        }
        deadWhitePieces.Clear();
        deadBlackPieces.Clear();

        this.isWhiteTurn = true;
    }

    /** Destroys all queens that were used for promotion
     * 
     */ 
    private void destroyAllSpawnedQueens()
    {
        for (int i = 0; i < this.spawnedQueens.Count; i++)
        {
            if(spawnedQueens[i] != null)
            Destroy(spawnedQueens[i].gameObject);
        }
    }

    public void onQuitButton()
    {
        Application.Quit();
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