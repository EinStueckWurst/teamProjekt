using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(ref ChessPiece[,] chessPieceMap, int tileCountX, int tileCountY)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        //1 forward
        Vector2Int currentPos = ChessGameUtil.floorToIntVector2Int(this.currentPosition);

        //2 Steps North
        if(currentPos.y + 2 < tileCountY)
        {
            if(currentPos.x + 1 < tileCountX 
                && (chessPieceMap[currentPos.x + 1, currentPos.y + 2] == null 
                    || chessPieceMap[currentPos.x + 1, currentPos.y + 2].team != this.team)
                )
            {
                possibleMoves.Add(new Vector2Int(currentPos.x +1, currentPos.y +2));
            }
            
            if(currentPos.x -1 >= 0
                && (chessPieceMap[currentPos.x - 1, currentPos.y + 2] == null
                    || chessPieceMap[currentPos.x - 1, currentPos.y + 2].team != this.team)
                )
            {
                possibleMoves.Add(new Vector2Int(currentPos.x -1, currentPos.y +2));
            }
        }

        //2 Steps South
        if (currentPos.y - 2 >= 0)
        {
            if (currentPos.x + 1 < tileCountX
                && (chessPieceMap[currentPos.x + 1, currentPos.y -2] == null
                    || chessPieceMap[currentPos.x + 1, currentPos.y -2].team != this.team)
                )
            {
                possibleMoves.Add(new Vector2Int(currentPos.x + 1, currentPos.y - 2));
            }

            if (currentPos.x - 1 >= 0
                && (chessPieceMap[currentPos.x - 1, currentPos.y - 2] == null
                    || chessPieceMap[currentPos.x - 1, currentPos.y - 2].team != this.team)
                )
            {
                possibleMoves.Add(new Vector2Int(currentPos.x - 1, currentPos.y - 2));
            }
        }
        
        
        //2 Steps West
        if (currentPos.x - 2 >= 0)
        {
            if (currentPos.y + 1 < tileCountY
                && (chessPieceMap[currentPos.x -2 , currentPos.y +1 ] == null
                    || chessPieceMap[currentPos.x -2, currentPos.y +1].team != this.team)
                )
            {
                possibleMoves.Add(new Vector2Int(currentPos.x - 2, currentPos.y + 1));
            }

            if (currentPos.y - 1 >= 0
                && (chessPieceMap[currentPos.x - 2, currentPos.y - 1] == null
                    || chessPieceMap[currentPos.x - 2, currentPos.y - 1].team != this.team)
                )
            {
                possibleMoves.Add(new Vector2Int(currentPos.x - 2, currentPos.y - 1));
            }
        }

        //2 Steps East
        if (currentPos.x + 2 < tileCountX)
        {
            if (currentPos.y + 1 < tileCountY
                && (chessPieceMap[currentPos.x + 2, currentPos.y + 1] == null
                    || chessPieceMap[currentPos.x + 2, currentPos.y + 1].team != this.team)
                )
            {
                possibleMoves.Add(new Vector2Int(currentPos.x + 2, currentPos.y + 1));
            }

            if (currentPos.y - 1 >= 0
                && (chessPieceMap[currentPos.x + 2, currentPos.y - 1] == null
                    || chessPieceMap[currentPos.x + 2, currentPos.y - 1].team != this.team)
                )
            {
                possibleMoves.Add(new Vector2Int(currentPos.x + 2, currentPos.y - 1));
            }
        }

        return possibleMoves;
    }
}
