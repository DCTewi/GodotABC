using Godot;
using System;
using System.Threading.Tasks;

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void GameStartedEventHandler();

    public override void _Ready()
    {
        base._Ready();

        GetNode<Button>("StartButton").Pressed += () =>
        {
            GetNode<Button>("StartButton").Hide();
            EmitSignal(SignalName.GameStarted);
        };

        GetNode<Timer>("MessageTimer").Timeout += () =>
        {
            GetNode<Label>("Message").Hide();
        };
    }

    public void ShowMessage(string text)
    {
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();

        GetNode<Timer>("MessageTimer").Start();
    }

    public async void ShowGameOver()
    {
        ShowMessage("Game Over");

        var timer = GetNode<Timer>("MessageTimer");
        await ToSignal(timer, Timer.SignalName.Timeout);

        var message = GetNode<Label>("Message");
        message.Text = "Dodge the Creeps!";
        message.Show();

        await Task.Delay(1000);
        GetNode<Button>("StartButton").Show();
    }

    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }

}
