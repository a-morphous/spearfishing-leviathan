using Godot;

public partial class MouseTarget : Node2D {

	public override void _Process(double delta)
	{
		base._Process(delta);
		GlobalPosition = GetGlobalMousePosition();
	}
}