using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChessBoard : MonoBehaviour
{
    [Header("Art")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private float tileSize = 10.0f;
    [SerializeField] private float yOffset;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] public int pieceType = 0;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterialsBlack;
    [SerializeField] private Material[] teamMaterialsWhite;


    //LOGIC
    private ChessPiece currentlyDraggingPiece;
    private AttackingBoard currentlyDraggingBoard;
    private List<Vector3Int> avaibleMoves = new List<Vector3Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();

    private const int BIG_TILE_COUNT_X = 4;
    private const int BIG_TILE_COUNT_Y = 4;
    private const int LITTLE_TILE_COUNT_X = 2;
    private const int LITTLE_TILE_COUNT_Y = 2;

    private GameObject[,,] tiles;
    private GameObject[,,] pins;
    private ChessPiece[,,] chessPieces;
    private AttackingBoard[,,] boards;

    private Vector3 bounds = new Vector3(30, 0, 30);
    private float[] level = { 0, 20, 40, 60, 80, 100 };

    private Camera currentCamera;
    private Vector3Int currentHoverTile = -Vector3Int.one;
    private Vector3Int currentHoverPin = -Vector3Int.one;
    private bool IsWhiteTurn;



    private void Awake()
    {

        SpawnAllAttackingBoards();
        PositionAllBoards();

        GenerateAllTiles();
        GenerateAllPins();

        SpawnAllPieces();
        PositionAllPieces();

        IsWhiteTurn = true;
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 500, LayerMask.GetMask("Tile", "Pin", "HoverTile", "HoverPin", "Highlight", "HighlightPin")))
        {
            // Get the indexes of the tile i've hit
            Vector3Int hitPositionTile = LookUpTileIndex(info.transform.gameObject);
            Vector3Int hitPositionPin = LookUpPinIndex(info.transform.gameObject);

            //if we're covering a tile after not hovering any tiles
            if (currentHoverTile == -Vector3Int.one && hitPositionTile != -Vector3Int.one)
            {
                currentHoverTile = hitPositionTile;
                tiles[hitPositionTile.x, hitPositionTile.y, hitPositionTile.z].layer = LayerMask.NameToLayer("HoverTile");
            }

            if (currentHoverPin == -Vector3Int.one && hitPositionPin != -Vector3Int.one)
            {
                currentHoverPin = hitPositionPin;
                if (pins[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z] != null)
                    pins[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z].layer = LayerMask.NameToLayer("HoverPin");
            }

            // if we were already covering a tile, change the previous one
            if (currentHoverTile != -Vector3Int.one && currentHoverTile != hitPositionTile && hitPositionTile != -Vector3Int.one)
            {
                tiles[currentHoverTile.x, currentHoverTile.y, currentHoverTile.z].layer =
                    (ContainsValidMove(ref avaibleMoves, currentHoverTile)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHoverTile = hitPositionTile;
                tiles[hitPositionTile.x, hitPositionTile.y, hitPositionTile.z].layer = LayerMask.NameToLayer("HoverTile");
            }

            if (currentHoverPin != -Vector3Int.one && currentHoverPin != hitPositionPin && hitPositionPin != -Vector3Int.one)
            {
                pins[currentHoverPin.x, currentHoverPin.y, currentHoverPin.z].layer =
                   (ContainsValidMove(ref avaibleMoves, currentHoverPin)) ? LayerMask.NameToLayer("HighlightPin") : LayerMask.NameToLayer("Pin");
                currentHoverPin = hitPositionPin;
                pins[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z].layer = LayerMask.NameToLayer("HoverPin");
            }

            // if we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                if (hitPositionTile != -Vector3Int.one && chessPieces[hitPositionTile.x, hitPositionTile.y, hitPositionTile.z] != null)
                {
                    // is it our turn?
                    if ((chessPieces[hitPositionTile.x, hitPositionTile.y, hitPositionTile.z].team == 0 && IsWhiteTurn)
                        || (chessPieces[hitPositionTile.x, hitPositionTile.y, hitPositionTile.z].team == 1 && !IsWhiteTurn))
                    {
                        currentlyDraggingPiece = chessPieces[hitPositionTile.x, hitPositionTile.y, hitPositionTile.z];
                        avaibleMoves = currentlyDraggingPiece.GetAvaibleMoves(ref chessPieces, ref tiles);
                        HighlightTiles();
                    }
                }

                if (hitPositionPin != -Vector3Int.one && boards[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z] != null)
                {
                    //if(boards[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z].IsMovable(ref chessPieces))
                    // is it our turn?
                    if ((boards[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z].GetCurrentTeam(ref chessPieces) == 0 && IsWhiteTurn)
                        || (boards[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z].GetCurrentTeam(ref chessPieces) == 1 && !IsWhiteTurn))
                    {
                        currentlyDraggingBoard = boards[hitPositionPin.x, hitPositionPin.y, hitPositionPin.z];
                        avaibleMoves = currentlyDraggingBoard.GetAvaibleMoves(ref boards, ref pins, ref chessPieces);
                        HighlightPins();
                    }
                }
            }

            // if we releasing the mouse button
            if (currentlyDraggingPiece != null && Input.GetMouseButtonUp(0))
            {
                Vector3Int previousPosition = new Vector3Int(currentlyDraggingPiece.currentX, currentlyDraggingPiece.currentY, currentlyDraggingPiece.level);

                //bool castling = false;
                bool validMove = MoveToPiece(currentlyDraggingPiece, hitPositionTile.x, hitPositionTile.y, hitPositionTile.z, false, true, true);

                if (!validMove)
                    currentlyDraggingPiece.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y, previousPosition.z));
                else
                {
                    //записать ход фигуры
                    //WriteMove( previousPosition, hitPositionTile, true);
                    currentlyDraggingPiece.WasMoved = true;
                }
                currentlyDraggingPiece = null;
                RemoveHighlightTiles();
            }

            if (currentlyDraggingBoard != null && Input.GetMouseButtonUp(0))
            {
                Vector3Int previousPosition = new Vector3Int(currentlyDraggingBoard.currentX, currentlyDraggingBoard.currentY, currentlyDraggingBoard.currentZ);

                bool validMove = MoveToBoard(currentlyDraggingBoard, hitPositionPin.x, hitPositionPin.y, hitPositionPin.z);
                if (!validMove)
                    currentlyDraggingBoard.SetPosition(GetBoardPosition(previousPosition.x, previousPosition.y, previousPosition.z));
                currentlyDraggingBoard = null;
                RemoveHighlightPins();
            }
        }
        else
        {
            if (currentHoverTile != -Vector3Int.one)
            {
                //Debug.Log(currentHover.x.ToString()+'\t'+currentHover.y.ToString()+ '\t' + currentHover.z.ToString());
                tiles[currentHoverTile.x, currentHoverTile.y, currentHoverTile.z].layer =
                    (ContainsValidMove(ref avaibleMoves, currentHoverTile)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");

                currentHoverTile = -Vector3Int.one;
            }

            if (currentHoverPin != -Vector3Int.one && pins[currentHoverPin.x, currentHoverPin.y, currentHoverPin.z] != null)
            {
                pins[currentHoverPin.x, currentHoverPin.y, currentHoverPin.z].layer =
                    (ContainsValidMove(ref avaibleMoves, currentHoverPin)) ? LayerMask.NameToLayer("HighlightPin") : LayerMask.NameToLayer("Pin");

                currentHoverPin = -Vector3Int.one;
            }

            if (currentlyDraggingPiece && Input.GetMouseButtonUp(0))
            {
                currentlyDraggingPiece.SetPosition(GetTileCenter(currentlyDraggingPiece.currentX, currentlyDraggingPiece.currentY, currentlyDraggingPiece.level));
                //currentlyDragging.WasMoved = true;
                currentlyDraggingPiece = null;
                RemoveHighlightTiles();
            }

            if (currentlyDraggingBoard && Input.GetMouseButtonUp(0))
            {
                currentlyDraggingBoard.SetPosition(GetBoardPosition(currentlyDraggingBoard.currentX, currentlyDraggingBoard.currentY, currentlyDraggingBoard.currentZ));
                currentlyDraggingBoard = null;
                RemoveHighlightPins();
            }
        }
    }


    // Generate the board
    private void GenerateAllTiles()
    {
        yOffset += transform.position.y;
        tiles = new GameObject[6, 10, 6];
        //большие доски
        for (int x = 1; x < 1 + BIG_TILE_COUNT_X; x++)
        {
            for (int y = 1; y < 1 + BIG_TILE_COUNT_Y; y++)
            {
                tiles[x, y, 0] = GenerateSingleTile(x, y, 0);
            }
            for (int y = 3; y < 3 + BIG_TILE_COUNT_Y; y++)
            {
                tiles[x, y, 2] = GenerateSingleTile(x, y, 2);
            }
            for (int y = 5; y < 5 + BIG_TILE_COUNT_Y; y++)
            {
                tiles[x, y, 4] = GenerateSingleTile(x, y, 4);
            }
        }
        //атакующие доски

        GenerateAttackingBoardTiles(0, 0, 1, true);
        GenerateAttackingBoardTiles(0, 4, 1, false);
        GenerateAttackingBoardTiles(4, 0, 1, true);
        GenerateAttackingBoardTiles(4, 4, 1, false);

        GenerateAttackingBoardTiles(0, 2, 3, false);
        GenerateAttackingBoardTiles(0, 6, 3, false);
        GenerateAttackingBoardTiles(4, 2, 3, false);
        GenerateAttackingBoardTiles(4, 6, 3, false);


        GenerateAttackingBoardTiles(0, 4, 5, false);
        GenerateAttackingBoardTiles(0, 8, 5, true);
        GenerateAttackingBoardTiles(4, 4, 5, false);
        GenerateAttackingBoardTiles(4, 8, 5, true);
    }

    private void GenerateAttackingBoardTiles(int x, int y, int z, bool active)
    {
        for (int i = x; i < x + LITTLE_TILE_COUNT_X; i++)
            for (int j = y; j < y + LITTLE_TILE_COUNT_Y; j++)
            {
                tiles[i, j, z] = GenerateSingleTile(i, j, z);
                tiles[i, j, z].SetActive(active);
            }
    }
    private GameObject GenerateSingleTile(int x, int y, int z)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}, Z:{2}", x, y, z));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;
        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(x * tileSize, level[z] + yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, level[z] + yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, level[z] + yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, level[z] + yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }


    // Spawning of the pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[6, 10, 6];
        int whiteTeam = 0, blackTeam = 1;

        // White Pieces
        chessPieces[1, 1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 1, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 1, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[4, 1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        for (int i = 1; i < 1 + BIG_TILE_COUNT_X; i++)
        {
            chessPieces[i, 2, 0] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        }

        chessPieces[0, 0, 1] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0, 1] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0, 1] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0, 1] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);

        chessPieces[0, 1, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        chessPieces[1, 1, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        chessPieces[4, 1, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        chessPieces[5, 1, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);



        // Black Pieces
        chessPieces[1, 8, 4] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 8, 4] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 8, 4] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[4, 8, 4] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        for (int i = 1; i < 1 + BIG_TILE_COUNT_X; i++)
        {
            chessPieces[i, 7, 4] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }

        chessPieces[0, 9, 5] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 9, 5] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 9, 5] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 9, 5] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);

        chessPieces[0, 8, 5] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        chessPieces[1, 8, 5] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        chessPieces[4, 8, 5] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        chessPieces[5, 8, 5] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);

    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp;
        if (pieceType == 0)
            cp = Instantiate(prefabs[(int)type], transform).GetComponent<ChessPiece>();
        else
            cp = Instantiate(prefabs[(int)type + 8], transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team;
        cp.pieceType = pieceType;
        if (team == 0) cp.GetComponent<MeshRenderer>().materials = teamMaterialsWhite;
        if (team == 1) cp.GetComponent<MeshRenderer>().materials = teamMaterialsBlack;

        return cp;
    }


    // Generate the pins
    private void GenerateAllPins()
    {
        pins = new GameObject[2, 5, 3];


        pins[0, 0, 0] = GenerateSinglePin(0, 0, 0);
        pins[1, 0, 0] = GenerateSinglePin(1, 0, 0);
        pins[0, 2, 0] = GenerateSinglePin(0, 2, 0);
        pins[1, 2, 0] = GenerateSinglePin(1, 2, 0);

        pins[0, 1, 1] = GenerateSinglePin(0, 1, 1);
        pins[1, 1, 1] = GenerateSinglePin(1, 1, 1);
        pins[0, 3, 1] = GenerateSinglePin(0, 3, 1);
        pins[1, 3, 1] = GenerateSinglePin(1, 3, 1);

        pins[0, 2, 2] = GenerateSinglePin(0, 2, 2);
        pins[1, 2, 2] = GenerateSinglePin(1, 2, 2);
        pins[0, 4, 2] = GenerateSinglePin(0, 4, 2);
        pins[1, 4, 2] = GenerateSinglePin(1, 4, 2);


    }
    private GameObject GenerateSinglePin(int x, int y, int z)
    {
        GameObject PinObject = Instantiate(prefabs[7], transform);
        PinObject.transform.position = new Vector3(40 * x - 20, 8.5f + level[2 * z], 20 * y - 20);

        PinObject.layer = LayerMask.NameToLayer("Pin");
        PinObject.AddComponent<BoxCollider>();

        return PinObject;
    }

    // Spawning of the attacking boards
    private void SpawnAllAttackingBoards()
    {
        boards = new AttackingBoard[2, 5, 3];
        boards[0, 0, 0] = SpawnAttackingBoard(0);
        boards[1, 0, 0] = SpawnAttackingBoard(0);
        boards[0, 4, 2] = SpawnAttackingBoard(1);
        boards[1, 4, 2] = SpawnAttackingBoard(1);
    }
    private AttackingBoard SpawnAttackingBoard(int team)
    {
        AttackingBoard board = Instantiate(prefabs[6], transform).GetComponent<AttackingBoard>();
        board.start_team = team;
        return board;
    }


    // Positioning
    private void PositionAllPieces()
    {
        for (int z = 0; z < 6; z++)
            for (int x = 0; x < 6; x++)
                for (int y = 0; y < 10; y++)
                    if (chessPieces[x, y, z] != null)
                        PositionSinglePiece(x, y, z, true);

    }

    private void PositionSinglePiece(int x, int y, int z, bool force = false)
    {
        chessPieces[x, y, z].currentX = x;
        chessPieces[x, y, z].currentY = y;
        chessPieces[x, y, z].level = z;
        chessPieces[x, y, z].SetPosition(GetTileCenter(x, y, z), force);
    }

    private void PositionAllBoards()
    {
        for (int z = 0; z < 3; z++)
            for (int x = 0; x < 2; x++)
                for (int y = 0; y < 5; y++)
                    if (boards[x, y, z] != null)
                        PositionSingleBoard(x, y, z, true);

    }

    private void PositionSingleBoard(int x, int y, int z, bool force = false)
    {
        boards[x, y, z].currentX = x;
        boards[x, y, z].currentY = y;
        boards[x, y, z].currentZ = z;
        boards[x, y, z].SetPosition(GetBoardPosition(x, y, z), force);
    }

    private Vector3 GetTileCenter(int x, int y, int z)
    {
        if (pieceType == 0)
            return new Vector3(x * tileSize, level[z] + yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
        else
        {
            if (chessPieces[x, y, z].type == ChessPieceType.Pawn)
                return new Vector3(x * tileSize, level[z] + yOffset + 0.15f, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
            if (chessPieces[x, y, z].type == ChessPieceType.Rook)
                return new Vector3(x * tileSize, level[z] + yOffset + 1.5f, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
            if (chessPieces[x, y, z].type == ChessPieceType.Queen)
                return new Vector3(x * tileSize, level[z] + yOffset + 2.2f, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
            if (chessPieces[x, y, z].type == ChessPieceType.King)
                return new Vector3(x * tileSize, level[z] + yOffset + 2.2f, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
            if (chessPieces[x, y, z].type == ChessPieceType.Knight)
                return new Vector3(x * tileSize, level[z] + yOffset + 1.4f, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
            if (chessPieces[x, y, z].type == ChessPieceType.Bishop)
                return new Vector3(x * tileSize, level[z] + yOffset + 4.5f, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
            return new Vector3(x * tileSize, level[z] + yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
        }
    }

    private Vector3 GetBoardPosition(int x, int y, int z)
    {
        float rack_length = 20;
        return new Vector3(40 * x - 20, rack_length - 1 + level[2 * z], 20 * y - 20);
    }

    // Operations
    private bool ContainsValidMove(ref List<Vector3Int> moves, Vector3 pos)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y && moves[i].z == pos.z)
                return true;
        }
        return false;
    }
    private bool MoveToPiece(ChessPiece cp, int x, int y, int z, bool force, bool writing, bool switch_turn)
    {
        if (!force && !ContainsValidMove(ref avaibleMoves, new Vector3(x, y, z)))
        {
            return false;
        }
        Vector3Int previousPosition = new Vector3Int(cp.currentX, cp.currentY, cp.level);

        // is there another piece on the target position?
        if (chessPieces[x, y, z] != null)
        {
            ChessPiece ocp = chessPieces[x, y, z];
            if ((ocp.team == cp.team) && (cp.type == ChessPieceType.King))
            {
                if (cp.team == 0) //рокировка белых
                    if (x == 5) // короткая 
                    {
                        Castling(0, true);
                        return true;
                    }
                    else // длинная 
                    {
                        Castling(0, false);
                        return true;
                    }
                else//рокировка чёрных
                {
                    if (x == 5) // короткая 
                    {
                        Castling(1, true);
                        return true;
                    }
                    else // длинная 
                    {
                        Castling(1, false);
                        return true;
                    }

                }
            }


            // if its the enemy team
            if (ocp.team == 0)
            {
                if (ocp.type == ChessPieceType.King)
                    CheckMate(1);

                deadWhites.Add(ocp);
                Destroy(chessPieces[x, y, z].GetComponent<MeshRenderer>());
            }
            else
            {
                if (ocp.type == ChessPieceType.King)
                    CheckMate(0);

                deadBlacks.Add(ocp);
                Destroy(chessPieces[x, y, z].GetComponent<MeshRenderer>());
            }
        }

        chessPieces[x, y, z] = cp;
        chessPieces[previousPosition.x, previousPosition.y, previousPosition.z] = null;

        PositionSinglePiece(x, y, z);

        if (switch_turn)
            IsWhiteTurn = !IsWhiteTurn;
        return true;
    }

    private bool MoveToBoard(AttackingBoard ab, int x, int y, int z)
    {
        if (!ContainsValidMove(ref avaibleMoves, new Vector3(x, y, z)))
            return false;
        Vector3Int previousPosition = new Vector3Int(ab.currentX, ab.currentY, ab.currentZ);

        // is there another board on the target position?
        //Debug.Log(x.ToString()+ y.ToString()+ z.ToString());
        if (boards[x, y, z] != null)
            return false;

        boards[x, y, z] = ab;
        boards[previousPosition.x, previousPosition.y, previousPosition.z] = null;
        MovePiecesToBoard(ab, x, y, z);
        SwitchTiles(ab, x, y, z);
        PositionSingleBoard(x, y, z);

        IsWhiteTurn = !IsWhiteTurn;
        return true;
    }

    private void SwitchTiles(AttackingBoard ab, int x, int y, int z)
    {
        for (int i = 0; i < LITTLE_TILE_COUNT_X; i++)
            for (int j = 0; j < LITTLE_TILE_COUNT_X; j++)
            {
                tiles[ab.currentX * 4 + i, ab.currentY * 2 + j, ab.currentZ * 2 + 1].SetActive(false);
                tiles[x * 4 + i, y * 2 + j, z * 2 + 1].SetActive(true);
            }

    }

    private void MovePiecesToBoard(AttackingBoard ab, int x, int y, int z)
    {
        for (int i = 0; i < LITTLE_TILE_COUNT_X; i++)
            for (int j = 0; j < LITTLE_TILE_COUNT_X; j++)
                if (chessPieces[ab.currentX * 4 + i, ab.currentY * 2 + j, ab.currentZ * 2 + 1] != null)
                    MoveToPiece(chessPieces[ab.currentX * 4 + i, ab.currentY * 2 + j, ab.currentZ * 2 + 1], x * 4 + i, y * 2 + j, z * 2 + 1, true, false, false);
    }

    private void Castling(int team, bool short_castling)
    {
        if (team == 0)
            if (short_castling)
            {
                (chessPieces[5, 0, 1], chessPieces[4, 0, 1]) =
                    (chessPieces[4, 0, 1], chessPieces[5, 0, 1]);
                PositionSinglePiece(5, 0, 1);
                PositionSinglePiece(4, 0, 1);
            }
            else
            {
                chessPieces[1, 0, 1] = chessPieces[0, 0, 1];
                chessPieces[0, 0, 1] = chessPieces[4, 0, 1];
                chessPieces[4, 0, 1] = null;

                PositionSinglePiece(0, 0, 1);
                PositionSinglePiece(1, 0, 1);
            }
        else
        {
            if (short_castling)
            {
                (chessPieces[5, 9, 5], chessPieces[4, 9, 5]) =
                    (chessPieces[4, 9, 5], chessPieces[5, 9, 5]);
                PositionSinglePiece(5, 9, 5);
                PositionSinglePiece(4, 9, 5);
            }
            else
            {
                chessPieces[1, 9, 5] = chessPieces[0, 9, 5];
                chessPieces[0, 9, 5] = chessPieces[4, 9, 5];
                chessPieces[4, 9, 5] = null;

                PositionSinglePiece(0, 9, 5);
                PositionSinglePiece(1, 9, 5);
            }
        }
        IsWhiteTurn = !IsWhiteTurn;
    }

    private Vector3Int LookUpTileIndex(GameObject hitInfo)
    {
        if (hitInfo.layer == LayerMask.NameToLayer("Tile") || hitInfo.layer == LayerMask.NameToLayer("HoverTile") || hitInfo.layer == LayerMask.NameToLayer("Highlight"))
            for (int z = 0; z < 6; z++)
                for (int x = 0; x < 6; x++)
                    for (int y = 0; y < 10; y++)
                        if (tiles[x, y, z] != null && tiles[x, y, z] == hitInfo) return new Vector3Int(x, y, z);

        return -Vector3Int.one; // -1 -1 INVALID THIS SHOUD NOT HAPPEND
    }
    public Vector3Int LookUpPinIndex(GameObject hitInfo)
    {
        if (hitInfo.layer == LayerMask.NameToLayer("Pin") || hitInfo.layer == LayerMask.NameToLayer("HoverPin") || hitInfo.layer == LayerMask.NameToLayer("HighlightPin"))
            for (int z = 0; z < 3; z++)
                for (int x = 0; x < 2; x++)
                    for (int y = 0; y < 5; y++)
                        if (pins[x, y, z] != null && pins[x, y, z] == hitInfo) return new Vector3Int(x, y, z);

        return -Vector3Int.one; // -1 -1 INVALID THIS SHOUD NOT HAPPEND
    }

    //CheckMate

    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }

    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }

    public void OnResetButton()
    {

        //ui
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        //fields reset
        currentlyDraggingBoard = null;
        currentlyDraggingPiece = null;
        avaibleMoves = new List<Vector3Int>();

        //clean up
        for (int z = 0; z < 6; z++)
            for (int x = 0; x < 6; x++)
                for (int y = 0; y < 10; y++)
                {
                    if (chessPieces[x, y, z] != null)
                        Destroy(chessPieces[x, y, z].gameObject);
                    chessPieces[x, y, z] = null;
                }

        deadBlacks.Clear();
        deadWhites.Clear();

        SpawnAllAttackingBoards();
        SpawnAllPieces();
        PositionAllBoards();
        PositionAllPieces();
        IsWhiteTurn = true;

    }

    public void OnExitButton()
    {
        Application.Quit();
    }

    // Highlighting
    private void HighlightTiles()
    {
        for (int i = 0; i < avaibleMoves.Count; i++)
        {
            tiles[avaibleMoves[i].x, avaibleMoves[i].y, avaibleMoves[i].z].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void HighlightPins()
    {
        for (int i = 0; i < avaibleMoves.Count; i++)
        {
            pins[avaibleMoves[i].x, avaibleMoves[i].y, avaibleMoves[i].z].layer = LayerMask.NameToLayer("HighlightPin");
        }
    }

    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < avaibleMoves.Count; i++)
        {
            tiles[avaibleMoves[i].x, avaibleMoves[i].y, avaibleMoves[i].z].layer = LayerMask.NameToLayer("Tile");
        }
        avaibleMoves.Clear();
    }

    private void RemoveHighlightPins()
    {
        for (int i = 0; i < avaibleMoves.Count; i++)
        {
            pins[avaibleMoves[i].x, avaibleMoves[i].y, avaibleMoves[i].z].layer = LayerMask.NameToLayer("Pin");
        }
        avaibleMoves.Clear();
    }
}
