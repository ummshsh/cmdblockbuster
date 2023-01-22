using BlockBuster.Score;
using BlockBuster.Tetrominoes;

namespace CmdBlockbusterTests;

[TestClass]
public class ScoreCounterTests
{
    [TestMethod("Check score for action that are not require history")]
    public void CheckScoreForActionsThatAreNotRequireHistory()
    {
        var counter = new ScoreCounter();
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single)
        {
            LinesCleared = 1
        });
        counter.RegisterAction(new ScoreablePlayfieldAction(new TetrominoI(), ScoreAction.Single)
        {
            LinesCleared = 1
        });

        Assert.IsTrue(counter.Score > 0);
    }

    [TestMethod("Check back to back scoring")]
    public void CheckScoreForBackToBack()
    {
        var counter = new ScoreCounter();
        Assert.Fail();
    }

    [TestMethod("Check combo scoring")]
    public void CheckScoreForCombo()
    {
        var counter = new ScoreCounter();
        Assert.Fail();
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