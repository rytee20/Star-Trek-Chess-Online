using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector3Int> GetAvaibleMoves(ref ChessPiece[,,] board, ref GameObject[,,] tiles)
    {
        const int TileCountX = 6;
        const int TileCountY = 10;
        const int TileCountZ = 6;
        bool free = true;
        List<Vector3Int> r = new List<Vector3Int>();

        //�����
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

        //�����
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

        //�����
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

        //������
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
        free = true;

        //����� �����
        x = currentX - 1;
        y = currentY - 1;

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

        //����� �����
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

        //����� ������
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

        //����� ������
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
