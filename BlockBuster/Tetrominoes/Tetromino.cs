using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockBusterXaml.Tetrominoes;

[Serializable]
public abstract class Tetromino
{
    // True for all except I
    public Dictionary<MinoRotationTransition, List<Tuple<int, int>>> wallKicks = new Dictionary<MinoRotationTransition, List<Tuple<int, int>>>
    {
        { MinoRotationTransition.Rotation_0_R, new List<Tuple<int,int>>(){ new Tuple<int, int>(-1,0), new Tuple<int, int>(-1,1), new Tuple<int,int>(0,-2), new Tuple<int, int>(-1,-2) } },
        { MinoRotationTransition.Rotation_R_0, new List<Tuple<int,int>>(){ new Tuple<int, int>(1,0), new Tuple<int, int>(1,-1), new Tuple<int,int>(0,+2), new Tuple<int, int>(1,2) } },
        { MinoRotationTransition.Rotation_R_2, new List<Tuple<int,int>>(){ new Tuple<int, int>(1,0), new Tuple<int, int>(1,-1), new Tuple<int,int>(0,2), new Tuple<int, int>(1,2) } },
        { MinoRotationTransition.Rotation_2_R, new List<Tuple<int,int>>(){ new Tuple<int, int>(-1,0), new Tuple<int, int>(-1,1), new Tuple<int,int>(0,-2), new Tuple<int, int>(-1,-2) } },
        { MinoRotationTransition.Rotation_2_L, new List<Tuple<int,int>>(){ new Tuple<int, int>(1,0), new Tuple<int, int>(1,1), new Tuple<int,int>(0,-2), new Tuple<int, int>(1,-2) } },
        { MinoRotationTransition.Rotation_L_2, new List<Tuple<int,int>>(){ new Tuple<int, int>(-1,0), new Tuple<int, int>(-1,-1), new Tuple<int,int>(0,2), new Tuple<int, int>(-1,2) } },
        { MinoRotationTransition.Rotation_L_0, new List<Tuple<int,int>>(){ new Tuple<int, int>(-1,0), new Tuple<int, int>(-1,1), new Tuple<int,int>(0,2), new Tuple<int, int>(-1,2) } },
        { MinoRotationTransition.Rotation_0_L, new List<Tuple<int,int>>(){ new Tuple<int, int>(1, 0), new Tuple<int, int>(1, 1), new Tuple<int, int>(0, -2), new Tuple<int, int>(1, -2) } }
    };

    public TetrominoCellType[,] Cells { get; set; }

    public TetrominoRotation Rotation { get; } = new TetrominoRotation();

    public int RowsLenght => Cells.GetLength(0);

    public int ColumnsLenght => Cells.GetLength(1);

    public int EmptyColumnsOnLeftSideCount
    {
        get
        {
            var columnCount = 0;
            for (int column = 0; column < ColumnsLenght; column++)
            {
                var foundEmptyColumn = Enumerable.Range(0, RowsLenght)
                    .Select(x => Cells[x, column])
                    .All(c => c == TetrominoCellType.Empty);

                if (foundEmptyColumn)
                {
                    columnCount++;
                }
                else
                {
                    return columnCount;
                }
            }

            return columnCount;
        }
    }

    public int EmptyColumnsOnRightSideCount
    {
        get
        {
            var columnCount = 0;
            for (int column = ColumnsLenght - 1; column > 0; column--)
            {
                var foundEmptyColumn = Enumerable.Range(0, RowsLenght)
                    .Select(x => Cells[x, column])
                    .All(c => c == TetrominoCellType.Empty);

                if (foundEmptyColumn)
                {
                    columnCount++;
                }
                else
                {
                    return columnCount;
                }
            }

            return columnCount;
        }
    }

    public int EmptyRowsOnBottomSideCount
    {
        get
        {
            var rowCount = 0;
            for (int row = RowsLenght - 1; row > 0; row--)
            {
                var foundEmptyRow = Enumerable.Range(0, ColumnsLenght)
                    .Select(x => Cells[row, x])
                    .All(c => c == TetrominoCellType.Empty);

                if (foundEmptyRow)
                {
                    rowCount++;
                }
                else
                {
                    return rowCount;
                }
            }

            return rowCount;
        }
    }

    /// <summary>
    /// Zero based, true for most of the tetrominoes except I and O
    /// </summary>
    public Tuple<int, int> SpawnLocation = new Tuple<int, int>(0, 4);

    public int HeightLocation { get; set; }

    public int WidthLocation { get; set; }

    public Tetromino()
    {
        HeightLocation = SpawnLocation.Item1;
        WidthLocation = SpawnLocation.Item2;
    }

    public bool IsLanded = false;
    public bool IsGhost = false;

    public virtual MinoRotationTransition RotateRight()
    {
        Transpose();
        ReverseEachRow();
        return Rotation.RotateRight();
    }

    public virtual MinoRotationTransition RotateLeft()
    {
        ReverseEachRow();
        Transpose();
        return Rotation.RotateLeft();
    }

    private void ReverseEachRow()
    {
        // Iterate rows
        for (int row = 0; row < RowsLenght; row++)
        {
            // change each row
            var rowArray = new TetrominoCellType[ColumnsLenght];
            for (int rowItemIndex = 0; rowItemIndex < ColumnsLenght; rowItemIndex++)
            {
                rowArray[rowItemIndex] = Cells[row, rowItemIndex];
            }
            Array.Reverse(rowArray);

            for (int rowItemIndex = 0; rowItemIndex < ColumnsLenght; rowItemIndex++)
            {
                Cells[row, rowItemIndex] = rowArray[rowItemIndex];
            }
        }
    }

    private void Transpose()
    {
        var result = new TetrominoCellType[ColumnsLenght, RowsLenght];

        for (int i = 0; i < RowsLenght; i++)
        {
            for (int j = 0; j < ColumnsLenght; j++)
            {
                result[j, i] = Cells[i, j];
            }
        }
        Cells = result;
    }
}
