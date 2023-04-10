using Godot;
using System;

public partial class OneshotParticleEffect : Node2D
{
	// Called when the node enters the scene tree for the first time.
	Timer timer;
	GpuParticles2D particles;
	public override void _Ready()
	{
		particles = GetNode<GpuParticles2D>("GPUParticles2D");
		particles.Emitting = true;
		timer = GetNode<Timer>("Timer");
		timer.Timeout += () => {
			QueueFree();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
