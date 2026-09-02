using UnityEngine;
using UnityEngine.Tilemaps;

public enum TetrominoType { I, O, T, S, Z, J, L }

// Holds the current state of the falling piece: its type, position, rotation, and tile.
public class Piece
{
    public TetrominoType Type;
    public Vector3Int Position; // pivot position on the grid
    public int RotationIndex;   // 0-3
    public TileBase Tile;

    public Vector2Int[] Cells => Tetromino.Shapes[Type][RotationIndex];
}

// Static shape data: for each piece type, 4 rotation states, each a list of
// 4 cell offsets from the pivot. Not full SRS, but non-overlapping and solid
// for a hobby build. Wall kicks are handled separately in Board.cs.
public static class Tetromino
{
    public static readonly System.Collections.Generic.Dictionary<TetrominoType, Vector2Int[][]> Shapes =
        new System.Collections.Generic.Dictionary<TetrominoType, Vector2Int[][]>
    {
        { TetrominoType.I, new[] {
            new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) },
            new[] { new Vector2Int(1,1),  new Vector2Int(1,0), new Vector2Int(1,-1),new Vector2Int(1,-2) },
            new[] { new Vector2Int(-1,-1),new Vector2Int(0,-1),new Vector2Int(1,-1),new Vector2Int(2,-1) },
            new[] { new Vector2Int(0,1),  new Vector2Int(0,0), new Vector2Int(0,-1),new Vector2Int(0,-2) },
        }},
        { TetrominoType.O, new[] {
            new[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },
            new[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },
            new[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },
            new[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },
        }},
        { TetrominoType.T, new[] {
            new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1) },
            new[] { new Vector2Int(0,1),  new Vector2Int(0,0), new Vector2Int(0,-1),new Vector2Int(1,0) },
            new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,-1)},
            new[] { new Vector2Int(0,1),  new Vector2Int(0,0), new Vector2Int(0,-1),new Vector2Int(-1,0)},
        }},
        { TetrominoType.S, new[] {
            new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1) },
            new[] { new Vector2Int(0,1),  new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,-1) },
            new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1) },
            new[] { new Vector2Int(0,1),  new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,-1) },
        }},
        { TetrominoType.Z, new[] {
            new[] { new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(0,0), new Vector2Int(1,0) },
            new[] { new Vector2Int(1,1),  new Vector2Int(1,0), new Vector2Int(0,0), new Vector2Int(0,-1) },
            new[] { new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(0,0), new Vector2Int(1,0) },
            new[] { new Vector2Int(1,1),  new Vector2Int(1,0), new Vector2Int(0,0), new Vector2Int(0,-1) },
        }},
        { TetrominoType.J, new[] {
            new[] { new Vector2Int(-1,1), new Vector2Int(-1,0),new Vector2Int(0,0), new Vector2Int(1,0) },
            new[] { new Vector2Int(0,1),  new Vector2Int(1,1), new Vector2Int(0,0), new Vector2Int(0,-1)},
            new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,-1)},
            new[] { new Vector2Int(0,1),  new Vector2Int(0,0), new Vector2Int(0,-1),new Vector2Int(-1,-1)},
        }},
        { TetrominoType.L, new[] {
            new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,1) },
            new[] { new Vector2Int(0,1),  new Vector2Int(0,0), new Vector2Int(0,-1),new Vector2Int(1,-1)},
            new[] { new Vector2Int(-1,-1),new Vector2Int(-1,0),new Vector2Int(0,0), new Vector2Int(1,0) },
            new[] { new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(0,0), new Vector2Int(0,-1)},
        }},
    };
}