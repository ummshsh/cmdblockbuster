using CMDblockbuster.Tetrominoes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdblockbuster.Game
{
    internal class TetrominoQueue
    {
        public Type NextTetromino => stack.Peek();

        public IEnumerable<Type> NextQueuePreview => stack.Take(5);

        private Type _hold = null;

        public Tetromino HoldTetrominoInstance =>
            Activator.CreateInstance(HoldTetrominoType) as Tetromino;

        public Type HoldTetrominoType
        {
            get
            {
                return _hold;
            }
            set
            {
                _hold = value;
            }
        }

        public readonly Stack<Type> stack;

        // Tetromino types
        public Type[] tetrominoes = new[] {
                typeof(TetrominoI),
                typeof(TetrominoJ),
                typeof(TetrominoL),
                typeof(TetrominoO),
                typeof(TetrominoS),
                typeof(TetrominoT),
                typeof(TetrominoZ)};

        public TetrominoQueue()
        {
            stack = new Stack<Type>();
            GenerateInitialQueue();
            RegenerateQueue();
        }

        private void GenerateInitialQueue()
        {
            var random = new Random();
            Enumerable.Range(0, tetrominoes.Length)
                .OrderBy(x => random.Next())
                .ToList()
                .ForEach(t => stack.Push(tetrominoes[t]));
        }

        // TODO: make actuall 7pack spawn here
        private void RegenerateQueue()
        {
            if (stack.Count < 8)
            {
                var random = new Random();
                Enumerable.Range(0, tetrominoes.Length)
                    .OrderBy(x => random.Next())
                    .ToList()
                    .ForEach(t => stack.Push(tetrominoes[t]));
            }
        }

        public Tetromino GetTetrominoFromQueue()
        {
            var tetromino = Activator.CreateInstance(/*typeof(TetrominoI)*/ stack.Pop()) as Tetromino;
            RegenerateQueue();
            return tetromino;
        }

        public Tetromino GetTetrominoFromHold(Type tetrominoToStoreInstead)
        {
            var tetrominoFromHold = Activator.CreateInstance(HoldTetrominoType) as Tetromino;
            HoldTetrominoType = tetrominoToStoreInstead;
            return tetrominoFromHold;
        }
    }
}
