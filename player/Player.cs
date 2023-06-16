using Godot;
using System;

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export] public int Speed { get; set; } = 400;

    private Vector2 ScreenSize { get; set; }
    private AnimatedSprite2D AnimatedSprite { get; set; }

    public override void _Ready()
    {
        Hide();
        ScreenSize = GetViewportRect().Size;
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _Process(double delta)
    {
        var velocity = GetInputVelocity();
        ProcessPlayerMovement(velocity, delta);
        ProcessPlayerAnimation(velocity);
    }

    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
    
    private void OnBodyEntered(PhysicsBody2D body)
    {
        Hide(); // Player disappears after being hit.
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
    
    private Vector2 GetInputVelocity()
    {
        var velocity = Vector2.Zero;

        if (Input.IsActionPressed("move_right")) velocity.X += 1;
        if (Input.IsActionPressed("move_left")) velocity.X -= 1;
        if (Input.IsActionPressed("move_down")) velocity.Y += 1;
        if (Input.IsActionPressed("move_up")) velocity.Y -= 1;

        return velocity;
    }

    private void ProcessPlayerMovement(Vector2 velocity, double delta)
    {
        if (velocity.Length() <= 0) return;

        velocity = velocity.Normalized() * Speed;
        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );
    }

    private void ProcessPlayerAnimation(Vector2 velocity)
    {
        if (velocity.Length() > 0)
        {
            AnimatedSprite.Play();
        }
        else
        {
            AnimatedSprite.Stop();
            return;
        }

        if (velocity.X != 0)
        {
            SetHorizontalAnimation(velocity.X);
        }
        else if (velocity.Y != 0)
        {
            SetVerticalAnimation(velocity.Y);
        }
    }

    private void SetHorizontalAnimation(float direction)
    {
        AnimatedSprite.Animation = "walk";
        AnimatedSprite.FlipV = false;
        AnimatedSprite.FlipH = direction < 0;
    }

    private void SetVerticalAnimation(float direction)
    {
        AnimatedSprite.Animation = "up";
        AnimatedSprite.FlipV = direction > 0;
    }
}