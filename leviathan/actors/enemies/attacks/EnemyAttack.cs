using Godot;

public partial class EnemyAttack : Area2D
{

	[Export]
	public bool DestroyOnHitWall = true;

	[Export]
	public bool DestroyOnHitPlayer = true;

	[Export]
	public int Damage = 1;

	public override void _Ready()
	{
		base._Ready();
		BodyEntered += OnHitPlayer;
	}

	protected void OnHitWall(Node2D other)
	{
		if (DestroyOnHitWall)
		{
			OnDestroy();
		}
	}

	protected void OnHitPlayer(Node2D body)
	{
		if (!(body is Player) && !(body is EnemyAttack) && !(body.IsInGroup("enemies")))
		{
			OnHitWall(body);
			return;
		}
		if (body is Player)
		{
			Player player = (Player)body;
			player.TakeDamage(Damage, this);
			if (DestroyOnHitPlayer)
			{
				OnDestroy();
			}
		}

	}

	protected virtual void OnDestroy()
	{
		// play animation here or something
		QueueFree();
	}
}