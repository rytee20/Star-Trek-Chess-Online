using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector3Int> GetAvaibleMoves(ref ChessPiece[,,] board, ref GameObject[,,] tiles)
    {
        const int TileCountX = 6;
        const int TileCountY = 10;
        const int TileCountZ = 6;
        //bool free = true;
        int x, y;
        List<Vector3Int> r = new List<Vector3Int>();

        //�����
        x = currentX - 1;
        y = currentY;
        if (x >= 0)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //������
        x = currentX + 1;
        y = currentY;
        if (x < TileCountX)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //�����
        x = currentX;
        y = currentY - 1;
        if (y >= 0)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //�����
        x = currentX;
        y = currentY + 1;
        if (y < TileCountY)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //����� �����
        x = currentX - 1;
        y = currentY - 1;
        if (y >= 0 && x >= 0)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //����� ������
        x = currentX + 1;
        y = currentY - 1;
        if (y >= 0 && x < TileCountX)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //����� �����
        x = currentX - 1;
        y = currentY + 1;
        if (y < TileCountY && x >= 0)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //����� ������
        x = currentX + 1;
        y = currentY + 1;
        if (y < TileCountY && x < TileCountX)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //����� � �����
        x = currentX + 1;
        y = currentY + 1;
        if (y < TileCountY && x < TileCountX)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //�������� ��������� �����
        if (this.team == 0)
        {
            if (board[5, 0, 1] != null && board[5, 0, 1].WasMoved == false)
                r.Add(new Vector3Int(5, 0, 1));
        }

        //������� ��������� �����
        if (this.team == 0)
        {
            if (board[1, 0, 1] == null && board[0, 0, 1] != null && board[0, 0, 1].WasMoved == false)
                r.Add(new Vector3Int(0, 0, 1));
        }


        //�������� ��������� ������
        if (this.team == 1)
        {
            if (board[5, 9, 5] != null && board[5, 9, 5].WasMoved == false)
                r.Add(new Vector3Int(5, 9, 5));
        }

        //������� ��������� ������
        if (this.team == 1)
        {
            if (board[1, 9, 5] == null && board[0, 9, 5] != null && board[0, 9, 5].WasMoved == false)
                r.Add(new Vector3Int(0, 9, 5));
        }

        return r;



    }
}
