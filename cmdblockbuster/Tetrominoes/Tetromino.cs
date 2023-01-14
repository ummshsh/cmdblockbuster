using System;
using System.Linq;
using cmdblockbuster.Tetrominoes;

namespace CMDblockbuster.Tetrominoes
{
    public abstract class Tetromino
    {
        public TetrominoCellType[,] Cells { get; set; }

        public int RowsLenght => Cells.GetLength(0);

        public int ColumnsLenght => Cells.GetLength(1);

        public int EmptyColumnsOnLeftSideCount
        {
            get
            {
                var columnCount = 0;
                for (int column = 0; column < Cells.GetLength(1); column++)
                {
                    var foundEmptyColumn = Enumerable.Range(0, Cells.GetLength(0))
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
                for (int column = Cells.GetLength(1) - 1; column > 0; column--)
                {
                    var foundEmptyColumn = Enumerable.Range(0, Cells.GetLength(0))
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
                for (int row = Cells.GetLength(0) - 1; row > 0; row--)
                {
                    var foundEmptyRow = Enumerable.Range(0, Cells.GetLength(1))
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

        public bool IsLanded = false;
        public bool IsGhost = false;

        public virtual void RotateRight()
        {
            Transpose();
            ReverseEachRow();
        }

        public virtual void RotateLeft()
        {
            ReverseEachRow();
            Transpose();
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
}
