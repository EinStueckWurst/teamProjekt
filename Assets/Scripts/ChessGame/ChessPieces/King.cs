using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(ref ChessPiece[,] chessPieceMap, int tileCountX, int tileCountY)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        Vector2Int currentPos = ChessGameUtil.floorToIntVector2Int(this.currentPosition);

        possibleMoves.Add(currentPos);

        //1 forward
        if (currentPos.y != (tileCountY-1)
            && (chessPieceMap[currentPos.x, currentPos.y +1] == null || chessPieceMap[currentPos.x, currentPos.y +1].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x, currentPos.y +1));
        }

        //1 back
        if (currentPos.y != 0 
            && (chessPieceMap[currentPos.x, currentPos.y -1] == null || chessPieceMap[currentPos.x, currentPos.y -1].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x, currentPos.y -1));
        }

        //1 left
        if (currentPos.x != (tileCountX - 1)
            && (chessPieceMap[currentPos.x + 1, currentPos.y] == null || chessPieceMap[currentPos.x +1, currentPos.y].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x + 1, currentPos.y));
        }
        
        //1 right
        if (currentPos.x != 0
            && (chessPieceMap[currentPos.x - 1, currentPos.y] == null || chessPieceMap[currentPos.x - 1, currentPos.y].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
        }
        
        //1 left-front
        if (currentPos.y != (tileCountY - 1)
            && currentPos.x != (tileCountX - 1)
            && (chessPieceMap[currentPos.x +1, currentPos.y + 1] == null || chessPieceMap[currentPos.x + 1, currentPos.y + 1].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x + 1, currentPos.y + 1));
        }
        
        //1 right-front
        if (currentPos.y != (tileCountY - 1)
            && currentPos.x != 0
            && (chessPieceMap[currentPos.x - 1, currentPos.y + 1] == null ||  chessPieceMap[currentPos.x - 1, currentPos.y +1].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x - 1, currentPos.y +1));
        }
        
        //1 left-back
        if (currentPos.y != 0 
            && currentPos.x != (tileCountX - 1)
            && (chessPieceMap[currentPos.x + 1, currentPos.y -1] == null || chessPieceMap[currentPos.x + 1, currentPos.y -1].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x + 1, currentPos.y -1));
        }
        
        //1 right-back
        if (currentPos.y != 0 
            && currentPos.x != 0
            && (chessPieceMap[currentPos.x - 1, currentPos.y -1] == null || chessPieceMap[currentPos.x - 1, currentPos.y -1].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x - 1, currentPos.y -1));
        }
        
        return possibleMoves;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] chessPieceMap, ref List<Vector2Int[]> moveList, ref List<Vector2Int> possibleMoves)
    {
        SpecialMove specialMove = SpecialMove.NONE;
        Vector2Int currentPos = ChessGameUtil.floorToIntVector2Int(this.currentPosition);

        Vector2Int kingPosition = new Vector2Int(3, (this.team == Team.BLACK) ? 0 : 7);
        //Find previous -> KingMove, and rookMoves
        var kingMove = moveList.Find(   m => m[0].x == kingPosition.x 
                                        && m[0].y == kingPosition.y
                                    );
        Vector2Int leftRookPosition = new Vector2Int(0, (this.team == Team.BLACK) ? 0 : 7);
        var leftRookMove = moveList.Find(m => m[0].x == leftRookPosition.x
                                     && m[0].y == leftRookPosition.y
                                    );

        Vector2Int rightRookPosition = new Vector2Int(7, (this.team == Team.BLACK) ? 0 : 7);

        var rightRookMove = moveList.Find(m => m[0].x == rightRookPosition.x
                                     && m[0].y == rightRookPosition.y
                                    );

        
        if(kingMove == null && currentPos.x == 3)
        {
            if (team == Team.BLACK)
            {
                if (leftRookMove == null && chessPieceMap[0 ,0].type == ChessPieceType.ROOK && chessPieceMap[0, 0].team == Team.BLACK)
                {
                    if(this.isEmptyBetweenLeftRook(chessPieceMap, 0))
                    {
                        possibleMoves.Add(new Vector2Int(1, 0));
                        specialMove = SpecialMove.CASTLING;
                    }
                }

                if (leftRookMove == null && chessPieceMap[0, 0].type == ChessPieceType.ROOK && chessPieceMap[0, 0].team == Team.BLACK)
                {
                    if (this.isEmptyBetweenRightRook(chessPieceMap, 0))
                    {
                        possibleMoves.Add(new Vector2Int(5, 0));
                        specialMove = SpecialMove.CASTLING;
                    }
                }
            }
            else if(team == Team.WHITE)
            {
                if (leftRookMove == null && chessPieceMap[0, 7].type == ChessPieceType.ROOK && chessPieceMap[0, 7].team == Team.WHITE)
                {
                    if (this.isEmptyBetweenLeftRook(chessPieceMap, 7))
                    {
                        possibleMoves.Add(new Vector2Int(1, 7));
                        specialMove = SpecialMove.CASTLING;
                    }
                }

                if (leftRookMove == null && chessPieceMap[0, 7].type == ChessPieceType.ROOK && chessPieceMap[0, 7].team == Team.WHITE)
                {
                    if (this.isEmptyBetweenRightRook(chessPieceMap, 7))
                    {
                        possibleMoves.Add(new Vector2Int(5, 7));
                        specialMove = SpecialMove.CASTLING;
                    }
                }
            }
        }

        return specialMove;
    }

    private bool isEmptyBetweenLeftRook(ChessPiece[,] chessPieceMap, int yIndex)
    {
        if (chessPieceMap[1, yIndex] == null && chessPieceMap[2, yIndex] == null) return true;

        return false;
    }

    private bool isEmptyBetweenRightRook(ChessPiece[,] chessPieceMap, int yIndex)
    {
        for (int i = 4; i < 7; i++)
        {
            if(chessPieceMap[i, yIndex] != null)
            {
                return false;
            }
        }
        return true;
    }
}
