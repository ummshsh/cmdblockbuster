using cmdblockbuster.Tetrominoes;
using CMDblockbuster.Field;
using CMDblockbuster.InputController;
using CMDblockbuster.Tetrominoes;
using System;
using System.Threading.Tasks;

namespace cmdblockbuster.Game
{
    /// <summary>
    /// Super Rotation System Engine
    /// </summary>
    internal class SRSRulesEngine : IRulesEngine
    {
        public Playfield playfieldInnerState; // All static elements without current tetromino
        public Playfield playfieldToDisplay;  // All static elements with current tetromino

        public event EventHandler<Playfield> PlayFieldUpdated; // Event fires each time clean playfield is updated

        private Tetromino currentTetromino;
        private int CurrentTetrominoHeightLocation { get; set; }
        private int CurrentTetrominoWidthLocation { get; set; }

        //private readonly TetrominoQueue queue = new TetrominoQueue();
        private readonly GameState gameState = new GameState();

        public SRSRulesEngine(IInputHandler inputHandler)
        {
            inputHandler.InputProvided += InputProvided; // Set input handler

            playfieldToDisplay = new Playfield(10, 22);
            playfieldInnerState = new Playfield(playfieldToDisplay.Width, playfieldToDisplay.Height);
        }

        #region StateManagement

        public Task Start()
        {
            gameState.State = State.Running;

            return Task.Run(() =>
            {
                while (gameState.State == State.Running)
                {
                    Tick();
                    Task.Delay(Variables.TickRate).Wait();
                }
            });
        }

        public void Tick()
        {
            if (gameState.State == State.Running)
            {
                if ((gameState.LastTimePlayfieldWasUpdated + Variables.RenderUpdateRate) < DateTime.Now)
                {
                    PlayFieldUpdated?.Invoke(this, playfieldToDisplay);
                    gameState.LastTimePlayfieldWasUpdated = DateTime.Now;
                }

                SpawnTetromino();
                UpdateFieldToDisplay(currentTetromino, CurrentTetrominoHeightLocation, CurrentTetrominoWidthLocation);
                Gravity();
                DestroyRows();
            }
        }

        private void GameOver()
        {
            gameState.State = State.GameOver;
            throw new NotImplementedException("GAME OVER!");
        }

        public void Pause() => gameState.State = State.Paused;

        #endregion

        #region HandleMovement

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

                case InputType.HardDrop:
                    HardFall();
                    break;

                case InputType.SoftDrop:
                    SoftFall();
                    break;

