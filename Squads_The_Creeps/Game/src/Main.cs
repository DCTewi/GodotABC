using Godot;

using Squash.UI;

using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    public override void _Ready()
    {
        base._Ready();

        GD.Print($"Read from another library {AnotherLibrary.GlobalVariables.MyProperty}");

        if (GetNode("Player") is Player player)
        {
            player.Hit += () =>
            {
                GetNode<Godot.Timer>("MobTimer").Stop();

                if (GetNode("UserInterface/Retry") is Control retry)
                {
                    retry.Show();
                }
            };
        }

        if (GetNode("UserInterface/Retry") is Control control)
        {
            control.Hide();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event.IsActionPressed("ui_accept") && (GetNode<Control>("UserInterface/Retry")?.Visible ?? false))
        {
            GetTree().ReloadCurrentScene();
        }
    }

    private void OnMobTimerTimeout()
    {
        if (GetNode("Player") is Player player)
        {
            Mob mob = MobScene.Instantiate<Mob>();

            var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
            mobSpawnLocation.ProgressRatio = GD.Randf();

            Vector3 playerPos = player.Position;
            mob.Initialize(mobSpawnLocation.Position, playerPos);
            mob.Squashed += GetNode<ScoreLabel>("UserInterface/ScoreLabel").OnMobSquashed;

            AddChild(mob);
        }
    }
}
