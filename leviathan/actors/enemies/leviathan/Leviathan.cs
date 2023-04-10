using Godot;
using deVoid.Utils;
using System.Threading.Tasks;

public class LeviathanEnterSignal : ASignal { }

public partial class Leviathan : Enemy
{
	public enum LEVIATHAN_STATES
	{
		INERT, // when the player hasn't gone into the room yet
		INTRO,
		SHOOTING_BULLETS, // Sinks down, then swims around and fires bullets in a spray pattern
		NIGHTMARE_CHARGE, // Charges at you (kind of like fusion nightmare's wobbly ram attack)
		ROARING, // Summons smaller enemies with a roar,
		IDLE,
		SWIMMING, // Swimming around kind of slowly (easiest to hit)
	}

	public LEVIATHAN_STATES LeviathanState = LEVIATHAN_STATES.INERT;

	LeviathanChargeBehavior ChargeBehavior;
	LeviathanPropulsionBehavior PropelBehavior;
	LeviathanShootingBehavior ShootingBehavior;
	LeviathanRoaringBehavior RoaringBehavior;
	LeviathanChangeStateBehavior ChangeStateBehavior;

	[Export]
	LeviathanChargeTarget ChargeTarget;

	[Export]
	AnimationPlayer IntroAnimationPlayer;

	[Export]
	Node2D WeakPoint;

	[Export]
	PackedScene Bullet;

	[Export]
	PackedScene MinionFish;

	[Export]
	Control FishSpawnArea;

	LeviathanSprite Sprite;

	float KnockbackSpeed = 500f;

	public override void _Ready()
	{
		base._Ready();
		HP = 20;
		MaxHP = 20;

		AddToGroup("leviathan");

		Sprite = GetNode<LeviathanSprite>("leviathan_sprite");
		Sprite.DeathAnimationFinished += OnDecay;

		ChangeStateBehavior = new LeviathanChangeStateBehavior();
		RoaringBehavior = new LeviathanRoaringBehavior(MinionFish, FishSpawnArea);
		ChargeBehavior = new LeviathanChargeBehavior(ChargeTarget);
		PropelBehavior = new LeviathanPropulsionBehavior(ChargeTarget);
		PropelBehavior.JetAcceleration = 700f;
		ShootingBehavior = new LeviathanShootingBehavior(Bullet);

		Signals.Get<LeviathanEnterSignal>().AddListener(Introduction);
	}

	protected void Introduction()
	{
		Visible = true;
		this.LeviathanState = LEVIATHAN_STATES.INTRO;
		IsInvincible = true;
		if (IntroAnimationPlayer != null)
		{
			IntroAnimationPlayer.Play("Intro");
			IntroAnimationPlayer.AnimationFinished += OnEndIntro;
		}
		Signals.Get<LeviathanEnterSignal>().RemoveListener(Introduction);
	}

	protected void OnEndIntro(StringName Animation)
	{
		if (Animation == "Intro")
		{
			IntroAnimationPlayer.AnimationFinished -= OnEndIntro;
		}
		LeviathanState = LEVIATHAN_STATES.NIGHTMARE_CHARGE;
		IsInvincible = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		switch (State)
		{
			case ACTOR_STATES.HURT:
				break;
			case ACTOR_STATES.DEAD:
				break;
			default:
				HandleLeviathanState(delta);
				break;
		}
		MoveAndSlide();
	}

	public void SetLeviathanState(LEVIATHAN_STATES newState)
	{
		switch (newState)
		{
			case LEVIATHAN_STATES.ROARING:
				AudioStreamManager.Get(this).Play("res://assets/sfx/leviathan_change.wav", -6);
				Sprite.Play("Roaring", 2f);
				RoaringBehavior.Reset();
				break;
			case LEVIATHAN_STATES.NIGHTMARE_CHARGE:
			case LEVIATHAN_STATES.IDLE:
				AudioStreamManager.Get(this).Play("res://assets/sfx/leviathan_change.wav", -6);
				var scale = Scale;
				scale.X = -scale.X;
				Scale = scale;
				Sprite.Play("Idle", 1f);
				break;
			case LEVIATHAN_STATES.SWIMMING:
				AudioStreamManager.Get(this).Play("res://assets/sfx/leviathan_change.wav", -6);
				PropelBehavior._jetTime = 3f;
				Sprite.Play("Swim", 2f);
				break;
			case LEVIATHAN_STATES.SHOOTING_BULLETS:
				AudioStreamManager.Get(this).Play("res://assets/sfx/leviathan_change.wav", -6);
				Sprite.Play("Curl", 1f);
				break;
		}
		LeviathanState = newState;
	}

	protected void HandleLeviathanState(double delta)
	{
		switch (LeviathanState)
		{
			case LEVIATHAN_STATES.INERT:
				Visible = false;
				break;
			case LEVIATHAN_STATES.INTRO:
				break;
			case LEVIATHAN_STATES.NIGHTMARE_CHARGE:
				ChargeBehavior.Update(this, delta);
				ChangeStateBehavior.Update(this, delta);
				break;
			case LEVIATHAN_STATES.SWIMMING:
				PropelBehavior.Update(this, delta);
				ChangeStateBehavior.Update(this, delta);
				break;
			case LEVIATHAN_STATES.IDLE:
				Velocity = Vector2.Zero;
				ChangeStateBehavior.Update(this, delta);
				break;
			case LEVIATHAN_STATES.SHOOTING_BULLETS:
				Velocity = Vector2.Zero;
				ShootingBehavior.Update(this, delta);
				ChangeStateBehavior.Update(this, delta);
				break;
			case LEVIATHAN_STATES.ROARING:
				Velocity = Vector2.Zero;
				RoaringBehavior.Update(this, delta);
				ChangeStateBehavior.Update(this, delta);
				break;
		}
	}

	protected async override Task OnTakeDamage(int damage, Node2D source = null)
	{
		AudioStreamManager.Get(this).Play("res://assets/sfx/monster_hit_2.wav", -6);
		if (LeviathanState != LEVIATHAN_STATES.SWIMMING)
		{
			// jump a little
			var vel = Velocity;
			if (WeakPoint != null && Player.GetPlayer(this) != null)
			{
				Vector2 TargetDirection = Player.GetPlayer(this).GlobalPosition.DirectionTo(WeakPoint.GlobalPosition);
				vel = TargetDirection * KnockbackSpeed;
			}
			Velocity = vel;
		}


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
		Velocity = Vector2.Zero;
		// kill all fish
		var Enemies = GetTree().GetNodesInGroup("enemies");
		foreach (var enemy in Enemies)
		{
			if (enemy is Fish)
			{
				Fish fish = (Fish)enemy;
				fish.HP = 0;
			}
		}
		DeathBehavior();
	}

	protected async void DeathBehavior()
	{
		AudioStreamManager.Get(this).Play("res://assets/sfx/monster_hit_2.wav", -6);
		flashEffect.SetFlash(true);
		GetTree().Paused = true;
		await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
		GetTree().Paused = false;
		Sprite.Play("Die", 1f);
		AudioStreamManager.Get(this).Play("res://assets/sfx/leviathan_roar.wav", -6);
	}

	protected void OnDecay()
	{
		AnimationPlayer player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.Play("Decay");
		player.AnimationFinished += (StringName animation) =>
		{
			QueueFree();
		};
	}
}