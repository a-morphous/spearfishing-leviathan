using Godot;

public partial class EnemyDamagePlayerArea : Area2D {

	[Export]
	public Enemy Enemy;

	public override void _Ready()
	{
		base._Ready();
		if (Enemy == null) {
			Enemy = NodeUtils.GetNearestAncestorNode<Enemy>(this);
		}
		if (Enemy == null) {
			GD.Print(this, " was created with no enemy ancestor, disabling");
			Monitoring = false;
		}
	}

	public override void _Process(double delta)
	{
		if (!Monitoring)
		{
			return;
		}
		foreach (var body in GetOverlappingBodies())
		{
			OnCollidedWithPlayer(body);
		}
	}

	protected void OnCollidedWithPlayer(Node2D body)
	{
		if (!body.IsInGroup("player"))
		{
			return;
		}
		if (Enemy.HP <= 0) {
			return; // enemy is dead
		}
		Player player = (Player)body;
		player.TakeDamage(Enemy.DamageOnContact, this);
	}
}