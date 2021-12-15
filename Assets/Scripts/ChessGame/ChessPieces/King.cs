using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(ref ChessPiece[,] chessPieceMap, int tileCountX, int tileCountY)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        Vector2Int currentPos = ChessGameUtil.floorToIntVector2Int(this.currentPosition);

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
}
