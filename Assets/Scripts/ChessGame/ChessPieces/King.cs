using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(ref ChessPiece[,] chessPieceMap, int tileCountX, int tileCountY)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        //Go up if you are White otherwise go down
        int direction = (this.team == Team.BLACK) ? 1 : -1;

        //1 forward
        Vector2Int currentPos = ChessGameUtil.floorToIntVector2Int(this.currentPosition);

        if (currentPos.y != (tileCountY-1)
            && (chessPieceMap[currentPos.x, currentPos.y + direction] == null || chessPieceMap[currentPos.x, currentPos.y + direction].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x, currentPos.y + direction));
        }

        //1 back
        if (currentPos.y != 0 
            && (chessPieceMap[currentPos.x, currentPos.y - direction] == null || chessPieceMap[currentPos.x, currentPos.y - direction].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x, currentPos.y - direction));
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
            && (chessPieceMap[currentPos.x +1, currentPos.y + direction] == null || chessPieceMap[currentPos.x + 1, currentPos.y + direction].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x + 1, currentPos.y + direction));
        }
        
        //1 right-front
        if (currentPos.y != (tileCountY - 1)
            && currentPos.x != 0
            && (chessPieceMap[currentPos.x - 1, currentPos.y + direction] == null ||  chessPieceMap[currentPos.x - 1, currentPos.y + direction].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x - 1, currentPos.y + direction));
        }
        
        //1 left-back
        if (currentPos.y != 0 
            && currentPos.x != (tileCountX - 1)
            && (chessPieceMap[currentPos.x + 1, currentPos.y - direction] == null || chessPieceMap[currentPos.x + 1, currentPos.y - direction].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x + 1, currentPos.y - direction));
        }
        
        //1 right-back
        if (currentPos.y != 0 
            && currentPos.x != 0
            && (chessPieceMap[currentPos.x - 1, currentPos.y - direction] == null || chessPieceMap[currentPos.x - 1, currentPos.y - direction].team != this.team)
            )
        {
            possibleMoves.Add(new Vector2Int(currentPos.x - 1, currentPos.y - direction));
        }
        
        return possibleMoves;
    }
}
