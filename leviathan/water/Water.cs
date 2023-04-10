using Godot;
using System;

public partial class Water : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("water");
	}
}