                case InputType.Hold:
                    if (gameState.CanUseHold)
                    {
                        var tetrominoToSpawnFromHold = gameState.Queue.GetTetrominoFromHold(currentTetromino.GetType());
                        currentTetromino = null;
                        SpawnTetromino(tetrominoToSpawnFromHold);
                    }
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
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, CurrentTetrominoHeightLocation, CurrentTetrominoWidthLocation))
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
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, CurrentTetrominoHeightLocation, CurrentTetrominoWidthLocation))
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
            var newLocation = CurrentTetrominoWidthLocation - 1;
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, CurrentTetrominoHeightLocation, newLocation))
            {
                CurrentTetrominoWidthLocation = newLocation;
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
            var newLocation = CurrentTetrominoWidthLocation + 1;
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, CurrentTetrominoHeightLocation, newLocation))
            {
                CurrentTetrominoWidthLocation = newLocation;
                return true;
            }
            return false;
        }

        private bool MoveDown(bool hardDrop = false)
        {
            if (currentTetromino.IsLanded)
            {
                return false;
            }

            if (IfTouchedFoundationOrAnotherTetrominoUnderneath() &&
                (hardDrop ? true : gameState.LastTimeTetrominoMovedDown.Add(Variables.LockDelayTimeout) < DateTime.Now))
            {
                AddCurrentTetrominoToInnerState();
                return false;
            }

            var newLocation = CurrentTetrominoHeightLocation + 1; // Plus one because coordinates starts from top left
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, newLocation, CurrentTetrominoWidthLocation))
            {
                CurrentTetrominoHeightLocation = newLocation;
                return true;
            }

            return false;
        }

        private void SoftFall() => MoveDown();

        private void HardFall()
        {
            while (MoveDown(true))
            {
                // Empty
            }
        }

        private void Gravity()
        {
            if (DateTime.Now > gameState.LastTimeTetrominoMovedDown + gameState.CurrentPerRowInterval)
            {
                MoveDown();
                gameState.LastTimeTetrominoMovedDown = DateTime.Now;
            }
        }

        #endregion

        #region Playfied Management And Helpers

        private void AddCurrentTetrominoToInnerState()
        {
            currentTetromino.IsLanded = true;

            for (int row = CurrentTetrominoHeightLocation; row < CurrentTetrominoHeightLocation + currentTetromino.RowsLenght; row++)
            {
                for (int rowItemIndex = CurrentTetrominoWidthLocation; rowItemIndex < CurrentTetrominoWidthLocation + currentTetromino.ColumnsLenght; rowItemIndex++)
                {
                    var cellToPaste = currentTetromino.Cells[row - CurrentTetrominoHeightLocation, rowItemIndex - CurrentTetrominoWidthLocation];
                    if (cellToPaste != TetrominoCellType.Empty)
                    {
                        playfieldInnerState.field[row, rowItemIndex] = cellToPaste;
                    }
                }
            }
        }

        private bool IfTouchedFoundationOrAnotherTetrominoUnderneath()
        {
            // Check to check if touched foundation or check if touched another tetromino underneath
            if (playfieldInnerState.Height <= CurrentTetrominoHeightLocation + currentTetromino.RowsLenght - currentTetromino.EmptyRowsOnBottomSideCount ||
                !CheckIfCanBePlacedOnCoordinate(currentTetromino, CurrentTetrominoHeightLocation + 1, CurrentTetrominoWidthLocation))
            {
                return true;
            }

            return false;
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
                for (int rowItem = 0; rowItem < playfieldInnerState.Width; rowItem++)
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
            for (int rowItem = 0; rowItem < playfieldInnerState.Width; rowItem++)
            {
                playfieldInnerState[row, rowItem] = TetrominoCellType.Empty;
            }
        }

        private bool CheckRowFilled(int row)
        {
            for (int rowItem = 0; rowItem < playfieldInnerState.Width; rowItem++)
            {
                if (playfieldInnerState[row, rowItem] == TetrominoCellType.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateFieldToDisplay(Tetromino tetromino, int currentTetrominoHeight, int currentTetrominoWidth)
        {
            // Add all static elements to playfield to display
            for (int row = 0; row < playfieldToDisplay.Height; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < playfieldToDisplay.Width; rowItemIndex++)
                {
                    playfieldToDisplay[row, rowItemIndex] = playfieldInnerState[row, rowItemIndex + 0];
                }
            }

            // Add current tetromino to playfield to display
            for (int row = 0; row < tetromino.RowsLenght; row++)
            {
                for (int rowItem = 0; rowItem < tetromino.ColumnsLenght; rowItem++)
                {
                    var cellToPaste = tetromino.Cells[row, rowItem];
                    if (cellToPaste != TetrominoCellType.Empty)
                    {
                        if (rowItem + currentTetrominoWidth < 0 || rowItem + currentTetrominoWidth > playfieldInnerState.Width)
                        {
                            continue;
                        }
                        playfieldToDisplay.field[row + currentTetrominoHeight, rowItem + currentTetrominoWidth] = cellToPaste;
                    }
                }
            }
        }

        public bool SpawnTetromino(Tetromino tetrominoToSpawnFromHold = null)
        {
            if (tetrominoToSpawnFromHold == null)
            {
                // Return  if tetromino exists already
                if (currentTetromino != null && !(currentTetromino?.IsLanded ?? false))
                {
                    return false;
                }
                currentTetromino = gameState.Queue.GetTetrominoFromQueue();
                gameState.CanUseHold = true;
            }
            else
            {
                gameState.CanUseHold = false;
                currentTetromino = tetrominoToSpawnFromHold;
            }

            lock (currentTetromino)
            {
                // Check if can be spawned
                if (!CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetromino.SpawnLocation.Item1, currentTetromino.SpawnLocation.Item2))
                {
                    GameOver();
                }
                else
                {
                    CurrentTetrominoHeightLocation = currentTetromino.SpawnLocation.Item1;
                    CurrentTetrominoWidthLocation = currentTetromino.SpawnLocation.Item2;
                }
            }

            return true;
        }

        /// <param name="rowCoordinate">Vertical top-down</param>
        /// <param name="rowItemCoordinate">Horizontal left-right</param>
        /// <returns></returns>
        private bool CheckIfCanBePlacedOnCoordinate(Tetromino tetromino, int rowCoordinate, int rowItemCoordinate)
        {
            //// Check that coordinate is not outside playfield boundaries
            if (rowCoordinate < 0 ||
                rowCoordinate > playfieldInnerState.Height ||
                rowItemCoordinate + tetromino.EmptyColumnsOnLeftSideCount < 0 ||
                rowItemCoordinate + tetromino.ColumnsLenght - tetromino.EmptyColumnsOnRightSideCount > playfieldInnerState.Width)
            {
                return false;
            }

            // Check that tetromino fits on playfield on coordinate
            for (int rowIndex = rowCoordinate; rowIndex < rowCoordinate + tetromino.RowsLenght - tetromino.EmptyRowsOnBottomSideCount; rowIndex++)
            {
                for (int rowItemIndex = rowItemCoordinate; rowItemIndex < rowItemCoordinate + tetromino.ColumnsLenght; rowItemIndex++)
                {
                    if (rowItemIndex >= 0 && rowItemIndex < playfieldInnerState.Width && rowCoordinate + tetromino.RowsLenght - 1 - tetromino.EmptyRowsOnBottomSideCount < playfieldInnerState.Height)
                    {
                        if (CheckIfCellsIntersect(
                            currentTetromino.Cells[rowIndex - rowCoordinate, rowItemIndex - rowItemCoordinate],
                            playfieldInnerState.field[rowIndex, rowItemIndex]))
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
                            if (0 - currentTetromino.EmptyColumnsOnLeftSideCount < rowItemIndex)
                            {
                                if (CheckIfCellsIntersect(
                                    currentTetromino.Cells[rowIndex - rowCoordinate, rowItemIndex - rowItemCoordinate],
                                    playfieldInnerState.field[rowIndex, rowItemIndex + currentTetromino.EmptyColumnsOnLeftSideCount]))
                                {
                                    return false;
                                }
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
                        else if (rowIndex + tetromino.RowsLenght - 1 - tetromino.EmptyRowsOnBottomSideCount <= playfieldInnerState.Height)
                        {
                            // bottom
                            if (rowIndex + tetromino.RowsLenght - 1 - currentTetromino.EmptyRowsOnBottomSideCount < playfieldInnerState.Height)
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

        private bool CheckIfCellsIntersect(TetrominoCellType fieldCell, TetrominoCellType tetrominoeCell) =>
            (int)fieldCell > 0 && (int)tetrominoeCell > 0;

        #endregion
    }
}