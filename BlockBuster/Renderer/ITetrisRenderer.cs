using BlockBuster.Field;
using BlockBuster.Score;
using BlockBuster.State;
using System;

namespace BlockBuster.Renderer
{

    public interface ITetrisRenderer
    {
        public void RenderGameState(object sender, GameState e);

        public void RenderPlayfield(object sender, VisiblePlayfield e);

        void RenderTextualFeedback(HistoryStack<Tuple<string, string>> historyStack);
    }
}