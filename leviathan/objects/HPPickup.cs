using Godot;

public partial class HPPickup : Area2D {

	public int RecoveryAmount = 3;
	public override void _Ready()
	{
		base._Ready();
		BodyEntered += OnPickup;
	}

	protected void OnPickup(Node2D body) {
		if (body is Player) {
			Player player = (Player) body;
			player.Heal(RecoveryAmount);
			AudioStreamManager.Get(this).Play("res://assets/sfx/pickupCoin.wav", -12);
			QueueFree();
		}
	}
}