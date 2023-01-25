using BlockBuster.Field;
using BlockBuster.InputHandler;
using BlockBuster.Score;
using BlockBuster.Settings;
using BlockBuster.Sound;
using BlockBuster.State;
using BlockBuster.Tetrominoes;
using BlockBuster.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlockBuster.Game;

/// <summary>
/// Super Rotation System Engine
/// </summary>
internal class SRSRulesEngine : IRulesEngine
{
    private InnerPlayfield playfieldInnerState; // All static elements without current tetromino
    public VisiblePlayfield playfieldToDisplay;  // All static elements with current tetromino

    public event EventHandler<VisiblePlayfield> PlayFieldUpdated; // Event fires each time clean playfield is updated
    public event EventHandler<GameState> GameStateUpdated; // Event fires each time game state is updated
    public event EventHandler<TetrisSound> SoundTriggered;

    private Tetromino currentTetromino = null;
    private Tetromino currentTetrominoGhost = null;

    internal readonly GameState gameState;

    private readonly HistoryStack<ScoreablePlayfieldAction> history = new(200);

    public SRSRulesEngine(IInputHandler inputHandler)
    {
        inputHandler.InputProvided += InputProvided; // Set input handler

        playfieldToDisplay = new VisiblePlayfield(10, 22);
        playfieldInnerState = new InnerPlayfield(playfieldToDisplay.Width, playfieldToDisplay.Height);

        gameState = new GameState();
    }

    #region StateManagement

    public Task Start()
    {
        gameState.State = Common.EngineState.Running;

        return Task.Run(() =>
        {
            while (true)
            {
                if (gameState.State == Common.EngineState.Running)
                {
                    Tick();
                    Task.Delay(Variables.TickRate).Wait();
                }
                else if (gameState.State == Common.EngineState.Stopped || gameState.State == Common.EngineState.GameOver)
                {
                    return;
                }
                else
                {
                    Task.Delay(Variables.TickRate).Wait();
                }
            }
        });
    }

    private void Tick()
    {
        SpawnTetromino();
        Gravity();
        UpdateGhost();
        var rowsDestroyed = DestroyRows();
        PushEventsFromHistoryToScoreCounter(rowsDestroyed);

        if ((gameState.LastTimePlayfieldWasUpdated + Variables.RenderUpdateRate) < DateTime.Now)
        {
            UpdateFieldToDisplay(currentTetromino, currentTetrominoGhost);
            PlayFieldUpdated?.Invoke(this, playfieldToDisplay);
            gameState.LastTimePlayfieldWasUpdated = DateTime.Now;
        }

        GameStateUpdated?.Invoke(this, gameState);
    }

