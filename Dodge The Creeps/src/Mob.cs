using Godot;
using System;

public partial class Mob : RigidBody2D
{
    private AnimatedSprite2D _animatedSprite2D = null;

    public override void _Ready()
    {
        base._Ready();

        _animatedSprite2D = GetNode<AnimatedSprite2D>(nameof(AnimatedSprite2D));
        var mobTypes = _animatedSprite2D.SpriteFrames.GetAnimationNames();
        _animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]);

        GetNode<VisibleOnScreenNotifier2D>(nameof(VisibleOnScreenNotifier2D))
            .ScreenExited += OnScreenExited;
    }

    private void OnScreenExited()
    {
        QueueFree();
    }
}
