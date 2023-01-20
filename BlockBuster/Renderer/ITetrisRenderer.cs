using BlockBuster.Field;
using BlockBuster.State;

namespace BlockBuster.Renderer;

public interface ITetrisRenderer
{
    public void RenderGameState(object sender, GameState e);
    public void RenderPlayfield(object sender, VisiblePlayfield e);
}
