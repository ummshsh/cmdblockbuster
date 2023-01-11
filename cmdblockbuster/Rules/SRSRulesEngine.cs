using CMDblockbuster.Common;
using CMDblockbuster.Field;
using CMDblockbuster.Game;
using CMDblockbuster.InputController;
using CMDblockbuster.Tetrominoes;
using System;
using System.Threading.Tasks;

namespace CMDblockbuster.Rules
{
    /// <summary>
    /// Super Rotation System Engine
    /// </summary>
    internal class SRSRulesEngine : IRulesEngine
    {
        public Playfield playfieldInnerState; // All static elements without current tetromino
        public Playfield playfieldToDisplay;  // All static elements with current tetromino
        public readonly int ArrayLength; // Number used to specify array lengh while copying arrays

        // Event fires each time clean playfield is updated
        public event EventHandler<Playfield> PlayFieldUpdated;

        // Current tetromino and it's location
        private Tetromino currentTetromino;
        // TODO: add ghost tetromino
        private int currentTetrominoHeightLocation { get; set; }
        private int currentTetrominoWidthLocation { get; set; }

        // Time
        public TimeSpan TickRate { get; } = TimeSpan.FromMilliseconds(10);
        public TimeSpan FallRate { get; } = TimeSpan.FromSeconds(1);

        // Tetromino types
        public Type[] tetrominoes = new[] {
                typeof(TetrominoI),
                typeof(TetrominoJ),
                typeof(TetrominoL),
                typeof(TetrominoO),
                typeof(TetrominoS),
                typeof(TetrominoT),
                typeof(TetrominoZ)};

        private GameState GameState;
        private int BlankRowsCountOnLeftSide = 2;

        public SRSRulesEngine(IInputHandler inputHandler)
        {
            // Set input handler
            inputHandler.InputProvided += InputProvided;

            // Init flayfields
            this.playfieldToDisplay = new Playfield(10, 22);
            this.playfieldInnerState = new Playfield(playfieldToDisplay.Width + BlankRowsCountOnLeftSide, playfieldToDisplay.Height);

            ArrayLength = playfieldInnerState.Width * playfieldInnerState.Height;
        }

        public Task Start()
        {
            this.GameState = GameState.Running;

            return Task.Run(() =>
            {

                while (GameState == GameState.Running)
                {
                    Tick();
                    Task.Delay(TickRate).Wait();
                }
            });
        }

        public void Tick()
        {
            if (GameState == GameState.Running)
            {
                PlayFieldUpdated?.Invoke(this, this.playfieldToDisplay);
                SpawnTetromino();
                //CheckIfGameIsOver();
                UpdateFieldToDisplay();
                Gravity();
                if (Stick())
                {
                    DestroyRows();

                    //mark current tetromino as landed
                }
            }
        }

        public void Pause() => this.GameState = GameState.Paused;

        private void InputProvided(object sender, InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Left:
                    MoveLeft();
                    break;

                case InputType.Right:
                    MoveRight();
                    break;

                case InputType.RotateLeft:
                    RotateLeft();
                    break;

                case InputType.RotateRight:
                    RotateRight();
                    break;

                case InputType.Up:
                    HardFall();
                    break;

                case InputType.Down:
                    SoftFall();
                    break;

                case InputType.Pause:
                    Pause();
                    break;

                case InputType.None:
                    break;

                default:
                    throw new NotImplementedException($"Input {inputType} not implemented");
            }
        }

