using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export]
    private bool _invencible = false;

    private Vector3 _targetVelocity = Vector3.Zero;

    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public Area3D MobDetector { get; set; } = null;

    [Export]
    public int Speed { get; set; } = 14;

    [Export]
    public int FallAcceleration { get; set; } = 75;

    [Export]
    public int JumpImpulse { get; set; } = 20;
    [Export]
    public int BounceImpulse { get; set; } = 16;

    public override void _Ready()
    {
        base._Ready();

        if (MobDetector != null)
        {
            MobDetector.BodyEntered += node =>
            {
                Die();
            };
        }
    }

    private void Die()
    {
        if (_invencible)
        {
            return;
        }

        EmitSignal(SignalName.Hit);
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = Vector3.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            direction.Z += 1.0f; 
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            GetNode<Node3D>("Pivot").LookAt(Position + direction, Vector3.Up);
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 4;
        }
        else
        {
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1;
        }

        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        if (!IsOnFloor())
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }
        else if (Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpImpulse;
        }

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is Mob mob)
            {
                // hit from above
                if (!mob.HasSquashed && Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
                {
                    mob.Squash();
                    _targetVelocity.Y = BounceImpulse;
                }
            }
        }

        Velocity = _targetVelocity;
        MoveAndSlide();

        var pivot = GetNode<Node3D>("Pivot");
        pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);
    }
}
