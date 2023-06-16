using Godot;
using System;

public partial class Mob : RigidBody2D
{
    private AnimatedSprite2D AnimatedSprite { get; set; }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        PlayRandomAnimation();
    }

    private void PlayRandomAnimation()
    {
        var mobTypes = AnimatedSprite.SpriteFrames.GetAnimationNames();
        var randomIndex = GD.Randi() % mobTypes.Length;
        AnimatedSprite.Play(mobTypes[randomIndex]);
    }

    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}