        private bool RotateLeft()
        {
            currentTetromino.RotateLeft();
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetrominoHeightLocation, currentTetrominoWidthLocation))
            {
                return true;
            }

            currentTetromino.RotateRight();
            return false;
        }

        private bool RotateRight()
        {
            currentTetromino.RotateRight();
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetrominoHeightLocation, currentTetrominoWidthLocation))
            {
                return true;
            }

            currentTetromino.RotateLeft();
            return false;
        }

        private bool MoveLeft()
        {
            var newLocation = currentTetrominoWidthLocation - 1;
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetrominoHeightLocation, newLocation))
            {
                currentTetrominoWidthLocation = newLocation;
                return true;
            }
            return false;
        }

        private bool MoveRight()
        {
            var newLocation = currentTetrominoWidthLocation + 1;
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetrominoHeightLocation, newLocation))
            {
                currentTetrominoWidthLocation = newLocation;
                return true;
            }
            return false;
        }


        private bool MoveDown()
        {
            if (IfTouchedFoundationOrAnotherTetrominoUnderneath())
            {
                AddCurrentTetrominoToInnerState();
                return false;
            }

            var newLocation = currentTetrominoHeightLocation + 1; // Plus one because coordinates starts from top left
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, newLocation, currentTetrominoWidthLocation))
            {
                currentTetrominoHeightLocation = newLocation;
                return true;
            }

            return false;
        }

        private void AddCurrentTetrominoToInnerState()
        {
            currentTetromino.IsLanded = true;
            currentTetromino = null;
            //throw new NotImplementedException();
        }

        private bool IfTouchedFoundationOrAnotherTetrominoUnderneath()
        {
            // Check to check if touched foundation
            if (playfieldInnerState.Height <= currentTetrominoHeightLocation + currentTetromino.CellsWithoutEmptyRowsAndColumns.GetRowsLenght())
            {
                return true;
            }

            // Check if touched another tetromino underneath
            //if (true) // TODO: to add check

            //{
            //    return true;
            //}

            return false;
        }

        private void SoftFall()
        {
            // TODO: Move tetromino down, and if it is already in contact with any block bellow, then release control of tetromino
            MoveDown();
        }

        private void HardFall()
        {
            // TODO: make tetromino fall immideately and mark it is as landed and release control of current tetromino
            throw new NotImplementedException();
        }

        private void Gravity()
        {
            //MoveDown();
        }

        private bool Stick()
        {
            return false; //TODO: retrun true if timer for current tetromino is up or it is marked as landed
        }

        private void DestroyRows()
        {
            // TODO: define destroy rows with regard to FallRate
        }

        private void UpdateFieldToDisplay()
        {
            // TODO: this guard check will not be necessary in future
            if (!CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetrominoHeightLocation, currentTetrominoWidthLocation))
            {
                return;
            }

            // Add all static elements to playfield to display
            for (int row = 0; row < playfieldToDisplay.Height; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < playfieldToDisplay.Width; rowItemIndex++)
                {
                    playfieldToDisplay[row, rowItemIndex] = playfieldInnerState[row, rowItemIndex + BlankRowsCountOnLeftSide];
                }
            }

            // Add current tetromino to playfield to display
            var currentTetromninoeRowIndex = 0;
            var currentTetromninoeRowItemIndex = 0;
            for (int row = currentTetrominoHeightLocation; row < currentTetromino.CellsWithoutEmptyRowsAndColumns.GetRowsLenght() + currentTetrominoHeightLocation; row++)
            {
                for (int rowItemIndex = currentTetrominoWidthLocation; rowItemIndex < currentTetrominoWidthLocation + currentTetromino.CellsWithoutEmptyRowsAndColumns.GetColumnsLenght(); rowItemIndex++)
                {
                    playfieldToDisplay.field[row, rowItemIndex] = currentTetromino.CellsWithoutEmptyRowsAndColumns[currentTetromninoeRowItemIndex, currentTetromninoeRowIndex];
                    currentTetromninoeRowIndex++;
                }
                currentTetromninoeRowItemIndex++;
                currentTetromninoeRowIndex = 0;
            }
        }

        public bool SpawnTetromino()
        {
            // Exit if tetromino exists already
            if (currentTetromino != null || (currentTetromino?.IsLanded ?? false))
            {
                return false;
            }

            // Create tetromnino
            var randomInt = new Random().Next(0, tetrominoes.Length - 1); // TODO: make SevenPackQueue class fot this
            currentTetromino = Activator.CreateInstance(tetrominoes[randomInt]) as Tetromino;

            // If tetromino can be spawned, then 
            if (!CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetromino.SpawnLocation.Item1, currentTetromino.SpawnLocation.Item2))
            {
                GameOver();
            }
            else
            {
                currentTetrominoHeightLocation = currentTetromino.SpawnLocation.Item1;
                currentTetrominoWidthLocation = currentTetromino.SpawnLocation.Item2;
            }
            return true;
        }

        private void GameOver()
        {
            throw new NotImplementedException("GAME OVER!");
        }

        /// <summary>
        /// TODO: make this method ignore empty cells and going out of bounds of playfield
        /// </summary>
        /// <param name="rowCoordinate">Vertical top-down</param>
        /// <param name="rowItemCoordinate">Horizontal left-right</param>
        /// <returns></returns>
        private bool CheckIfCanBePlacedOnCoordinate(Tetromino currentTetromino, int rowCoordinate, int rowItemCoordinate)
        {
            if (rowCoordinate < 0 ||
                rowCoordinate > playfieldInnerState.Height ||
                rowItemCoordinate < 0 ||
                rowItemCoordinate >= playfieldInnerState.Width - 1 - currentTetromino.CellsWithoutEmptyRowsAndColumns.GetColumnsLenght())
            {
                return false;
            }

            CellType[,] cellsWithoutEmptyRowsAndColumns = currentTetromino.CellsWithoutEmptyRowsAndColumns;
            var cuttedPlayfiedArray = new CellType[
                cellsWithoutEmptyRowsAndColumns.GetRowsLenght(),
                cellsWithoutEmptyRowsAndColumns.GetColumnsLenght()];

            // Cut tetromino sized array from playfield on location
            for (int row = rowCoordinate; row < cuttedPlayfiedArray.GetRowsLenght(); row++)
            {
                for (int rowItemIndex = rowItemCoordinate; rowItemIndex < cuttedPlayfiedArray.GetColumnsLenght(); rowItemIndex++)
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