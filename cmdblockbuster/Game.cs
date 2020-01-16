using cmdblockbuster.Field;
using cmdblockbuster.Tetrominoes;
using System;

namespace cmdblockbuster
{
    class Game
    {
        private Playfield playfield;
        private bool running;
        private Tetromino currentTetromino;
        private Tuple<int, int> currentTetrominoLocation;

        public void Start()
        {
            running = true;
            playfield = new Playfield();

            // gameloop
            while (running)
            {
                // Spawn Tetromino
                SpawnTetromino();
                RenderTetromino(currentTetromino);
                // TODO: who should track Tetromino position? looks like field

                // listen to inputs -> move tetromino
                // once current tetromino changed which means it landed -> check field for rows to destroy
            }
        }

        private void RenderTetromino(Tetromino currentTetromino)
        {
            throw new NotImplementedException();
        }

        public void Stop() => running = false; //TODO: maybe do some cleanup here

        /// <summary>
        /// Todo: to replace this with 7pack spawning
        /// </summary>
        public void SpawnTetromino()
        {
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

            currentTetrominoLocation = currentTetromino.SpawnLocation;
        }

        private bool CheckIfCanBePlacedOnCoordinate(Tuple<int, int> location)
        {
            // Cut tetromino sized array from playfield on location
            var cuttedPlayfiedArray = new CellType[currentTetromino.YDimLenght, currentTetromino.XDimLenght];
            for (int row = location.Item2; row < currentTetromino.YDimLenght; row++)
            {
                for (int rowItemIndex = currentTetrominoLocation.Item1; rowItemIndex < currentTetromino.XDimLenght; rowItemIndex++)
                {
                    cuttedPlayfiedArray[row, rowItemIndex] = playfield.field[row, rowItemIndex];
                }
            }

            // check intersections with current position of tetromino
            for (int row = 0; row < currentTetromino.YDimLenght; row++)
            {
                for (int column = 0; column < currentTetromino.XDimLenght; column++)
                {
                    if (CheckIfCellsNotIntercect(cuttedPlayfiedArray[row, column], currentTetromino.Cells[row, column]))
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
        private bool CheckIfCellsNotIntercect(CellType fieldCell, CellType tetrominoeCell)
        {
            return (int)fieldCell < 1 || (int)tetrominoeCell < 1;
        }

        private void GameOver()
        {
            // Handle here this case
        }
    }
}
