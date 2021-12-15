using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(ref ChessPiece[,] chessPieceMap, int tileCountX, int tileCountY)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        Vector2Int currentPos = ChessGameUtil.floorToIntVector2Int(this.currentPosition);

        //North
        for (int i = currentPos.y + 1; i < tileCountY; i++)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x, i] != null
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
        for (int i = currentPos.y - 1; i >= 0; i--)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x, i] != null
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

        int j = 1;
        //NorthEast
        while ((currentPos.x + j) < tileCountX && (currentPos.y + j) < tileCountY)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x + j, currentPos.y + j] != null
                && chessPieceMap[currentPos.x + j, currentPos.y + j].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x + j, currentPos.y + j] != null
                && chessPieceMap[currentPos.x + j, currentPos.y + j].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x + j, currentPos.y + j));
                break;
            }
            possibleMoves.Add(new Vector2Int(currentPos.x + j, currentPos.y + j));

            j++;
        }

        //NorthWest
        j = 1;
        while ((currentPos.x - j) >= 0 && (currentPos.y + j) < tileCountY)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x - j, currentPos.y + j] != null
                && chessPieceMap[currentPos.x - j, currentPos.y + j].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x - j, currentPos.y + j] != null
                && chessPieceMap[currentPos.x - j, currentPos.y + j].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x - j, currentPos.y + j));
                break;
            }
            possibleMoves.Add(new Vector2Int(currentPos.x - j, currentPos.y + j));

            j++;
        }

        //SouthWest
        j = 1;
        while ((currentPos.x - j) >= 0 && (currentPos.y - j) >= 0)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x - j, currentPos.y - j] != null
                && chessPieceMap[currentPos.x - j, currentPos.y - j].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x - j, currentPos.y - j] != null
                && chessPieceMap[currentPos.x - j, currentPos.y - j].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x - j, currentPos.y - j));
                break;
            }
            possibleMoves.Add(new Vector2Int(currentPos.x - j, currentPos.y - j));

            j++;
        }

        //SouthEast
        j = 1;
        while ((currentPos.x + j) < tileCountX && (currentPos.y - j) >= 0)
        {
            //Break on friend
            if (chessPieceMap[currentPos.x + j, currentPos.y - j] != null
                && chessPieceMap[currentPos.x + j, currentPos.y - j].team == this.team)
            {
                break;
            }

            //Break on enemy
            if (chessPieceMap[currentPos.x + j, currentPos.y - j] != null
                && chessPieceMap[currentPos.x + j, currentPos.y - j].team != this.team)
            {
                possibleMoves.Add(new Vector2Int(currentPos.x + j, currentPos.y - j));
                break;
            }
            possibleMoves.Add(new Vector2Int(currentPos.x + j, currentPos.y - j));

            j++;
        }

        return possibleMoves;
    }
}
