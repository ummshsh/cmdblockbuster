using cmdblockbuster.Common;
using cmdblockbuster.Field;
using cmdblockbuster.Tetrominoes;
using System;
using System.Threading;

namespace cmdblockbuster
{
    class Game
    {
        /// <summary>
        /// Only static blocks <para/>
        /// Inner state of static blocks to check valid moves against
        /// </summary>
        private Playfield playfieldInnerState;

        /// <summary>
        /// Actually will be rendered <para/>
        /// </summary>
        private Playfield playfieldToDisplay;

        private bool running;
        private Tetromino currentTetromino;
        private int currentTetrominoXLocation;
        private int currentTetrominoYLocation;

        public void Start()
        {
            running = true;
            playfieldInnerState = new Playfield();
            playfieldToDisplay = new Playfield();

            Console.CursorVisible = false;

            while (running)
            {
                SpawnTetromino();
                ShowTetrominoOnField();
                ReadInput();
                Gravity();
                DestroyRows();
                // once current tetromino changed which means it landed
                Console.SetCursorPosition(0, 0);
                playfieldToDisplay.field.Print();

                Thread.Sleep(100); // Debug, just to reduce CPU usage of my laptop
            }
        }

        /// <summary>
        /// TODO: Add listener to constructor and use it here
        /// This one should:
        /// get input - DONE
        /// react to other keys - WILL DO LATER
        /// </summary>
        private void ReadInput()
        {
            if (!Console.KeyAvailable)
            {
                return;
            }

            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.J)
            {
                currentTetromino.RotateLeft();
            }
            else if (key.Key == ConsoleKey.K)
            {
                currentTetromino.RotateRight();
            }
            else if (key.Key == ConsoleKey.S)
            {
                currentTetrominoXLocation++;
            }
            else if (key.Key == ConsoleKey.A)
            {
                currentTetrominoYLocation--;
            }
            else if (key.Key == ConsoleKey.D)
            {
                currentTetrominoYLocation++;
            }
            else
            {
                Console.WriteLine("unhandled:" + key.Key);
            }
        }

        private void ShowTetrominoOnField()
        {
            if (CheckIfCanBePlacedOnCoordinate(currentTetrominoXLocation, currentTetrominoYLocation))
            {
                Array.Copy(playfieldInnerState.field, playfieldToDisplay.field, 200);
                // add tetromino to playfield inner state           
                var currentTetromninoeYIndex = 0;
                var currentTetromninoeXIndex = 0;
                for (int row = currentTetrominoXLocation; row < currentTetromino.RowsLenght + currentTetrominoXLocation; row++)
                {
                    for (int rowItemIndex = currentTetrominoYLocation; rowItemIndex < currentTetrominoYLocation + currentTetromino.ColumnsLenght; rowItemIndex++)
                    {
                        playfieldToDisplay.field[row, rowItemIndex] = currentTetromino.Cells[currentTetromninoeXIndex, currentTetromninoeYIndex];
                        currentTetromninoeYIndex++;
                    }
                    currentTetromninoeXIndex++;
                    currentTetromninoeYIndex = 0;
                }
            }
        }

        public void Stop() => running = false; //TODO: Some cleanup here

        public void Pause() => running = false;

        /// <summary>
        /// Todo: to replace this with 7pack spawning
        /// Don't create array each game loop
        /// </summary>
        public bool SpawnTetromino()
        {
            if (currentTetromino != null || (currentTetromino?.landed ?? false))
            {
                return false;
            }

            var tetrominoes = new[] {
                typeof(TetrominoI),
                typeof(TetrominoJ),
                typeof(TetrominoL),
                typeof(TetrominoO),
                typeof(TetrominoS),
                typeof(TetrominoT),
                typeof(TetrominoZ)};
            var randomInt = new Random().Next(0, tetrominoes.Length - 1);

            currentTetromino = Activator.CreateInstance(tetrominoes[randomInt]) as Tetromino;

            if (!CheckIfCanBePlacedOnCoordinate(currentTetromino.SpawnLocation.Item1, currentTetromino.SpawnLocation.Item2))
            {
                GameOver();
            }
            else
            {
                currentTetrominoXLocation = currentTetromino.SpawnLocation.Item1;
                currentTetrominoYLocation = currentTetromino.SpawnLocation.Item2;
            }
            return true;
        }

        /// <summary>
        /// TODO: make this method ignore empty cells and going out of bounds of playfield
        /// </summary>
        /// <param name="xCoordinate">Vertical top-down</param>
        /// <param name="yCoordinate">Horizontal left-right</param>
        /// <returns></returns>
        private bool CheckIfCanBePlacedOnCoordinate(int xCoordinate, int yCoordinate)
        {
            CellType[,] cellsWithoutEmptyRowsAndColumns = currentTetromino.CellsWithoutEmptyRowsAndColumns;
            var cuttedPlayfiedArray = new CellType[
                cellsWithoutEmptyRowsAndColumns.GetRowsLenght(),
                cellsWithoutEmptyRowsAndColumns.GetColumnsLenght()];

            // Cut tetromino sized array from playfield on location
            for (int row = xCoordinate; row < cuttedPlayfiedArray.GetRowsLenght(); row++)
            {
                for (int rowItemIndex = yCoordinate; rowItemIndex < cuttedPlayfiedArray.GetColumnsLenght(); rowItemIndex++)
                {
                    cuttedPlayfiedArray[row, rowItemIndex] = playfieldInnerState.field[row, rowItemIndex];
                }
            }

            // check intersections with current position of tetromino
            for (int row = 0; row < currentTetromino.CellsWithoutEmptyRowsAndColumns.GetRowsLenght(); row++)
            {
                for (int column = 0; column < currentTetromino.CellsWithoutEmptyRowsAndColumns.GetColumnsLenght(); column++)
                {
                    if (CheckIfCellsIntercect(cuttedPlayfiedArray[row, column], currentTetromino.CellsWithoutEmptyRowsAndColumns[row, column]))
                    {
                        return false; // fail on first found intercestion
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// This one will try kick tetromino according to the rules and use <seealso cref="CheckIfCanBePlacedOnCoordinate(int, int)"/>
        /// </summary>
        private bool CheckIfCanBePlacedOnCoordinateWithKick(int xCoordinate, int yCoordinate)
        {
            throw new NotImplementedException("TODO");
        }

        /// <summary>
        /// Cells intercect if: <para/>
        /// if current cell is empty and tetrominoe's cell is empty or not empty <para/>
        /// if current cell in not empty and tetrominoe's cell is empty <para/>
        /// </summary>
        private bool CheckIfCellsIntercect(CellType fieldCell, CellType tetrominoeCell)
        {
            // if fieldCell empty and tetrominoe cell empty
            // if fieldCell empty and tetrominoe cell not
            if ((int)fieldCell < 1)
            {
                return false;
            }
            // if field cell not empty and tetrominoe cell is empty
            else if ((int)fieldCell > 0 && (int)tetrominoeCell < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void GameOver()
        {
            // TODO
        }

        private void Gravity()
        {
            // TODO
        }

        private void DestroyRows()
        {
            // TODO
        }
    }
}
