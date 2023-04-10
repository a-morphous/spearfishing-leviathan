using Godot;

public partial class FishTarget : Node2D {
	public FishSpawner Home;
	Player player;

	public override void _Ready()
	{
		base._Ready();
	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		if (player == null) {
			GlobalPosition = Home.GlobalPosition;
		}
		else {
			GlobalPosition = player.GlobalPosition;
		}
		
	}

	public void OnPlayerEntered(Node2D body) {
		if (body.IsInGroup("player")) {
			player = (Player)body;
		}
	}

	public void OnPlayerExited(Node2D body) {
		if (body.IsInGroup("player")) {
			player = null;
		}
	}
}