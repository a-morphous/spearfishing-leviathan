using Godot;
using System;

public partial class Head : Area2D
{
	public bool CanBreathe = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.AreaEntered += Submerged;
		this.AreaExited += Surfaced;
	}

	public void Submerged(Area2D other)
	{
		if (other.IsInGroup("water"))
		{
			CanBreathe = false;
		}

	}

	public void Surfaced(Area2D other)
	{
		if (other.IsInGroup("water"))
		{
			CanBreathe = true;
		}
	}
}
