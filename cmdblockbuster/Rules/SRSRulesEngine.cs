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
        private int BlankRowsCountOnLeftSide = 0;

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
                DestroyRows();
                //mark current tetromino as landed
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
            if (currentTetromino.IsLanded)
            {
                return false;
            }
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
            if (currentTetromino.IsLanded)
            {
                return false;
            }
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
            if (currentTetromino.IsLanded)
            {
                return false;
            }
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
            if (currentTetromino.IsLanded)
            {
                return false;
            }
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
            if (currentTetromino.IsLanded)
            {
                return false;
            }

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

            for (int row = currentTetrominoHeightLocation; row < currentTetrominoHeightLocation + currentTetromino.RowsLenght; row++)
            {
                for (int rowItemIndex = currentTetrominoWidthLocation; rowItemIndex < currentTetrominoWidthLocation + currentTetromino.ColumnsLenght; rowItemIndex++)
                {
                    var cellToPaste = currentTetromino.Cells[row - currentTetrominoHeightLocation, rowItemIndex - currentTetrominoWidthLocation];
                    if (cellToPaste != CellType.Empty)
                    {
                        playfieldInnerState.field[row, rowItemIndex + BlankRowsCountOnLeftSide] = cellToPaste;
                    }
                }
            }
        }

        private bool IfTouchedFoundationOrAnotherTetrominoUnderneath()
        {
            // Check to check if touched foundation or to check if touched another tetromino underneath
            if (playfieldInnerState.Height <= currentTetrominoHeightLocation + currentTetromino.CellsWithoutEmptyRowsAndColumns.GetRowsLenght() ||
                !CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetrominoHeightLocation + 1, currentTetrominoWidthLocation))
            {
                return true;
            }

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
            for (int row = playfieldInnerState.Height - 1; row >= 0; row--)
            {
                if (CheckRowFilled(row))
                {
                    DestroyRow(row);
                    MoveTopRowsToRow(row);
                }
            }
        }

        private void MoveTopRowsToRow(int rowToMoveTo)
        {
            for (int row = rowToMoveTo - 1; row > 0; row--)
            {
                for (int rowItem = BlankRowsCountOnLeftSide; rowItem < playfieldInnerState.Width; rowItem++)
                {
                    if (row == 0)
                    {
                        return;
                    }

                    playfieldInnerState[row + 1, rowItem] = playfieldInnerState[row, rowItem];
                }
            }

        }

        private void DestroyRow(int row)
        {
            for (int rowItem = BlankRowsCountOnLeftSide; rowItem < playfieldInnerState.Width; rowItem++)
            {
                playfieldInnerState[row, rowItem] = CellType.Empty;
            }
        }

        private bool CheckRowFilled(int row)
        {
            for (int rowItem = BlankRowsCountOnLeftSide; rowItem < playfieldInnerState.Width; rowItem++)
            {
                if (playfieldInnerState[row, rowItem] == CellType.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateFieldToDisplay()
        {
            // Add all static elements to playfield to display
            for (int row = 0; row < playfieldToDisplay.Height; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < playfieldToDisplay.Width; rowItemIndex++)
                {
                    playfieldToDisplay[row, rowItemIndex] = playfieldInnerState[row, rowItemIndex + BlankRowsCountOnLeftSide];
                }
            }

            // Add current tetromino to playfield to display
            for (int row = 0; row < currentTetromino.RowsLenght; row++)
            {
                for (int rowItem = 0; rowItem < currentTetromino.ColumnsLenght; rowItem++)
                {
                    var cellToPaste = currentTetromino.Cells[row, rowItem];
                    if (cellToPaste != CellType.Empty)
                    {
                        playfieldToDisplay.field[row + currentTetrominoHeightLocation, rowItem + currentTetrominoWidthLocation] = cellToPaste;
                    }
                }
            }
        }

        public bool SpawnTetromino()
        {
            // Exit if tetromino exists already
            if (currentTetromino != null && !(currentTetromino?.IsLanded ?? false))
            {
                return false;
            }

            // Create tetromnino
            var randomInt = new Random().Next(0, tetrominoes.Length - 1); // TODO: make SevenPackQueue class fot this
            currentTetromino = Activator.CreateInstance(tetrominoes[0]) as Tetromino;

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
            GameState = GameState.GameOver;
            throw new NotImplementedException("GAME OVER!");
        }

        /// <summary>
        /// TODO: make this method ignore empty cells and going out of bounds of playfield
        /// </summary>
        /// <param name="rowCoordinate">Vertical top-down</param>
        /// <param name="rowItemCoordinate">Horizontal left-right</param>
        /// <returns></returns>
        private bool CheckIfCanBePlacedOnCoordinate(Tetromino tetromino, int rowCoordinate, int rowItemCoordinate)
        {
            //// Check that coordinate is not outside playfield boundaries
            if (rowCoordinate < 0 ||
                rowCoordinate > playfieldInnerState.Height)
            {
                return false;
            }

            // Check that tetromino fits on playfield on coordinate
            for (int rowIndex = rowCoordinate; rowIndex < rowCoordinate + tetromino.RowsLenght; rowIndex++)
            {
                for (int rowItemIndex = rowItemCoordinate; rowItemIndex < rowItemCoordinate + tetromino.ColumnsLenght; rowItemIndex++)
                {
                    if (rowItemIndex >= 0 && rowItemIndex < playfieldInnerState.Width && rowCoordinate + tetromino.RowsLenght < playfieldInnerState.Height)
                    {
                        if (CheckIfCellsIntercect(currentTetromino.Cells[rowIndex - rowCoordinate, rowItemIndex - rowItemCoordinate], playfieldInnerState.field[rowIndex, rowItemIndex + BlankRowsCountOnLeftSide]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // Check intersection ignoring empty columns and rows that are outside of the bounds of array
                        if (rowItemIndex < 0)
                        {
                            // left
                            if (currentTetromino.EmptyColumnsOnLeftSideCount < rowIndex)
                            {
                                return true;
                            }
                        }
                        else if (rowItemIndex + tetromino.ColumnsLenght > playfieldInnerState.Width - 1)
                        {
                            // right
                            if (rowItemIndex + tetromino.ColumnsLenght - currentTetromino.EmptyColumnsOnRightSideCount < playfieldInnerState.Width)
                            {
                                return true;
                            }
                        }
                        else if (rowIndex + tetromino.RowsLenght -1 + tetromino.EmptyRowsOnBottomSideCount > playfieldInnerState.Height)
                        {
                            // bottom
                            if (rowIndex + tetromino.RowsLenght - currentTetromino.EmptyRowsOnBottomSideCount < playfieldInnerState.Height)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }
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