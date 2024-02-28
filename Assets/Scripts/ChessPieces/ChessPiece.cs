using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public enum ChessPieceType
{
    Pawn = 0, // Пешка
    Rook = 1, // Ладья
    Knight = 2, // Конь
    Bishop = 3, // Слон
    Queen = 4, // Ферзь
    King = 5 // Король
}

public class ChessPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public int level;
    public bool WasMoved = false;
    public ChessPieceType type;
    public int pieceType;

    private Vector3 desiredPosition;
    //private Vector3 desiredScale = new Vector3(1300, 1300, 1300);

    private void Start()
    {
        if(pieceType==0)
            transform.rotation = Quaternion.Euler((team==0) ? new Vector3(270,-90,90) : new Vector3(270,90,90));
        if (pieceType == 1)
        {
            if(type != ChessPieceType.Knight && team != 0) transform.rotation = Quaternion.Euler(new Vector3(-90, 180, 0));
            if (type == ChessPieceType.Knight && team != 0) transform.rotation = Quaternion.Euler(new Vector3(-90, 180, -90));
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        //transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    virtual public List<Vector3Int> GetAvaibleMoves(ref ChessPiece[,,] board, ref GameObject[,,] tiles)
    {
        List<Vector3Int> r = new List<Vector3Int>();
        r.Add(new Vector3Int(2, 2, 0));
        r.Add(new Vector3Int(3, 3, 0));
        r.Add(new Vector3Int(4, 4, 0));

        return r;
    }
    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
            transform.position = desiredPosition;
    }

    //public virtual void SetScale(Vector3 scale, bool force = false)
    //{
    //    desiredScale = scale;
    //    if (force)
    //        transform.localScale = desiredScale;
    //}
}
