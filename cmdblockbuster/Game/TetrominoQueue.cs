using CMDblockbuster.Tetrominoes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdblockbuster.Game
{
    public class TetrominoQueue
    {
        public Type NextTetromino => queue.Peek();

        public IEnumerable<Type> NextQueuePreview => queue.Take(5);

        private readonly Random random = new Random();

        public Type HoldTetrominoType { get; private set; }

        public readonly Queue<Type> queue;

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
            queue = new Queue<Type>();
            GenerateInitialQueue();
            RegenerateQueue();
        }

        private void GenerateInitialQueue()
        {
            Enumerable.Range(0, tetrominoes.Length)
                .OrderBy(x => random.Next())
                .ToList()
                .ForEach(t => queue.Enqueue(tetrominoes[t]));
        }
        
        private void RegenerateQueue()
        {
            if (queue.Count < 8)
            {
                Enumerable.Range(0, tetrominoes.Length)
                    .OrderBy(x => random.Next())
                    .ToList()
                    .ForEach(t => queue.Enqueue(tetrominoes[t]));
            }
        }

        public Tetromino GetTetrominoFromQueue()
        {
            var tetromino = Activator.CreateInstance(/*typeof(TetrominoJ)*/ queue.Dequeue()) as Tetromino;
            RegenerateQueue();
            return tetromino;
        }

        public Tetromino GetTetrominoFromHold(Type tetrominoToStoreInstead)
        {
            Type typeToReturn;
            if (HoldTetrominoType == null)
            {
                typeToReturn = queue.Dequeue();
                HoldTetrominoType = tetrominoToStoreInstead;
            }
            else
            {
                typeToReturn = HoldTetrominoType;
                HoldTetrominoType = tetrominoToStoreInstead;
            }

            return Activator.CreateInstance(typeToReturn) as Tetromino;
        }
    }
}
