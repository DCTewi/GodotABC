using Godot;
using System;

public partial class Mob : CharacterBody3D
{
    [Signal]
    public delegate void SquashedEventHandler();

    [Export]
    public int MinSpeed { get; set; } = 10;

    [Export]
    public int MaxSpeed { get; set; } = 18;

    public bool HasSquashed { get; private set; } = false;

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    public void Initialize(Vector3 startPos, Vector3 playerPos)
    {
        LookAtFromPosition(startPos, playerPos, Vector3.Up);
        RotateY((float)GD.RandRange(-Mathf.Pi / 4.0, Mathf.Pi / 4.0));

        int randomSpeed = GD.RandRange(MinSpeed, MaxSpeed);
        Velocity = Vector3.Forward * randomSpeed;

        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);

        GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = randomSpeed / MinSpeed;
    }

    private void OnVisibilityNotifierScreenExited()
    {
        GD.Print("Mob exit from screen");
        QueueFree();
    }

    public void Squash()
    {
        HasSquashed = true;
        EmitSignal(SignalName.Squashed);
        QueueFree();
    }
}