    // Interpret moves and send them to counter
    private void PushEventsFromHistoryToScoreCounter(int linesCleared)
    {
        // Return If nothing to report
        if (currentTetromino is null || !(history.Peek(0)?.Tetromino.IsLanded ?? false))
        {
            return;
        }

        Debug.WriteLineIf(linesCleared > 0, "Lines cleared:" + linesCleared);

        // Report: By lines cleared + T-Spin
        var previousActionBeforeLanding = history.Peek(1);
        bool lastActionBeforeLandingWasRotation =
            previousActionBeforeLanding?.Action == ScoreAction.RotatedLeft ||
            previousActionBeforeLanding?.Action == ScoreAction.RotatedRight;

        if (previousActionBeforeLanding is not null && 
            currentTetromino is TetrominoT && 
            lastActionBeforeLandingWasRotation)
        {
            ScoreAction action;

            // T-Spin
            if (previousActionBeforeLanding.TSpin == TSpin.Normal)
            {
                if (linesCleared == 0)
                {
                    action = ScoreAction.TSpinNoLines;
                }
                else if (linesCleared == 1)
                {
                    action = ScoreAction.TSpinSingle;
                }
                else if (linesCleared == 2)
                {
                    action = ScoreAction.TSpinDouble;
                }
                else if (linesCleared == 3)
                {
                    action = ScoreAction.TSpinTriple;
                }
                else
                {
                    throw new AggregateException("Not supported");
                }
            }
            // T-Spin upgrade by last kick
            else if (previousActionBeforeLanding.TSpin == TSpin.Mini && history.Peek(0).WithLastKick)
            {
                if (linesCleared == 0)
                {
                    action = ScoreAction.TSpinNoLines;
                }
                else if (linesCleared == 1)
                {
                    action = ScoreAction.TSpinSingle;
                }
                else if (linesCleared == 2)
                {
                    action = ScoreAction.TSpinDouble;
                }
                else
                {
                    throw new AggregateException("Not supported");
                }
            }
            // T-Spin mini
            else if (previousActionBeforeLanding.TSpin == TSpin.Mini)
            {
                if (linesCleared == 0)
                {
                    action = ScoreAction.TSpinMiniNoLines;
                }
                else if (linesCleared == 1)
                {
                    action = ScoreAction.TSpinMiniSingle;
                }
                else if (linesCleared == 2)
                {
                    action = ScoreAction.TSpinMiniDouble;
                }
                else
                {
                    throw new AggregateException("Not supported");
                }
            }
            else
            {
                throw new AggregateException("Should not be here");
            }

            gameState.ScoreCounter.RegisterAction(new ScoreablePlayfieldAction(currentTetromino, action) { LinesCleared = linesCleared });
        }

        // Report: By lines cleared(1-4)
        else if (linesCleared > 0)
        {
            ScoreAction action;

            if (linesCleared == 1)
            {
                action = ScoreAction.Single;
            }
            else if (linesCleared == 2)
            {
                action = ScoreAction.Double;
            }
            else if (linesCleared == 3)
            {
                action = ScoreAction.Triple;
            }
            else if (linesCleared == 4)
            {
                action = ScoreAction.Tetris;
            }
            else
            {
                throw new AggregateException("Not supported");
            }

            gameState.ScoreCounter.RegisterAction(new ScoreablePlayfieldAction(currentTetromino, action) { LinesCleared = linesCleared });
        }


        // Report: By playfield state(perfect clears)
        if (playfieldInnerState.IsEmpty && linesCleared > 0)
        {
            ScoreAction action;
            if (linesCleared == 1)
            {
                action = ScoreAction.PerfectClearSingleLine;
            }
            else if (linesCleared == 2)
            {
                action = ScoreAction.PerfectClearDoubleLine;
            }
            else if (linesCleared == 3)
            {
                action = ScoreAction.PerfectClearTripleLine;
            }
            else if (linesCleared == 2)
            {
                action = ScoreAction.PerfectClearTetris;
            }
            else
            {
                throw new AggregateException("Not supported");
            }

            gameState.ScoreCounter.RegisterAction(new ScoreablePlayfieldAction(currentTetromino, action) { LinesCleared = linesCleared });
        }


        // Clear history or mino
        history.Items.Clear();
    }

