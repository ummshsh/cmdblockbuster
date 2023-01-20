using BlockBuster.Game;
using BlockBuster.Tetrominoes;

namespace CmdBlockbusterTests;

[TestClass]
public class BlockBusterTests
{
    [TestMethod("Check Queue For Absence Of Snake Sequences Longer Than Two")]
    public void CheckQueueForAbsenceOfSnakeSequencesLongerThanTwo()
    {
        var queue = new TetrominoQueue();
        var snakeCounter = 0;
        var minoChecked = 0;

        while (minoChecked < 7001)
        {
            var mino = queue.GetTetrominoFromQueue();
            snakeCounter = IsSnake(mino.GetType()) ? snakeCounter + 1 : 0;

            if (snakeCounter > 2)
            {
                Assert.Fail($"3 snakes detected. Checked {minoChecked} of 7000");
            }

            minoChecked++;
        }
    }

    private static bool IsSnake(Type mino)
    {
        return mino == typeof(TetrominoS) ||
                mino == typeof(TetrominoZ);
    }
}