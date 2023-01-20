using BlockBusterXaml.Tetrominoes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockBusterXaml.Game;

/// <summary>
/// 7-Bag spawn system
/// </summary>
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
        GenerateInitialBag();
        RegenerateQueue();
    }

    private void GenerateInitialBag()
    {
        Enumerable.Range(0, tetrominoes.Length)
            .OrderBy(x => random.Next())
            .ToList()
            .ForEach(t => queue.Enqueue(tetrominoes[t]));

        // Make sure queue starts not form S\Z\O to avoid forced overhang
        while (queue.First() == typeof(TetrominoS) ||
            queue.First() == typeof(TetrominoZ) ||
            queue.First() == typeof(TetrominoO))
        {
            var newOrder = queue.OrderBy(t => random.Next()).ToList();
            queue.Clear();
            newOrder.ToList().ForEach(t => queue.Enqueue(t));
        }
    }

    private void RegenerateQueue()
    {
        if (queue.Count < 8)
        {
            // Generate new 7 bag
            var newBag = Enumerable.Range(0, tetrominoes.Length)
                .OrderBy(x => random.Next())
                .ToList()
                .Select(t => tetrominoes[t]);


            // Detect and break snake sequence 
            while (ContainsSnakeSequenceLongerThanTwo(queue, newBag))
            {
                var newOrder = newBag.OrderBy(x => random.Next()).ToList();
                newBag = newOrder;
            }
            newBag.ToList().ForEach(t => queue.Enqueue(t));
        }
    }

    /// <summary>
    /// Detects snake sequence longer than 2 on seam between 7bags
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="newBag"></param>
    /// <returns></returns>
    private bool ContainsSnakeSequenceLongerThanTwo(Queue<Type> queue, IEnumerable<Type> newBag)
    {
        var listToCheck = new List<Type>();
        listToCheck.AddRange(queue.TakeLast(2));
        listToCheck.AddRange(newBag.Take(2));

        var concequentSnakes = 0;

        foreach (var t in listToCheck)
        {
            concequentSnakes = (t == typeof(TetrominoS) || t == typeof(TetrominoZ)) ? ++concequentSnakes : 0;
        }

        return concequentSnakes > 2;
    }


    public Tetromino GetTetrominoFromQueue()
    {
        var tetromino = Activator.CreateInstance( queue.Dequeue()) as Tetromino;
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
