using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector3Int> GetAvaibleMoves(ref ChessPiece[,,] board, ref GameObject[,,] tiles)
    {
        const int TileCountX = 6;
        const int TileCountY = 10;
        const int TileCountZ = 6;
        bool free = true;
        List<Vector3Int> r = new List<Vector3Int>();

        //Назад Влево
        int x = currentX - 1;
        int y = currentY - 1;

        while (free && y >= 0 && x >= 0)
        {
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                {
                    r.Add(new Vector3Int(x, y, z));
                }
                if (board[x, y, z] != null)
                    free = false;
            }
            x--;
            y--;
        }
        free = true;

        //Вперёд Влево
        x = currentX - 1;
        y = currentY + 1;

        while (free && y < TileCountY && x >= 0)
        {
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                {
                    r.Add(new Vector3Int(x, y, z));
                }
                if (board[x, y, z] != null)
                    free = false;
            }
            x--;
            y++;
        }
        free = true;

        //Назад Вправо
        x = currentX + 1;
        y = currentY - 1;

        while (free && y >= 0 && x < TileCountX)
        {
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                {
                    r.Add(new Vector3Int(x, y, z));
                }
                if (board[x, y, z] != null)
                    free = false;
            }
            x++;
            y--;
        }
        free = true;

        //Вперёд Вправо
        x = currentX + 1;
        y = currentY + 1;

        while (free && y < TileCountY && x < TileCountX)
        {
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                {
                    r.Add(new Vector3Int(x, y, z));
                }
                if (board[x, y, z] != null)
                    free = false;
            }
            x++;
            y++;
        }

        return r;
    }
}
