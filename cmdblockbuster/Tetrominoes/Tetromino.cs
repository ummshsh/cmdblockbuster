using CMDblockbuster.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMDblockbuster.Tetrominoes
{
    public abstract class Tetromino
    {
        public CellType[,] Cells { get; set; }

        public CellType[,] CellsWithoutEmptyRowsAndColumns
        {
            get
            {
                // Getting rows to exclude from new array
                var rowsToDelete = new List<int>();
                for (int row = 0; row < RowsLenght; row++)
                {
                    if (Cells.GetRow(row).Count(rowItem => rowItem.Equals(CellType.Empty)) == ColumnsLenght)
                    {
                        rowsToDelete.Add(row);
                    }
                }

                // Getting columns to exclude from new array
                var columnsToDelete = new List<int>();
                for (int column = 0; column < ColumnsLenght; column++)
                {
                    if (Cells.GetColumn(column).Count(columnItem => columnItem.Equals(CellType.Empty)) == RowsLenght)
                    {
                        columnsToDelete.Add(column);
                    }
                }

                // Construction of the new array
                var arrayToReturn = new CellType[
                    RowsLenght - rowsToDelete.Count,
                    ColumnsLenght - columnsToDelete.Count];

                var newColumnsIndex = 0;
                for (int column = 0; column < ColumnsLenght; column++)
                {
                    if (columnsToDelete.Contains(column))
                    {
                        continue;
                    }
                    for (int row = 0, newRowsIndex = 0; row < RowsLenght; row++)
                    {
                        if (rowsToDelete.Contains(row))
                        {
                            continue;
                        }
                        arrayToReturn[newRowsIndex, newColumnsIndex] = Cells[row, column];
                        newRowsIndex++;
                    }
                    newColumnsIndex++;
                }

                return arrayToReturn;
            }
        }

        /// <summary>
        /// Rows
        /// </summary>
        public int RowsLenght => Cells.GetLength(0);

        /// <summary>
        /// Columns
        /// </summary>
        public int ColumnsLenght => Cells.GetLength(1);

        public int EmptyColumnsOnLeftSideCount
        {
            get
            {
                var columnCount = 0;
                for (int column = 0; column < Cells.GetColumnsLenght(); column++)
                {
                    var emptyCellsInColumn = 0;
                    for (int row = 0; row < Cells.GetRowsLenght(); row++)
                    {
                        if (Cells[row, column] == CellType.Empty)
                        {
                            emptyCellsInColumn++;
                        }
                        else
                        {
                            return columnCount;
                        }
                    }
                    if (emptyCellsInColumn == Cells.GetColumnsLenght())
                    {
                        columnCount++;
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
                for (int column = Cells.GetColumnsLenght() - 1; column > 0; column--)
                {
                    var emptyCellsInColumn = 0;
                    for (int row = 0; row < Cells.GetRowsLenght(); row++)
                    {
                        if (Cells[row, column] == CellType.Empty)
                        {
                            emptyCellsInColumn++;
                        }
                        else
                        {
                            return columnCount;
                        }
                    }
                    if (emptyCellsInColumn == Cells.GetColumnsLenght())
                    {
                        columnCount++;
                    }
                }
                return columnCount;
            }
        }

        public int EmptyRowsOnBottomSideCount
        {
            get
            {
                var columnCount = 0;
                for (int row = Cells.GetRowsLenght() - 1; row > 0; row--)
                {
                    var emptyCellsInColumn = 0;
                    for (int column = 0; column < Cells.GetColumnsLenght(); column++)
                    {
                        {
                            if (Cells[row, column] == CellType.Empty)
                            {
                                emptyCellsInColumn++;
                            }
                            else
                            {
                                return columnCount;
                            }
                        }
                        if (emptyCellsInColumn == Cells.GetColumnsLenght())
                        {
                            columnCount++;
                        }
                    }
                }
                return columnCount;
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
                var rowArray = new CellType[ColumnsLenght];
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
            var result = new CellType[ColumnsLenght, RowsLenght];

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
