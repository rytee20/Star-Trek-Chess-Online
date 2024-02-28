using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingBoard : MonoBehaviour
{
    public int start_team;
    public int currentX;
    public int currentY;
    public int currentZ;

    private const int LITTLE_TILE_COUNT = 2;
    private const int BOARD_COUNT_X = 2;
    private const int BOARD_COUNT_Y = 5;
    private const int BOARD_COUNT_Z = 3;
    private Vector3 desiredPosition;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
            transform.position = desiredPosition;
    }

    public int GetCurrentTeam(ref ChessPiece[,,] board)
    {
        for (int i = 0; i < LITTLE_TILE_COUNT; i++)
            for (int j = 0; j < LITTLE_TILE_COUNT; j++)
                 if (board[currentX * 4 + i, currentY * 2 + j, currentZ * 2 + 1] != null)
                    return board[currentX * 4 + i, currentY * 2 + j, currentZ * 2 + 1].team;
         return start_team;
    }

    public int GetNumberPieces(ref ChessPiece[,,] board)
    {
        int count = 0;
        for (int i = 0; i < LITTLE_TILE_COUNT; i++)
            for (int j = 0; j < LITTLE_TILE_COUNT; j++)
                if (board[currentX * 4 + i, currentY * 2 + j, currentZ * 2 + 1] != null)
                    count++;

        return count;
    }

    public List<Vector3Int> GetAvaibleMoves(ref AttackingBoard[,,] boards, ref GameObject[,,] pins, ref ChessPiece[,,] pieces)
    {
        int N;
        List<Vector3Int> r = new List<Vector3Int>();
        if ((N = GetNumberPieces(ref pieces)) > 1)
            return r;

        // влево/вправо
        int x, y, z;
        x = currentX + 1;
        y = currentY;
        z = currentZ;
        if ((x < BOARD_COUNT_X)&&(pins[x,y,z]!=null)&&(boards[x,y,z]==null))
            r.Add(new Vector3Int(x, y, z));

        x = currentX - 1;
        if ((x >= 0) && (pins[x, y, z] != null) && (boards[x, y, z] == null))
            r.Add(new Vector3Int(x, y, z));

        //вперёд
        if ((GetCurrentTeam(ref pieces) == 0) || (GetNumberPieces(ref pieces) == 0))
        {
            x = currentX;
            y = currentY + 2;
            if ((y < BOARD_COUNT_Y) && (pins[x, y, z] != null) && (boards[x, y, z] == null))
                r.Add(new Vector3Int(x, y, z));
        }

        //вперёд и вверх
        if ((GetCurrentTeam(ref pieces) == 0) || (GetNumberPieces(ref pieces) == 0))
        {
            x = currentX;
            y = currentY + 1;
            z = currentZ + 1;
            if ((y < BOARD_COUNT_Y) && (z < BOARD_COUNT_Z) && (pins[x, y, z] != null) && (boards[x, y, z] == null))
                r.Add(new Vector3Int(x, y, z));
        }

        //вперёд и вниз
        if ((GetCurrentTeam(ref pieces) == 0) || (GetNumberPieces(ref pieces) == 0))
        {
            x = currentX;
            y = currentY + 1;
            z = currentZ - 1;
            if ((y < BOARD_COUNT_Y) && (z >= 0) && (pins[x, y, z] != null) && (boards[x, y, z] == null))
                r.Add(new Vector3Int(x, y, z));
        }

        //назад
        if ((GetCurrentTeam(ref pieces) == 1) || (GetNumberPieces(ref pieces) == 0))
        {
            x = currentX;
            y = currentY - 2;
            z = currentZ;
            if ((y >= 0) && (pins[x, y, z] != null) && (boards[x, y, z] == null))
                r.Add(new Vector3Int(x, y, z));
        }

        //назад и вниз
        if ((GetCurrentTeam(ref pieces) == 1) || (GetNumberPieces(ref pieces) == 0))
        {
            x = currentX;
            y = currentY - 1;
            z = currentZ - 1;
            if ((y >= 0) && (z >= 0) && (pins[x, y, z] != null) && (boards[x, y, z] == null))
                r.Add(new Vector3Int(x, y, z));
        }

        //назад и вверх
        if ((GetCurrentTeam(ref pieces) == 1) || (GetNumberPieces(ref pieces) == 0))
        {
            x = currentX;
            y = currentY - 1;
            z = currentZ + 1;
            if ((y >= 0) && (z < BOARD_COUNT_Z) && (pins[x, y, z] != null) && (boards[x, y, z] == null))
                r.Add(new Vector3Int(x, y, z));
        }

        return r;
    }
}
