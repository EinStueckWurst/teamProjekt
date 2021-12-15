using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(ref ChessPiece[,] chessPieceMap, int tileCountX, int tileCountY)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        Vector2Int currentPos = ChessGameUtil.floorToIntVector2Int(this.currentPosition);

        //North
        for (int i = currentPos.y+1; i < tileCountY; i++)
        {
            //Break on friend
            if(chessPieceMap[currentPos.x, i] != null 
                && chessPieceMap[currentPos.x, i].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x, i] != null 
                && chessPieceMap[currentPos.x, i].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x, i));
                break;
            }

            possibleMoves.Add(new Vector2Int(currentPos.x, i));
        }
        
        //South
        for (int i = currentPos.y-1; i >= 0; i--)
        {
            //Break on friend
            if(chessPieceMap[currentPos.x, i] != null 
                && chessPieceMap[currentPos.x, i].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x, i] != null 
                && chessPieceMap[currentPos.x, i].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x, i));
                break;
            }

            possibleMoves.Add(new Vector2Int(currentPos.x, i));
        }

        //East
        for (int i = currentPos.x + 1; i < tileCountX; i++)
        {
            //break on friend
            if (chessPieceMap[i, currentPos.y] != null
                && chessPieceMap[i, currentPos.y].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[i, currentPos.y] != null
                && chessPieceMap[i, currentPos.y].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(i, currentPos.y));
                break;
            }

            possibleMoves.Add(new Vector2Int(i, currentPos.y));
        }
        
        //West
        for (int i = currentPos.x - 1; i >= 0; i--)
        {
            //break on friend
            if (chessPieceMap[i, currentPos.y] != null
                && chessPieceMap[i, currentPos.y].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[i, currentPos.y] != null
                && chessPieceMap[i, currentPos.y].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(i, currentPos.y));
                break;
            }

            possibleMoves.Add(new Vector2Int(i, currentPos.y));
        }

        return possibleMoves;
    }


}
