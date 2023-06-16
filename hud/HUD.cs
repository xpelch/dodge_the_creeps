using Godot;
using System.Threading.Tasks;

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    private Label _scoreLabel;
    private Label _message;
    private Timer _messageTimer;
    private Button _startButton;
    
    public override void _Ready()
    {
        _scoreLabel = GetNode<Label>("ScoreLabel");
        _message = GetNode<Label>("Message");
        _messageTimer = GetNode<Timer>("MessageTimer");
        _startButton = GetNode<Button>("StartButton");
    }

    public void UpdateScore(int score)
    {
        _scoreLabel.Text = score.ToString();
    }

    public void ShowMessage(string text)
    {
        _message.Text = text;
        _message.Show();
        _messageTimer.Start();
    }

    public async Task ShowGameOver()
    {
        ShowMessage("Game Over");
        await WaitForMessageTimeout();
        ShowRestartMessage();
        await WaitBeforeShowingStartButton();
        _startButton.Show();
    }

    private async Task WaitForMessageTimeout()
    {
        await ToSignal(_messageTimer, Timer.SignalName.Timeout);
    }

    private void ShowRestartMessage()
    {
        _message.Text = "Dodge the\nInvaders!";
        _message.Show();
    }

    private async Task WaitBeforeShowingStartButton()
    {
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
    }

    private void OnStartButtonPressed()
    {
        _startButton.Hide();
        EmitSignal(SignalName.StartGame);
    }

    private void OnMessageTimerTimeout()
    {
        _message.Hide();
    }
}