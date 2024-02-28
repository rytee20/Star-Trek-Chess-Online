using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector3Int> GetAvaibleMoves(ref ChessPiece[,,] board, ref GameObject[,,] tiles)
    {
        const int TileCountX = 6;
        const int TileCountY = 10;
        const int TileCountZ = 6;
        bool free = true;
        List<Vector3Int> r = new List<Vector3Int>();

        //Назад
        int y = currentY - 1;
        while (free && y >= 0)
        {
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[currentX, y, z] != null && (board[currentX, y, z] == null || board[currentX, y, z].team != this.team))
                {
                    r.Add(new Vector3Int(currentX, y, z));
                }
                if (board[currentX, y, z] != null)
                    free = false;
            }
            y--;
        }
        free = true;

        //Вперёд
        y = currentY + 1;
        while (free && y < TileCountY)
        {
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[currentX, y, z] != null && (board[currentX, y, z] == null || board[currentX, y, z].team != this.team))
                {
                    r.Add(new Vector3Int(currentX, y, z));
                }
                if (board[currentX, y, z] != null)
                    free = false;
            }
            y++;
        }
        free = true;

        //Влево
        int x = currentX - 1;
        while (free && x >= 0)
        {
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[x, currentY, z] != null && (board[x, currentY, z] == null || board[x, currentY, z].team != this.team))
                {
                    r.Add(new Vector3Int(x, currentY, z));
                }
                if (board[x, currentY, z] != null)
                    free = false;
            }
            x--;
        }
        free = true;

        //Вправо
        x = currentX + 1;
        while (free && x < TileCountX)
        {
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[x, currentY, z] != null && (board[x, currentY, z] == null || board[x, currentY, z].team != this.team))
                {
                    r.Add(new Vector3Int(x, currentY, z));
                }
                if (board[x, currentY, z] != null)
                    free = false;
            }
            x++;
        }


        return r;
    }

}
