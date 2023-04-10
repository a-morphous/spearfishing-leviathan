using Godot;
using System;

public partial class WaterBody : Area2D
{
	Actor _Actor;
	AudioStreamManager AudioManager;
	bool _PrevInWater = false;
	// Called when the node enters the scene tree for the first time.

	float CheckTimer = 0.1f;
	float _currentTimer = 0f;
	public override void _Ready()
	{
		AudioManager = AudioStreamManager.Get(this);
		this._Actor = GetParent<Actor>();
		this.AreaEntered += Submerged;
		this.AreaExited += Surfaced;
	}

	// dumb dumb dumb dumb dumb
	public override void _Process(double delta)
	{
		_currentTimer -= (float)delta;
		if (_currentTimer <= 0)
		{

			CheckWaterState();
			_currentTimer = CheckTimer;
		}
	}

	public void CheckWaterState()
	{
		if (_Actor.IsInWater != _PrevInWater)
		{
			AudioManager.Play("res://assets/sfx/splash.wav", -6);
		}
		_PrevInWater = _Actor.IsInWater;
	}

	public void Submerged(Area2D other)
	{
		_Actor.IsInWater = true;
	}

	public void Surfaced(Area2D other)
	{
		_Actor.IsInWater = false;
	}
}
