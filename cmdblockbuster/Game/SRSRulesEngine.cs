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
        private InnerPlayfield playfieldInnerState; // All static elements without current tetromino
        public InnerPlayfield playfieldToDisplay;  // All static elements with current tetromino

        public event EventHandler<InnerPlayfield> PlayFieldUpdated; // Event fires each time clean playfield is updated

        private Tetromino currentTetromino = null;
        private Tetromino currentTetrominoGhost = null;

        private readonly GameState gameState;

        public SRSRulesEngine(IInputHandler inputHandler)
        {
            inputHandler.InputProvided += InputProvided; // Set input handler

            playfieldToDisplay = new InnerPlayfield(10, 22);
            playfieldInnerState = new InnerPlayfield(playfieldToDisplay.Width, playfieldToDisplay.Height);

            gameState = new GameState();
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
                SpawnTetromino();
                Gravity();
                UpdateGhost();
                DestroyRows();

                if ((gameState.LastTimePlayfieldWasUpdated + Variables.RenderUpdateRate) < DateTime.Now)
                {
                    UpdateFieldToDisplay(currentTetromino, currentTetrominoGhost);
                    PlayFieldUpdated?.Invoke(this, playfieldToDisplay);
                    gameState.LastTimePlayfieldWasUpdated = DateTime.Now;
                }
            }
        }

        private void UpdateGhost()
        {
            currentTetrominoGhost = currentTetromino.Clone();

            var newLocation = currentTetromino.HeightLocation; // Plus one because coordinates starts from top left
            while (CheckIfCanBePlacedOnCoordinate(currentTetrominoGhost, newLocation + 1, currentTetrominoGhost.WidthLocation))
            {
                newLocation++;
            }

            currentTetrominoGhost.HeightLocation = newLocation;
            currentTetrominoGhost.WidthLocation = currentTetromino.WidthLocation;
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
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetromino.HeightLocation, currentTetromino.WidthLocation))
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
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetromino.HeightLocation, currentTetromino.WidthLocation))
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
            var newLocation = currentTetromino.WidthLocation - 1;
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetromino.HeightLocation, newLocation))
            {
                currentTetromino.WidthLocation = newLocation;
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
            var newLocation = currentTetromino.WidthLocation + 1;
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetromino.HeightLocation, newLocation))
            {
                currentTetromino.WidthLocation = newLocation;
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

            if (IfTouchedFoundationOrAnotherTetrominoUnderneath(currentTetromino) &&
                (hardDrop ? true : gameState.LastTimeTetrominoMovedDown.Add(Variables.LockDelayTimeout) < DateTime.Now))
            {
                AddCurrentTetrominoToInnerState();
                return false;
            }

            var newLocation = currentTetromino.HeightLocation + 1; // Plus one because coordinates starts from top left
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, newLocation, currentTetromino.WidthLocation))
            {
                currentTetromino.HeightLocation = newLocation;
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

            for (int row = currentTetromino.HeightLocation; row < currentTetromino.HeightLocation + currentTetromino.RowsLenght; row++)
            {
                for (int rowItemIndex = currentTetromino.WidthLocation; rowItemIndex < currentTetromino.WidthLocation + currentTetromino.ColumnsLenght; rowItemIndex++)
                {
                    var cellToPaste = currentTetromino.Cells[row - currentTetromino.HeightLocation, rowItemIndex - currentTetromino.WidthLocation];
                    if (cellToPaste != TetrominoCellType.Empty)
                    {
                        playfieldInnerState.Cells[row, rowItemIndex] = cellToPaste;
                    }
                }
            }
        }

        private bool IfTouchedFoundationOrAnotherTetrominoUnderneath(Tetromino tetromino)
        {
            // Check to check if touched foundation or check if touched another tetromino underneath
            if (playfieldInnerState.Height <= tetromino.HeightLocation + tetromino.RowsLenght - tetromino.EmptyRowsOnBottomSideCount ||
                !CheckIfCanBePlacedOnCoordinate(tetromino, tetromino.HeightLocation + 1, tetromino.WidthLocation))
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

        private void UpdateFieldToDisplay(Tetromino tetromino, Tetromino tetrominoGhost)
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
                    if (cellToPaste == TetrominoCellType.Empty ||
                        rowItem + tetromino.WidthLocation < 0 ||
                        rowItem + tetromino.WidthLocation > playfieldToDisplay.Width)
                    {
                        continue;
                    }

                    playfieldToDisplay.Cells[row + tetromino.HeightLocation, rowItem + tetromino.WidthLocation] = cellToPaste;
                }
            }

            // Add ghost as well
            for (int row = 0; row < tetrominoGhost.RowsLenght; row++)
            {
                for (int rowItem = 0; rowItem < tetrominoGhost.ColumnsLenght; rowItem++)
                {
                    var cellToPaste = tetrominoGhost.Cells[row, rowItem];
                    if (cellToPaste == TetrominoCellType.Empty ||
                       rowItem + tetrominoGhost.WidthLocation < 0 ||
                       rowItem + tetrominoGhost.WidthLocation > playfieldToDisplay.Width)
                    {
                        continue;
                    }


                    int height = row + tetrominoGhost.HeightLocation;
                    if (height > playfieldInnerState.Height - 1)
                    {
                        continue;
                    }
                    playfieldToDisplay.Cells[height, rowItem + tetrominoGhost.WidthLocation] = cellToPaste;
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

            if (!CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetromino.SpawnLocation.Item1, currentTetromino.SpawnLocation.Item2))
            {
                GameOver();
            }

            return true;
        }

        /// <param name="rowCoordinate">Vertical top-down</param>
        /// <param name="rowItemCoordinate">Horizontal left-right</param>
        /// <returns></returns>
        private bool CheckIfCanBePlacedOnCoordinate(Tetromino tetromino, int rowCoordinate, int rowItemCoordinate)
        {
            bool youCan = true;

            for (int rowItemIndex = rowItemCoordinate; rowItemIndex < tetromino.ColumnsLenght + rowItemCoordinate; rowItemIndex++)
            {
                for (int rowIndex = rowCoordinate; rowIndex < tetromino.RowsLenght + rowCoordinate; rowIndex++)
                {
                    var tetrominoCell = currentTetromino.Cells[rowIndex - rowCoordinate, rowItemIndex - rowItemCoordinate];

                    if (tetrominoCell == TetrominoCellType.Empty)
                    {
                        continue;
                    }

                    // Check if not empty cell is outside field
                    if (/*rowItemIndex == 0 & rowItemIndex + tetromino.EmptyColumnsOnLeftSideCount == 0 ||*/
                        (rowItemIndex < 0 & rowItemCoordinate + tetromino.EmptyColumnsOnLeftSideCount < 0) ||
                        rowItemCoordinate + currentTetromino.ColumnsLenght -1 - currentTetromino.EmptyColumnsOnRightSideCount >= playfieldInnerState.Width ||
                        rowIndex > playfieldInnerState.Height - 1)
                    {
                        return false;
                    }

                    if (rowItemIndex > playfieldInnerState.Width - 1)
                    {
                        return false;
                    }

                    // Cell within playfield, checking for collision
                    youCan &= !CheckIfCellsIntersect(tetrominoCell,
                        playfieldInnerState.Cells[rowIndex, rowItemIndex < 0 ? 0 : rowItemIndex]);
                }
            }

            return youCan;
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