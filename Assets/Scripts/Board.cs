using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap boardTilemap;   // locked blocks
    public Tilemap activeTilemap;  // currently falling piece

    [Header("Tiles (assign in this order: I, O, T, S, Z, J, L)")]
    public TileBase[] tiles = new TileBase[7];

    [Header("Board size")]
    public int width = 10;
    public int height = 20;

    [Header("Timing")]
    public float gravityInterval = 0.8f;
    public float lockDelay = 0.5f;
    public float moveRepeatDelay = 0.15f; // simple DAS-ish repeat for held movement

    private TileBase[,] grid;          // logical occupancy grid
    private Piece activePiece;
    private float gravityTimer;
    private float lockTimer;
    private bool isLocking;

    private List<TetrominoType> bag = new List<TetrominoType>();
    private System.Random rng = new System.Random();

    private float moveTimer;
    private int moveDir; // -1, 0, 1

    // Define the event
    public event Action<int> OnLineBreak;

    void Awake()
    {
        grid = new TileBase[width, height];
    }

    void Start()
    {
        SpawnPiece();
    }

    void Update()
    {
        if (activePiece == null) return;

        HandleInput();
        HandleGravity();
    }

    // ---------- Spawning ----------

    void SpawnPiece()
    {
        TetrominoType type = NextFromBag();

        activePiece = new Piece
        {
            Type = type,
            Position = new Vector3Int(width / 2, height - 2, 0),
            RotationIndex = 0,
            Tile = tiles[(int)type]
        };

        gravityTimer = 0f;
        lockTimer = 0f;
        isLocking = false;

        if (!IsValidPosition(activePiece, activePiece.Position, activePiece.RotationIndex))
        {
            // New piece immediately collides -> game over.
            GameOver();
            return;
        }

        Paint(activePiece);
    }

    TetrominoType NextFromBag()
    {
        // "7-bag" randomizer: shuffle all 7 pieces, hand them out before reshuffling.
        if (bag.Count == 0)
        {
            foreach (TetrominoType t in System.Enum.GetValues(typeof(TetrominoType)))
                bag.Add(t);

            // Fisher-Yates shuffle
            for (int i = bag.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (bag[i], bag[j]) = (bag[j], bag[i]);
            }
        }

        TetrominoType next = bag[0];
        bag.RemoveAt(0);
        return next;
    }

    // ---------- Input ----------

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(Vector3Int.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryMove(Vector3Int.right);

        // simple auto-repeat while held
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveRepeatDelay)
            {
                moveTimer = 0f;
                TryMove(Input.GetKey(KeyCode.LeftArrow) ? Vector3Int.left : Vector3Int.right);
            }
        }
        else
        {
            moveTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) TryRotate(1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) SoftDrop();
        if (Input.GetKeyDown(KeyCode.Space)) HardDrop();
    }

    void TryMove(Vector3Int dir)
    {
        Vector3Int newPos = activePiece.Position + dir;
        if (IsValidPosition(activePiece, newPos, activePiece.RotationIndex))
        {
            Clear(activePiece);
            activePiece.Position = newPos;
            Paint(activePiece);
            ResetLockTimerIfGrounded();
        }
    }

    void TryRotate(int direction)
    {
        int newRotation = (activePiece.RotationIndex + direction + 4) % 4;

        // Try the rotation as-is, then a few simple wall-kick nudges.
        Vector3Int[] kicks =
        {
            Vector3Int.zero,
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.left * 2,
            Vector3Int.right * 2,
            Vector3Int.down
        };

        foreach (Vector3Int kick in kicks)
        {
            Vector3Int testPos = activePiece.Position + kick;
            if (IsValidPosition(activePiece, testPos, newRotation))
            {
                Clear(activePiece);
                activePiece.Position = testPos;
                activePiece.RotationIndex = newRotation;
                Paint(activePiece);
                ResetLockTimerIfGrounded();
                return;
            }
        }
        // No valid kick found -> rotation is rejected.
    }

    void SoftDrop()
    {
        Vector3Int newPos = activePiece.Position + Vector3Int.down;
        if (IsValidPosition(activePiece, newPos, activePiece.RotationIndex))
        {
            Clear(activePiece);
            activePiece.Position = newPos;
            Paint(activePiece);
            gravityTimer = 0f;
        }
    }

    void HardDrop()
    {
        Clear(activePiece);
        while (IsValidPosition(activePiece, activePiece.Position + Vector3Int.down, activePiece.RotationIndex))
        {
            activePiece.Position += Vector3Int.down;
        }
        Paint(activePiece);
        LockPiece();
    }

    // ---------- Gravity & locking ----------

    void HandleGravity()
    {
        bool grounded = !IsValidPosition(activePiece, activePiece.Position + Vector3Int.down, activePiece.RotationIndex);

        if (grounded)
        {
            isLocking = true;
            lockTimer += Time.deltaTime;
            if (lockTimer >= lockDelay)
            {
                LockPiece();
            }
            return;
        }

        isLocking = false;
        lockTimer = 0f;

        gravityTimer += Time.deltaTime;
        if (gravityTimer >= gravityInterval)
        {
            gravityTimer = 0f;
            Clear(activePiece);
            activePiece.Position += Vector3Int.down;
            Paint(activePiece);
        }
    }

    void ResetLockTimerIfGrounded()
    {
        // Sliding a piece while it's grounded gives it a fresh moment before locking.
        if (isLocking) lockTimer = 0f;
    }

    void LockPiece()
    {
        foreach (Vector2Int cell in activePiece.Cells)
        {
            Vector3Int pos = activePiece.Position + new Vector3Int(cell.x, cell.y, 0);
            if (pos.y >= 0 && pos.y < height && pos.x >= 0 && pos.x < width)
            {
                grid[pos.x, pos.y] = activePiece.Tile;
                boardTilemap.SetTile(pos, activePiece.Tile);
            }
        }

        Clear(activePiece);
        activePiece = null;

        ClearLines();
        SpawnPiece();
    }

    // ---------- Line clearing ----------

    void ClearLines()
    {
        int clearedCount = 0;

        for (int y = 0; y < height; y++)
        {
            if (IsRowFull(y))
            {
                clearedCount++;
                ShiftRowsDown(y);
                y--; // re-check this row index since everything above just shifted into it
            }
        }

        if (clearedCount > 0)
        {
            OnLineBreak?.Invoke(clearedCount);
            Debug.Log($"Cleared {clearedCount} line(s)");
        }
    }

    bool IsRowFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null) return false;
        }
        return true;
    }

    void ShiftRowsDown(int clearedRow)
    {
        for (int y = clearedRow; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = grid[x, y + 1];
                boardTilemap.SetTile(new Vector3Int(x, y, 0), grid[x, y]);
            }
        }

        // Clear the top row after everything has shifted down.
        for (int x = 0; x < width; x++)
        {
            grid[x, height - 1] = null;
            boardTilemap.SetTile(new Vector3Int(x, height - 1, 0), null);
        }
    }

    // ---------- Collision ----------

    bool IsValidPosition(Piece piece, Vector3Int position, int rotationIndex)
    {
        foreach (Vector2Int cell in Tetromino.Shapes[piece.Type][rotationIndex])
        {
            Vector3Int pos = position + new Vector3Int(cell.x, cell.y, 0);

            if (pos.x < 0 || pos.x >= width || pos.y < 0) return false;
            if (pos.y < height && grid[pos.x, pos.y] != null) return false;
        }
        return true;
    }

    // ---------- Rendering ----------

    void Paint(Piece piece)
    {
        foreach (Vector2Int cell in piece.Cells)
        {
            Vector3Int pos = piece.Position + new Vector3Int(cell.x, cell.y, 0);
            activeTilemap.SetTile(pos, piece.Tile);
        }
    }

    void Clear(Piece piece)
    {
        foreach (Vector2Int cell in piece.Cells)
        {
            Vector3Int pos = piece.Position + new Vector3Int(cell.x, cell.y, 0);
            activeTilemap.SetTile(pos, null);
        }
    }

    // ---------- Game over ----------

    void GameOver()
    {
        Debug.Log("Game Over");
        enabled = false; // stops Update from running; hook up a restart flow as needed
    }
}