using BlockBusterXaml.InputHandler;
using BlockBusterXaml.Renderer;
using BlockBusterXaml.Sound;
using System.Threading.Tasks;

namespace BlockBusterXaml.Game;

public class Tetris
{
    internal SRSRulesEngine rulesEngine;

    internal ITetrisRenderer tetrisRenderer;
    private readonly ISoundPlayer soundPlayer;
    internal IInputHandler inputController;

    public Tetris(IInputHandler inputController, ITetrisRenderer tetrisRenderer, ISoundPlayer player)
    {
        this.inputController = inputController;
        this.tetrisRenderer = tetrisRenderer;
        this.soundPlayer = player;
    }

    public Task Start()
    {
        this.rulesEngine = new SRSRulesEngine(this.inputController);
        this.rulesEngine.PlayFieldUpdated += this.tetrisRenderer.RenderPlayfield;
        this.rulesEngine.GameStateUpdated += this.tetrisRenderer.RenderGameState;
        this.rulesEngine.SoundTriggered += this.soundPlayer.PlaySound;

        return rulesEngine.Start();
    }

    public void Pause(bool pause)
    {
        rulesEngine.Pause(pause);
    }

    public void Stop()
    {
        rulesEngine.Stop();
    }
}
