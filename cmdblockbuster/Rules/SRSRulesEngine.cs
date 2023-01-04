using CMDblockbuster.Common;
using CMDblockbuster.Field;
using CMDblockbuster.InputController;
using CMDblockbuster.Tetrominoes;
using System;
using System.Timers;

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
        private int currentTetrominoHeightLocation { get; set; }
        private int currentTetrominoWidthLocation { get; set; }

        // Time
        public TimeSpan TickRate { get; } = TimeSpan.FromMilliseconds(500);
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

        private bool running;


        public SRSRulesEngine(IInputHandler inputHandler, Playfield playFieldToDisplay)
        {
            // Set input handler
            inputHandler.InputProvided += InputProvided;

            // Init flayfields
            this.playfieldToDisplay = playFieldToDisplay;
            this.playfieldInnerState = new Playfield(playFieldToDisplay.Width, playFieldToDisplay.Height);

            ArrayLength = playfieldInnerState.Width * playfieldInnerState.Height;
        }

        public void Start()
        {
            this.running = true;

            // Starts timer and call Tick() each TickRate defined interval
            var tickRateTimer = new Timer();
            tickRateTimer.Elapsed += new ElapsedEventHandler(Tick);
            tickRateTimer.Interval = TickRate.TotalMilliseconds;
            tickRateTimer.Start();

            // TODO: for now just one speed
            var fallRateTimer = new Timer();
            fallRateTimer.Elapsed += new ElapsedEventHandler(Gravity);
            fallRateTimer.Interval = FallRate.TotalMilliseconds;
            fallRateTimer.Start();
        }

        public void Tick(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            PlayFieldUpdated?.Invoke(this, this.playfieldToDisplay);

            while (running)
            {
                SpawnTetromino();
                ShowTetrominoOnField();
                // Gravity: done using timer in Start()
                // TODO: figure out how to Stick tetromino(remove control of current tetromino) and then DestroyRows() if possible; 
                // CheckIfGameIsOver(); // if tetromino spawned, but landed on 2 offsceen rows with at least 1 block
                // once current tetromino changed, means it landed
            }
        }

        public void Pause() => this.running = false;

        private void InputProvided(object sender, InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Left:
                    break;

                case InputType.Right:
                    break;

                case InputType.RotateLeft:
                    break;

                case InputType.RotateRight:
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

        private bool MoveDown()
        {
            var newLocation = currentTetrominoHeightLocation + 1; // Plus one because coordinates starts from top left

            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetrominoHeightLocation, newLocation))
            {
                currentTetrominoHeightLocation = newLocation;
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
            // TODO: make tetromino fall immideately and release control of current tetromino
            throw new NotImplementedException();
        }

        private void Gravity(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            //MoveDown();
        }

        private void DestroyRows()
        {
            // TODO: define destroy rows with regard to FallRate
        }

        private void ShowTetrominoOnField()
        {
            // TODO: this guard check will not be necessary in future
            if (CheckIfCanBePlacedOnCoordinate(currentTetromino, currentTetrominoHeightLocation, currentTetrominoWidthLocation))
            {
                // Add all static elements to playfield to display
                Array.Copy(playfieldInnerState.field, playfieldToDisplay.field, ArrayLength);

                // Add current tetromino to playfield to display
                var currentTetromninoeYIndex = 0;
                var currentTetromninoeXIndex = 0;
                for (int row = currentTetrominoHeightLocation; row < currentTetromino.RowsLenght + currentTetrominoHeightLocation; row++)
                {
                    for (int rowItemIndex = currentTetrominoWidthLocation; rowItemIndex < currentTetrominoWidthLocation + currentTetromino.ColumnsLenght; rowItemIndex++)
                    {
                        playfieldToDisplay.field[row, rowItemIndex] = currentTetromino.Cells[currentTetromninoeXIndex, currentTetromninoeYIndex];
                        currentTetromninoeYIndex++;
                    }
                    currentTetromninoeXIndex++;
                    currentTetromninoeYIndex = 0;
                }
            }
        }

        public bool SpawnTetromino()
        {
            // Exit if tetromino exists already
            if (currentTetromino != null || (currentTetromino?.landed ?? false))
            {
                return false;
            }

            // Create tetromnino
            var randomInt = new Random().Next(0, tetrominoes.Length - 1);
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
        /// <param name="xCoordinate">Vertical top-down</param>
        /// <param name="yCoordinate">Horizontal left-right</param>
        /// <returns></returns>
        private bool CheckIfCanBePlacedOnCoordinate(Tetromino currentTetromino, int xCoordinate, int yCoordinate)
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