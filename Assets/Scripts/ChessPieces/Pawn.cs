using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector3Int> GetAvaibleMoves(ref ChessPiece[,,] board, ref GameObject[,,] tiles)
    {
        const int TileCountX = 6;
        const int TileCountY = 10;
        const int TileCountZ = 6;
        bool free = true;
        int x, y;
        List<Vector3Int> r = new List<Vector3Int>();

        int direction = (team == 0) ? 1 : -1;

        //движение вперёд
        x = currentX;
        y = currentY + direction;
        if (y >= 0 && y < TileCountY)
            for (int z = 0; z < TileCountZ; z++)
            {
                if (tiles[x, y, z] != null && board[x, y, z] == null)
                    r.Add(new Vector3Int(x, y, z));
                if (board[x, y, z] != null)
                    free = false;
            }

        //движение вперёд на 2 поля
        if (!WasMoved && free)
        {
            x = currentX;
            y = currentY + 2 * direction;
            if (y >= 0 && y < TileCountY)
                for (int z = 0; z < TileCountZ; z++)
                    if (tiles[x, y, z] != null && board[x, y, z] == null)
                        r.Add(new Vector3Int(x, y, z));
        }

        //удар по диагонали влево
        x = currentX - 1;
        y = currentY + direction;
        if (y >= 0 && y < TileCountY && x >= 0)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && board[x, y, z] != null && board[x, y, z].team != this.team)
                    r.Add(new Vector3Int(x, y, z));

        //удар по диагонали вправо
        x = currentX + 1;
        y = currentY + direction;
        if (y >= 0 && y < TileCountY && x < TileCountX)
            for (int z = 0; z < TileCountZ; z++)
                if (tiles[x, y, z] != null && board[x, y, z] != null && board[x, y, z].team != this.team)
                    r.Add(new Vector3Int(x, y, z));

        return r;
    }
}
