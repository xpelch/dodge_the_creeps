using Godot;
using System;

public partial class Main : Node
{
    [Export] public PackedScene MobScene { get; set; }

    private int _score;
    private AudioStreamPlayer _music;
    private AudioStreamPlayer _deathSound;
    private HUD _hud;
    private Timer _mobTimer;
    private Timer _scoreTimer;
    private Timer _startTimer;
    private Player _player;
    private Marker2D _startPosition;
    private PathFollow2D _mobSpawnLocation;
    
    public override void _Ready()
    {
        _music = GetNode<AudioStreamPlayer>("Music");
        _deathSound = GetNode<AudioStreamPlayer>("DeathSound");
        _hud = GetNode<HUD>("HUD");
        _mobTimer = GetNode<Timer>("MobTimer");
        _scoreTimer = GetNode<Timer>("ScoreTimer");
        _startTimer = GetNode<Timer>("StartTimer");
        _player = GetNode<Player>("Player");
        _startPosition = GetNode<Marker2D>("StartPosition");
        _mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
    }
    
    public void NewGame()
    {
        ResetGame();
        StartMusic();
        DisplayStartMessage();
        PreparePlayer();
        _startTimer.Start();
    }
    
    private void ResetGame()
    {
        _score = 0;
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
    }
    
    private void StartMusic()
    {
        _music.Play();
    }

    private void DisplayStartMessage()
    {
        _hud.UpdateScore(_score);
        _hud.ShowMessage("Get Ready!");
    }

    private void PreparePlayer()
    {
        _player.Start(_startPosition.Position);
    }
    
    public void GameOver()
    {
        StopGame();
        _hud.ShowGameOver();
    }
    
    private void StopGame()
    {
        _music.Stop();
        _deathSound.Play();
        _mobTimer.Stop();
        _scoreTimer.Stop();
    }

    private void OnStartTimerTimeout()
    {
        _mobTimer.Start();
        _scoreTimer.Start();
    }
    
    private void OnScoreTimerTimeout()
    {
        _score++;
        _hud.UpdateScore(_score);
    }
    
    private void OnMobTimerTimeout()
    {
        SpawnMob();
    }

    private void SpawnMob()
    {
        Mob mob = MobScene.Instantiate<Mob>();
        SetMobSpawnLocation(mob);
        SetMobDirection(mob);
        SetMobVelocity(mob);
        AddChild(mob);
    }

    private void SetMobSpawnLocation(Mob mob)
    {
        _mobSpawnLocation.ProgressRatio = GD.Randf();
        mob.Position = _mobSpawnLocation.Position;
    }

    private void SetMobDirection(Mob mob)
    {
        float direction = _mobSpawnLocation.Rotation + Mathf.Pi / 2;
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;
    }

    private void SetMobVelocity(Mob mob)
    {
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(mob.Rotation);
    }
}
