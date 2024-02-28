using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector3Int> GetAvaibleMoves(ref ChessPiece[,,] board, ref GameObject[,,] tiles)
    {
        const int TileCountX = 6;
        const int TileCountY = 10;
        //const int TileCountZ = 6;


        List<Vector3Int> r = new List<Vector3Int>();

        //Вперёд вправо
        int x = currentX + 1;
        int y = currentY + 2;
        if (x < TileCountX && y < TileCountY)
            for (int z = 0; z < 6; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));



        x = currentX + 2;
        y = currentY + 1;
        if (x < TileCountX && y < TileCountY)
            for (int z = 0; z < 6; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));



        //Вперёд влево
        x = currentX - 1;
        y = currentY + 2;
        if (x >= 0 && y < TileCountY)
            for (int z = 0; z < 6; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        x = currentX - 2;
        y = currentY + 1;
        if (x >= 0 && y < TileCountY)
            for (int z = 0; z < 6; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //Назад вправо
        x = currentX + 2;
        y = currentY - 1;
        if (x < TileCountX && y >= 0)
            for (int z = 0; z < 6; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        x = currentX + 1;
        y = currentY - 2;
        if (x < TileCountX && y >= 0)
            for (int z = 0; z < 6; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        //Назад влево
        x = currentX - 2;
        y = currentY - 1;
        if (x >= 0 && y >= 0)
            for (int z = 0; z < 6; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));

        x = currentX - 1;
        y = currentY - 2;
        if (x >= 0 && y >= 0)
            for (int z = 0; z < 6; z++)
                if (tiles[x, y, z] != null && (board[x, y, z] == null || board[x, y, z].team != this.team))
                    r.Add(new Vector3Int(x, y, z));


        return r;



    }
}
