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

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(250, counter.Score);
        Assert.AreEqual(1, counter.ComboCounter);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(2, counter.ComboCounter);
        Assert.AreEqual(450, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(3, counter.ComboCounter);
        Assert.AreEqual(700, counter.Score);

        // Breaking combo
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(700, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(800, counter.Score);

        // Actions that not award points, just break BTB and Combo
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(800, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Tetris) { LinesCleared = 4 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(1600, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Tetris) { LinesCleared = 4 });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(3250, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(3250, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(3350, counter.Score);

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
        Assert.AreEqual(3350, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(3350, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.TSpinDouble) { LinesCleared = 2 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(4550, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(4650, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.SoftDrop) { DroppedLines = 5 });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(4655, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.SoftDrop) { DroppedLines = 5 });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.HardDrop) { DroppedLines = 5 });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));

        Assert.AreEqual(-1, counter.ComboCounter);
        Assert.AreEqual(4670, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.PerfectClearSingleLine) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(5470, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Landed));
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single) { LinesCleared = 1 });

        Assert.AreEqual(0, counter.ComboCounter);
        Assert.AreEqual(5570, counter.Score);

        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.PerfectClearDoubleLine) { LinesCleared = 2 });

        Assert.AreEqual(1, counter.ComboCounter);
        Assert.AreEqual(7970, counter.Score);
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
}