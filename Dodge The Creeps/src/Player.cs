using Godot;
using System;

public partial class Player : Area2D
{
    private AnimatedSprite2D _animatedSprite = null;
    private CollisionShape2D _collisionShape = null;

    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int Speed { get; set; } = 400;

    public Vector2 ScreenSize { get; set; }

    public override void _Ready()
    {
        base._Ready();

        _animatedSprite = GetNode<AnimatedSprite2D>(nameof(AnimatedSprite2D));
        _collisionShape = GetNode<CollisionShape2D>(nameof(CollisionShape2D));

        ScreenSize = GetViewportRect().Size;

        BodyEntered += OnBodyEntered;

        Hide();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        var velocity = Vector2.Zero;

        velocity.X += Input.IsActionPressed("move_right") ? 1 : 0;
        velocity.X -= Input.IsActionPressed("move_left") ? 1 : 0;
        velocity.Y += Input.IsActionPressed("move_down") ? 1 : 0;
        velocity.Y -= Input.IsActionPressed("move_up") ? 1 : 0;

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            _animatedSprite?.Play();
        }
        else
        {
            _animatedSprite?.Stop();
        }

        if (!Mathf.IsZeroApprox(velocity.X))
        {
            _animatedSprite.Animation = "walk";
            _animatedSprite.FlipV = false;
            _animatedSprite.FlipH = velocity.X < 0;
        }
        else if (!Mathf.IsZeroApprox(velocity.Y))
        {
            _animatedSprite.Animation = "up";
            _animatedSprite.FlipV = velocity.Y > 0;
        }

        Position += velocity * (float)delta;
        Position = new Vector2
        {
            X = Mathf.Clamp(Position.X, 0, ScreenSize.X),
            Y = Mathf.Clamp(Position.Y, 0, ScreenSize.Y),
        };
    }

    private void OnBodyEntered(Node2D body)
    {
        Hide();
        EmitSignal(SignalName.Hit);
        _collisionShape?.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }

    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        _collisionShape.Disabled = false;
    }
}
