using cmdblockbuster.Field;
using cmdblockbuster.Tetrominoes;
using System;

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
        private Tuple<int, int> currentTetrominoLocation;

        public void Start()
        {
            running = true;
            playfieldInnerState = new Playfield();
            playfieldToDisplay = new Playfield();

            while (running)
            {
                SpawnTetromino();
                ShowTetrominoOnField();
                ReadInput();
                DestroyRows();
                // once current tetromino changed which means it landed
            }
        }

        private void DestroyRows()
        {
            // TODO
        }

        /// <summary>
        /// TODO: Add listener to constructor and use it
        /// </summary>
        private void ReadInput()
        {
            if (!Console.KeyAvailable)
            {
                return;
            }

            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.UpArrow)
            {
                Console.WriteLine("Up");
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                Console.WriteLine("Down");
            }
            else if (key.Key == ConsoleKey.LeftArrow)
            {
                Console.WriteLine("Left");
            }
            else if (key.Key == ConsoleKey.RightArrow)
            {
                Console.WriteLine("Right");
            }
            else
            {
                Console.WriteLine("Unhandled:" + key.Key);
            }
        }

        private void ShowTetrominoOnField()
        {
            if (CheckIfCanBePlacedOnCoordinate(currentTetrominoLocation))
            {
                // add tetromino to playfield inner state
                var currentTetromninoeYIndex = 0;
                var currentTetromninoeXIndex = 0;
                for (int row = currentTetrominoLocation.Item1; row < currentTetromino.XDimLenght - row; row++)
                {
                    for (int rowItemIndex = currentTetrominoLocation.Item2; rowItemIndex < currentTetrominoLocation.Item2 + currentTetromino.YDimLenght; rowItemIndex++)
                    {
                        playfieldToDisplay.field[row, rowItemIndex] = currentTetromino.Cells[currentTetromninoeXIndex, currentTetromninoeYIndex];
                        currentTetromninoeYIndex++;
                    }
                    currentTetromninoeXIndex++;
                    currentTetromninoeYIndex = 0;
                }
            }
        }

        public void Stop() => running = false; //TODO: maybe do some cleanup here

        /// <summary>
        /// Todo: to replace this with 7pack spawning
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

            if (!CheckIfCanBePlacedOnCoordinate(currentTetromino.SpawnLocation))
            {
                GameOver();
            }
            else
            {
                currentTetrominoLocation = currentTetromino.SpawnLocation;
            }
            return true;
        }

        private bool CheckIfCanBePlacedOnCoordinate(Tuple<int, int> coordinate)
        {
            // Cut tetromino sized array from playfield on location
            var cuttedPlayfiedArray = new CellType[currentTetromino.XDimLenght, currentTetromino.YDimLenght];
            for (int row = coordinate.Item1; row < currentTetromino.XDimLenght; row++)
            {
                for (int rowItemIndex = coordinate.Item2; rowItemIndex < currentTetromino.YDimLenght; rowItemIndex++)
                {
                    cuttedPlayfiedArray[row, rowItemIndex] = playfieldInnerState.field[row, rowItemIndex];
                }
            }

            // check intersections with current position of tetromino
            for (int row = 0; row < currentTetromino.XDimLenght; row++)
            {
                for (int column = 0; column < currentTetromino.YDimLenght; column++)
                {
                    if (CheckIfCellsIntercect(cuttedPlayfiedArray[row, column], currentTetromino.Cells[row, column]))
                    {
                        return false; // fail on first found intercestion
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Cells intercect if: <para/>
        /// if current cell is empty and tetrominoe's cell is empty or not empty <para/>
        /// if current cell in not empty and tetrominoe's cell is empty <para/>
        /// </summary>
        /// <param name="fieldCell"></param>
        /// <param name="tetrominoeCell"></param>
        /// <returns></returns>
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
            // Handle here this case
        }
    }
}
