using Godot;

public partial class LeviathanChargeTarget : Node2D {

	Vector2 OriginalPosition;
	Player Player;
	public override void _Ready()
	{
		base._Ready();
		OriginalPosition = GlobalPosition;

		Player = Player.GetPlayer(this);

	}
	public override void _Process(double delta)
	{
		base._Process(delta);
		// if a player exists, follow the player
		if (Player == null) {
			Player = Player.GetPlayer(this);
		}
		if (Player == null) {
			// otherwise, center of the room (original position)
			GlobalPosition = OriginalPosition;
			return;
		}
		if (!Node.IsInstanceValid(Player)) {
			Player = null;
			return;
		}
		if (!Player.IsInsideTree()) {
			Player = null;
			return;
		}
		GlobalPosition = Player.GlobalPosition;
	}
}