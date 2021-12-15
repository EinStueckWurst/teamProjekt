using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(ref ChessPiece[,] chessPieceMap, int tileCountX, int tileCountY)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        Vector2Int currentPos = ChessGameUtil.floorToIntVector2Int(this.currentPosition);


        int i = 1;
        //NorthEast
        while ((currentPos.x + i) < tileCountX && (currentPos.y + i) < tileCountY)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x + i, currentPos.y + i] != null
                && chessPieceMap[currentPos.x + i, currentPos.y + i].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x + i, currentPos.y + i] != null
                && chessPieceMap[currentPos.x + i, currentPos.y + i].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x + i, currentPos.y + i));
                break;
            }
            possibleMoves.Add(new Vector2Int(currentPos.x + i, currentPos.y + i));

            i++;
        }

        //NorthWest
        i = 1;
        while ((currentPos.x - i) >= 0 && (currentPos.y + i) < tileCountY)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x - i, currentPos.y + i] != null
                && chessPieceMap[currentPos.x - i, currentPos.y + i].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x - i, currentPos.y + i] != null
                && chessPieceMap[currentPos.x - i, currentPos.y + i].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x - i, currentPos.y + i));
                break;
            }
            possibleMoves.Add(new Vector2Int(currentPos.x - i, currentPos.y + i));

            i++;
        }

        //SouthWest
        i = 1;
        while ((currentPos.x - i) >= 0 && (currentPos.y - i) >= 0)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x - i, currentPos.y - i] != null
                && chessPieceMap[currentPos.x - i, currentPos.y - i].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x - i, currentPos.y - i] != null
                && chessPieceMap[currentPos.x - i, currentPos.y - i].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x - i, currentPos.y - i));
                break;
            }
            possibleMoves.Add(new Vector2Int(currentPos.x - i, currentPos.y - i));

            i++;
        }

        //SouthEast
        i = 1;
        while ((currentPos.x + i) < tileCountX && (currentPos.y - i) >= 0)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x + i, currentPos.y - i] != null
                && chessPieceMap[currentPos.x + i, currentPos.y - i].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x + i, currentPos.y - i] != null
                && chessPieceMap[currentPos.x + i, currentPos.y - i].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x + i, currentPos.y - i));
                break;
            }
            possibleMoves.Add(new Vector2Int(currentPos.x + i, currentPos.y - i));

            i++;
        }

        return possibleMoves;
    }
}