    private void UpdateFieldToDisplay(Tetromino tetromino, Tetromino tetrominoGhost)
    {
        // Add all static elements to playfield to display
        for (int row = 0; row < playfieldToDisplay.Height; row++)
        {
            for (int rowItemIndex = 0; rowItemIndex < playfieldToDisplay.Width; rowItemIndex++)
            {
                playfieldToDisplay[row, rowItemIndex] = Cell.FromInnerCell(playfieldInnerState[row, rowItemIndex + 0], false);
            }
        }

        // Add ghost
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
                playfieldToDisplay[height, rowItem + tetrominoGhost.WidthLocation] = Cell.FromInnerCell(cellToPaste, true);
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

                playfieldToDisplay[row + tetromino.HeightLocation, rowItem + tetromino.WidthLocation] = Cell.FromInnerCell(cellToPaste, false);
            }
        }
    }

    private bool SpawnTetromino(Tetromino tetrominoToSpawnFromHold = null)
    {
        if (tetrominoToSpawnFromHold == null)
        {
            // Return  if tetromino exists already
            if (currentTetromino != null && !(currentTetromino?.IsLanded ?? false))
            {
                return false;
            }

            //gameState.ThisMinoInfinityAvailable = true;
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
        gameState.State = Common.EngineState.GameOver;
        SoundTriggered?.Invoke(this, TetrisSound.GameOver);
    }

    public void Pause(bool pause) => gameState.State = pause ? Common.EngineState.Paused : Common.EngineState.Running;

    public void Stop() => gameState.State = Common.EngineState.Stopped;

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
                HardDrop();
                break;

            case InputType.SoftDrop:
                SoftDrop();
                break;

            case InputType.Hold:
                Hold();
                break;

            case InputType.Pause:
                Pause(true);
                break;

            case InputType.None:
                break;

            default:
                throw new NotImplementedException($"Input {inputType} not implemented");
        }
    }

    private void Hold()
    {
        if (gameState.CanUseHold)
        {
            var tetrominoToSpawnFromHold = gameState.Queue.GetTetrominoFromHold(currentTetromino.GetType());
            currentTetromino = null;
            SpawnTetromino(tetrominoToSpawnFromHold);
            SoundTriggered?.Invoke(this, TetrisSound.Hold);
        }
    }

    private bool RotateLeft() => RotateLeftOrRight(false);

    private bool RotateRight() => RotateLeftOrRight(true);

    private bool RotateLeftOrRight(bool right)
    {
        var rotated = false;
        if (currentTetromino.IsLanded)
        {
            return false;
        }

        // Try rotate piece
        var rotationTransition = right ? currentTetromino.RotateRight() : currentTetromino.RotateLeft();
        if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetromino.HeightLocation, currentTetromino.WidthLocation))
        {
            gameState.TimeInfinityTriggered = DateTime.Now;
            rotated = true;
        }

        // Try rotate piece if have rotations(all, except O)
        else if (Config.EnableWallKick &
            rotationTransition != MinoRotationTransition.Rotation_0_0 &
            CheckIfCanBePlacedOnCoordinateWithKick(
                currentTetromino,
                rotationTransition,
                currentTetromino.HeightLocation,
                currentTetromino.WidthLocation,
                out int newRowCoordinate,
                out int newRowItemCoordinate,
                out TSpin tSpin,
                out bool lastKick))
        {
            currentTetromino.HeightLocation = newRowCoordinate;
            currentTetromino.WidthLocation = newRowItemCoordinate;

            rotated = true;
            history.Push(new ScoreablePlayfieldAction(currentTetromino, ScoreAction.RotatedLeft) { WithLastKick = lastKick, TSpin = tSpin });
            gameState.TimeInfinityTriggered = DateTime.Now;
        }

        // Rotate mino back on failure to rotate
        else
        {
            var _ = (right ? currentTetromino.RotateLeft() : currentTetromino.RotateRight());
        }

        if (rotated)
        {
            if (gameState.ThisMinoInfinityAvailable)
            {
                gameState.TimeInfinityTriggered = DateTime.Now;
                gameState.ThisMinoInfinityAvailable = false;
            }
            SoundTriggered?.Invoke(this, TetrisSound.Rotation);
        }

        return rotated;
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
            history.Push(new ScoreablePlayfieldAction(currentTetromino, ScoreAction.MovedLeft));
            SoundTriggered?.Invoke(this, TetrisSound.Movement);
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
            history.Push(new ScoreablePlayfieldAction(currentTetromino, ScoreAction.MovedRight));
            SoundTriggered?.Invoke(this, TetrisSound.Movement);
            return true;
        }
        return false;
    }

    private bool MoveMinoDown(bool hardDrop = false)
    {
        if (currentTetromino.IsLanded)
        {
            return false;
        }

        bool infinity = gameState.TimeInfinityTriggered + Variables.InfinityTime > DateTime.Now && gameState.ThisMinoInfinityAvailable & !hardDrop;
        if (IfTouchedFoundationOrAnotherTetrominoUnderneath(currentTetromino) &&
            (hardDrop || gameState.LastTimeTetrominoMovedDown.Add(Variables.LockDelayTimeout) < DateTime.Now))
        {
            if (infinity)
            {
                return false;
            }
            else
            {
                AddCurrentTetrominoToInnerState();

                SoundTriggered?.Invoke(this, TetrisSound.Locking);
                return false;
            }
        }

        var newLocation = currentTetromino.HeightLocation + 1; // Plus one because coordinates starts from top left
        if (CheckIfCanBePlacedOnCoordinate(currentTetromino, newLocation, currentTetromino.WidthLocation))
        {
            currentTetromino.HeightLocation = newLocation;
            return true;
        }

        return false;
    }

    private void SoftDrop()
    {
        MoveMinoDown();
        gameState.ScoreCounter.RegisterAction(new ScoreablePlayfieldAction(currentTetromino, ScoreAction.SoftDrop) { DroppedLines = 1 });
    }

    private void HardDrop()
    {
        var before = currentTetromino.HeightLocation;
        while (MoveMinoDown(true))
        {
            // Empty
        }
        gameState.ScoreCounter.RegisterAction(new ScoreablePlayfieldAction(currentTetromino, ScoreAction.HardDrop) { DroppedLines = currentTetromino.HeightLocation - before });
        SoundTriggered?.Invoke(this, TetrisSound.Locking);
    }

    private void Gravity()
    {
        if (DateTime.Now > gameState.LastTimeTetrominoMovedDown + gameState.CurrentPerRowInterval)
        {
            MoveMinoDown();
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

    private int DestroyRows()
    {
        var rowsDestroyed = 0;
        for (int row = playfieldInnerState.Height - 1; row >= 0; row--)
        {
            if (CheckRowFilled(row))
            {
                DestroyRow(row);
                MoveTopRowsToRow(row);
                rowsDestroyed++;
            }
        }

        if (rowsDestroyed > 0)
        {
            Debug.WriteLine($"Removed rows:{rowsDestroyed} with mino: {history.Peek(0).Tetromino.GetType()}");
            SoundTriggered?.Invoke(this, TetrisSound.LineClear);
        }

        return rowsDestroyed;
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
                if ((rowItemIndex < 0 & rowItemCoordinate + tetromino.EmptyColumnsOnLeftSideCount < 0) ||
                    rowItemCoordinate + currentTetromino.ColumnsLenght - 1 - currentTetromino.EmptyColumnsOnRightSideCount >= playfieldInnerState.Width ||
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
    private bool CheckIfCanBePlacedOnCoordinateWithKick(
        Tetromino tetromino,
        MinoRotationTransition minoRotationTransition,
        int rowCoordinate,
        int rowItemCoordinate,
        out int newRowCoordinate,
        out int newRowItemCoordinate,
        out TSpin tSpin,
        out bool lastKick)
    {
        var kicksForThisRotatioin = tetromino.wallKicks[minoRotationTransition];
        foreach (var kick in kicksForThisRotatioin.Select((value, i) => new { i, value }))
        {
            if (CheckIfCanBePlacedOnCoordinate(tetromino, rowCoordinate + -(kick.value.Item2), rowItemCoordinate + kick.value.Item1))
            {
                newRowCoordinate = rowCoordinate + -(kick.value.Item2); // Invert this value, because I use default SRS kick table, but I count rows from top to bottom
                newRowItemCoordinate = rowItemCoordinate + kick.value.Item1;
                lastKick = kick.i == kicksForThisRotatioin.Count - 1;
                tSpin = CalculateTSpinType(tetromino);
                return true;
            }
        }

        newRowCoordinate = rowCoordinate;
        newRowItemCoordinate = rowItemCoordinate;
        lastKick = false;
        tSpin = TSpin.None;
        return false;
    }


    /// <summary>
    /// <see cref="https://tetris.wiki/T-Spin"/>
    /// </summary>
    /// <param name="tetromino"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private TSpin CalculateTSpinType(Tetromino tetromino)
    {
        if (tetromino is not TetrominoT)
        {
            return TSpin.None;
        }

        var topCornerCells = tetromino.Rotation.Rotation switch
        {
            MinoRotation.Rotation_0 => ((0, 0), (0, 1)),
            MinoRotation.Rotation_R => ((0, 2), (2, 2)),
            MinoRotation.Rotation_2 => ((2, 0), (2, 2)),
            MinoRotation.Rotation_L => ((0, 0), (2, 0)),
            _ => throw new NotImplementedException(),
        };

        var bottomCornerCells = tetromino.Rotation.Rotation switch
        {
            MinoRotation.Rotation_0 => ((2, 0), (2, 2)),
            MinoRotation.Rotation_R => ((0, 0), (2, 0)),
            MinoRotation.Rotation_2 => ((0, 0), (0, 2)),
            MinoRotation.Rotation_L => ((0, 2), (2, 2)),
            _ => throw new NotImplementedException(),
        };

        var topCell1 = IsCellFilledOrOutsidePlayfield(topCornerCells.Item1);
        var topCell2 = IsCellFilledOrOutsidePlayfield(topCornerCells.Item2);
        var bottomCell1 = IsCellFilledOrOutsidePlayfield(bottomCornerCells.Item1);
        var bottomCell2 = IsCellFilledOrOutsidePlayfield(bottomCornerCells.Item2);


        if (topCell1 & topCell2 & (bottomCell1 || bottomCell2))
        {
            return TSpin.Normal;
        }
        else if ((topCell1 || topCell2) & (bottomCell1 & bottomCell2))
        {
            return TSpin.Mini;
        }
        else
        {
            return TSpin.None;
        }

        //continue debugging this method
        bool IsCellFilledOrOutsidePlayfield((int, int) coordinate)
        {
            var heightToCheck = tetromino.HeightLocation + coordinate.Item1;
            var widthToCheck = tetromino.WidthLocation + coordinate.Item2;

            // If empty cell outside playfield, then it is cosidered filled
            // For T mino possible only from bottom
            if (widthToCheck < 0 || // left side
                tetromino.WidthLocation > tetromino.WidthLocation + tetromino.ColumnsLenght - 1 - tetromino.EmptyColumnsOnRightSideCount || // right side
                tetromino.HeightLocation > tetromino.HeightLocation + tetromino.RowsLenght - 1 - tetromino.EmptyRowsOnBottomSideCount) // bottom
            {
                return true;
            }

            return playfieldInnerState.Cells[heightToCheck, widthToCheck] != TetrominoCellType.Empty;
        }
    }

    private static bool CheckIfCellsIntersect(TetrominoCellType fieldCell, TetrominoCellType tetrominoeCell) =>
        (int)fieldCell > 0 && (int)tetrominoeCell > 0;

    #endregion
}