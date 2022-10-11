using cmdblockbuster.Common;
using cmdblockbuster.Field;
using cmdblockbuster.InputController;
using cmdblockbuster.Tetrominoes;
using System;
using System.Threading;
using System.Timers;

namespace cmdblockbuster.Rules
{
    // TODO: define tickrate for gamestate updates
    // TODO: handle movement with account to palyfield state
    /// <summary>
    /// Super Rotation System Engine
    /// </summary>
    internal class SRSRulesEngine : IRulesEngine
    {
        private bool running;
        public Playfield playfieldInnerState;
        public Playfield playfieldToDisplay;

        private Tetromino currentTetromino;
        private int currentTetrominoXLocation;
        private int currentTetrominoYLocation;

        public event EventHandler<Playfield> PlayFieldUpdated;

        public SRSRulesEngine(IInputHandler inputHandler, Playfield playFieldToDisplay)
        {
            inputHandler.InputProvided += InputProvided;
            this.playfieldToDisplay = playFieldToDisplay;
            this.playfieldInnerState = new Playfield(); // TODO: just make copy of original, it may have custom size
        }

        private void InputProvided(object sender, InputType e)
        {
            throw new NotImplementedException(); // update playfield
        }

        public void Start()
        {
            this.running = true;
            var myTimer = new System.Timers.Timer();
            myTimer.Elapsed += new ElapsedEventHandler(Tick);
            myTimer.Interval = 250;
            myTimer.Start();
        }

        public void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            PlayFieldUpdated?.Invoke(this, this.playfieldToDisplay);

            while (running)
            {
                Thread.Sleep(100); // TODO: delete
                //    SpawnTetromino();
                //    ShowTetrominoOnField();
                //    Gravity();
                //    DestroyRows();
                //    once current tetromino changed which means it landed
            }
        }

        private void Gravity()
        {
            // TODO: define gravity with regard to tickrate
        }

        private void DestroyRows()
        {
            // TODO: define destroy rows with regard to tickrate
        }

        // TODO: revork methods bellow so they won't depend on playfieldToDisplay
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

        /// <summary>
        /// Todo: To replace this with 7pack spawning
        /// Todo: Don't create array each game loop
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
                //GameOver();
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
    }
}
