using System.Threading.Tasks;
using Godot;

public partial class Fish : Enemy
{
	public Node2D Target;
	public Node2D Sprite;

	float KnockbackSpeed = 300f;

	LeviathanPropulsionBehavior MovementBehavior;

	[Export]
	PackedScene DeathAnimation;

	[Export]
	PackedScene HPPickup;

	public override void _Ready()
	{
		base._Ready();
		Sprite = GetNode<Node2D>("fish_sprite");
		MovementBehavior = new LeviathanPropulsionBehavior(Target);

		HP = 2;
		MaxHP = 2;

		MovementBehavior.ShouldWander = true;
		MovementBehavior.TimeBetweenJets = 1.5f;
		MovementBehavior.JetAcceleration = 200f;
	}

	public override void _Process(double delta)
	{
		var scale = Sprite.Scale;
		if (Velocity.X > 0.1)
		{
			scale.X = 1;
		}
		if (Velocity.X < -0.1)
		{
			scale.X = -1;
		}
		Sprite.Scale = scale;
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		switch (State)
		{
			case ACTOR_STATES.HURT:
			case ACTOR_STATES.DEAD:
				break;
			default:
				MovementBehavior.Update(this, delta);
				break;
		}
		MoveAndSlide();
	}

	protected async override Task OnTakeDamage(int damage, Node2D source = null)
	{
		// jump a little
		var vel = Velocity;
		if (Player.GetPlayer(this) != null)
		{
			Vector2 TargetDirection = Player.GetPlayer(this).GlobalPosition.DirectionTo(GlobalPosition);
			vel = TargetDirection * KnockbackSpeed;
		}
		Velocity = vel;
		AudioStreamManager.Get(this).Play("res://assets/sfx/fish_hit.wav", -9);

		HurtBehavior();

		await base.OnTakeDamage(damage, source);
	}

	protected async void HurtBehavior()
	{
		SetState(ACTOR_STATES.HURT);
		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		SetState(ACTOR_STATES.DEFAULT);
	}

	protected override void OnDie()
	{
		AudioStreamManager.Get(this).Play("res://assets/sfx/fish_hit.wav", -9);
		OnDieAnimation();

	}

	protected async void OnDieAnimation()
	{
		var scale = Sprite.Scale;
		Rotate(Mathf.DegToRad(scale.X > 0 ? -20 : 20));
		flashEffect.SetFlash(true);
		
		Velocity = new Vector2(scale.X * 50, -50);
		await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
		if (DeathAnimation != null)
		{
			var anim = DeathAnimation.Instantiate<Node2D>();
			anim.Position = Position;
			GetParent().AddChild(anim);
		}

		if (Player.GetPlayer(this) != null && HPPickup != null) {
			Player player = Player.GetPlayer(this);
			if (player.HP < player.MaxHP - 4) {
				// spawn a health pickup with 1/3 chance
				float rand = GD.Randf();
				if (rand < 0.34 || GetTree().GetNodesInGroup("leviathan").Count > 0) {
					var hpPickup = HPPickup.Instantiate<HPPickup>();
					hpPickup.Position = Position;
					GetParent().AddChild(hpPickup);
				}
			}
		}
		AudioStreamManager.Get(this).Play("res://assets/sfx/fish_die.wav", -9);
		base.OnDie();
	}
}