using BlockBusterXaml.Field;
using BlockBusterXaml.State;

namespace BlockBusterXaml.Renderer;

public interface ITetrisRenderer
{
    public void RenderGameState(object sender, GameState e);
    public void RenderPlayfield(object sender, VisiblePlayfield e);
}
