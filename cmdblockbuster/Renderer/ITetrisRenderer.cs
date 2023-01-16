using cmdblockbuster.Field;
using cmdblockbuster.Game;

namespace CMDblockbuster.Renderer
{
    public interface ITetrisRenderer
    {
        public void RenderGameState(object sender, GameState e);
        public void RenderPlayfield(object sender, VisiblePlayfield e);
    }
}
