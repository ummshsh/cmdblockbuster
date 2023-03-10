using BlockBuster.Score;
using BlockBuster.Tetrominoes;

namespace CmdBlockbusterTests;

[TestClass]
public class ScoreCounterTests
{
    [TestMethod("Check score case: 1")]
    public void CheckScoreCounterCaseOne()
    {
        var counter = new ScoreCounter();

        // LEVEL 1
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });
        Assert.AreEqual(100, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });
        Assert.AreEqual(250, counter.Score);
        Assert.AreEqual(1, counter.ComboCounter);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(2, counter.ComboCounter);
        Assert.AreEqual(450, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(3, counter.ComboCounter);
        Assert.AreEqual(700, counter.Score);

        //// Breaking combo
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(700, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(800, counter.Score);

        //// Actions that not award points, just break Combo
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(0, counter.DifficultMoveCounter);
        Assert.AreEqual(800, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Tetris) { LinesCleared = 4 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(1, counter.DifficultMoveCounter);
        Assert.AreEqual(1600, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Tetris) { LinesCleared = 4 });

        // Score after BTB: (previous score 1600) + (tetris score 800 * BTB 1.5) + (1 combo 50)
        Assert.AreEqual(2, counter.DifficultMoveCounter);
        Assert.AreEqual(2850, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(2, counter.DifficultMoveCounter);
        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(2850, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(2850, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        // LEVEL 2
        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(0, counter.DifficultMoveCounter);
        Assert.AreEqual(3050, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedLeft));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedRight));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedRight));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedRight));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedRight));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedLeft));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedLeft));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedLeft));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedLeft));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.RotatedLeft));

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(0, counter.DifficultMoveCounter);
        Assert.AreEqual(3050, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(3050, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.TSpinDouble) { LinesCleared = 2 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(1, counter.DifficultMoveCounter);
        Assert.AreEqual(5450, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(5650, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.SoftDrop) { DroppedLines = 5 });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(5655, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.SoftDrop) { DroppedLines = 5 });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.HardDrop) { DroppedLines = 5 });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(5670, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.PerfectClearSingleLine) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(7270, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(7470, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.PerfectClearDoubleLine) { LinesCleared = 2 });

        Assert.AreEqual(1, counter.ComboCounter);
        Assert.AreEqual(9970, counter.Score);
    }

    [TestMethod("Check score case: BTB into Perfect clear")]
    public void CheckScoreBtbPerfectClear()
    {
        var counter = new ScoreCounter();

        Assert.AreEqual(-1, counter.ComboCounter);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });
        Assert.AreEqual(100, counter.Score);
        Assert.AreEqual(0, counter.ComboCounter);


        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });
        Assert.AreEqual(1, counter.ComboCounter);
        Assert.AreEqual(250, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        Assert.AreEqual(-1, counter.ComboCounter);
    }

    [TestMethod("Check History Stack")]
    public void CheckHistoryStack()
    {
        var stack = new HistoryStack<string>(5);

        // Push
        stack.Push("1");
        stack.Push("2");
        stack.Push("3");
        stack.Push("4");
        stack.Push("5");
        stack.Push("6");
        Assert.AreEqual(5, stack.Items.Count, "Only 5 items allowed");

        // Peek
        Assert.AreEqual("6", stack.Peek(0));
        Assert.AreEqual("5", stack.Peek(1));
        Assert.AreEqual("4", stack.Peek(2));
        Assert.AreEqual("3", stack.Peek(3));
        Assert.AreEqual("2", stack.Peek(4));
        Assert.AreEqual(null, stack.Peek(5));

        // Pop
        Assert.AreEqual("6", stack.Pop());
        Assert.AreEqual("5", stack.Pop());
        Assert.AreEqual("4", stack.Pop());
        Assert.AreEqual("3", stack.Pop());
        Assert.AreEqual("2", stack.Pop());
        Assert.AreEqual(null, stack.Pop());
    }

    [TestMethod("Check History Stack with multiple threads")]
    public void CheckHistoryStackMultipleThreads()
    {

    }
}