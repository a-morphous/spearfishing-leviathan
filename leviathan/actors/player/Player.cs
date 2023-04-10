using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using deVoid.Utils;

public class PlayerCreatedSignal : ASignal<Player> { };

public partial class Player : Actor
{
	[Export]
	PackedScene DeathExplosion;

	// states and behaviors
	ActorGravityBehavior _gravityBehavior;
	PlayerMovementBehavior _movementBehavior;
	PlayerJumpBehavior _jumpBehavior;
	PlayerDashBehavior _dashBehavior;
	PlayerCanAttackBehavior _attackBehavior;

	// swimming
	PlayerToggleSwimBehavior _toggleSwimBehavior;
	PlayerSwimmingBehavior _swimmingBehavior;
	PlayerSwimDashBehavior _swimDashBehavior;

	PlayerAnimationBehavior _animationBehavior;

	Dictionary<string, AttackArea> _attackColliders;

	public Head playerHead { get; protected set; }
	FlashOnHurt flashEffect;

	public PlayerSprite Sprite { get; protected set; }
	public Node2D AttackColliders { get; protected set; }

	AudioStreamManager AudioManager;
	AudioStreamPlayer SwimmingLoop;

	public override void _Ready()
	{
		AudioManager = AudioStreamManager.Get(this);
		SwimmingLoop = GetNode<AudioStreamPlayer>("Audio/SwimLoop");
		SwimmingLoop.StreamPaused = true;

		_gravityBehavior = new ActorGravityBehavior();
		_jumpBehavior = new PlayerJumpBehavior();
		_movementBehavior = new PlayerMovementBehavior();
		_dashBehavior = new PlayerDashBehavior();
		_swimDashBehavior = new PlayerSwimDashBehavior();
		_toggleSwimBehavior = new PlayerToggleSwimBehavior();
		_swimmingBehavior = new PlayerSwimmingBehavior();
		_animationBehavior = new PlayerAnimationBehavior();
		_attackBehavior = new PlayerCanAttackBehavior();

		playerHead = GetNode<Head>("Head");
		Sprite = GetNode<PlayerSprite>("PlayerSprite");

		AttackColliders = GetNode<Node2D>("Attacks");

		_attackColliders = new Dictionary<string, AttackArea>() {
			{PlayerAnimations.FloatAttackDown, AttackColliders.GetNode<AttackArea>("FloatAttackDown")},
			{PlayerAnimations.FloatAttackDownForward, AttackColliders.GetNode<AttackArea>("FloatAttackDownForward")},
			{PlayerAnimations.FloatAttackForward, AttackColliders.GetNode<AttackArea>("FloatAttackForward")},
			{PlayerAnimations.FloatAttackUpForward, AttackColliders.GetNode<AttackArea>("FloatAttackUpForward")},
			{PlayerAnimations.FloatAttackUp, AttackColliders.GetNode<AttackArea>("FloatAttackUp")},
			{"default", AttackColliders.GetNode<AttackArea>("RegularAttack")},
		};

		Sprite.OnAttack += OnHandleAttackColliders;
		Sprite.Animations.AnimationFinished += OnFinishedAttackAnimation;

		flashEffect = new FlashOnHurt(this);

		InvincibleTimer = 2f;

		// testing
		HP = 10;
		MaxHP = 10;

		AddToGroup("player");
		Signals.Get<PlayerCreatedSignal>().Dispatch(this);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (IsInWater)
		{
			if (Velocity.Length() > 0.5f)
			{
				SwimmingLoop.StreamPaused = false;
			}
			else
			{
				SwimmingLoop.StreamPaused = true;
			}

		}
		else if (!IsInWater && !SwimmingLoop.StreamPaused)
		{
			SwimmingLoop.StreamPaused = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		switch (State)
		{
			case ACTOR_STATES.ATTACKING:
				_gravityBehavior.Update(this, delta);
				_jumpBehavior.Update(this, delta);
				_movementBehavior.Update(this, delta);
				break;
			case ACTOR_STATES.DASHING:
				_dashBehavior.Update(this, delta);
				_attackBehavior.Update(this, delta);
				break;
			case ACTOR_STATES.HURT:
				_gravityBehavior.Update(this, delta);
				_movementBehavior.Update(this, delta);
				break;
			case ACTOR_STATES.SWIMMING:
				if (!IsInWater)
				{
					_gravityBehavior.Update(this, delta);
				}
				_toggleSwimBehavior.Update(this, delta);
				_swimmingBehavior.Update(this, delta);
				_attackBehavior.Update(this, delta);
				_swimDashBehavior.Update(this, delta);
				break;
			case ACTOR_STATES.SWIM_ATTACKING:
				_swimmingBehavior.Update(this, delta);
				break;
			case ACTOR_STATES.SWIM_DASHING:
				_swimDashBehavior.Update(this, delta);
				_attackBehavior.Update(this, delta);
				break;
			case ACTOR_STATES.DEFAULT:
				_gravityBehavior.Update(this, delta);
				_jumpBehavior.Update(this, delta);
				_movementBehavior.Update(this, delta);
				_toggleSwimBehavior.Update(this, delta);
				_dashBehavior.Update(this, delta);
				_attackBehavior.Update(this, delta);
				break;
		}

		_animationBehavior.Update(this, delta);

		MoveAndSlide();
	}

	public override void SetState(ACTOR_STATES newState)
	{
		if (newState == ACTOR_STATES.SWIMMING)
		{
			// we need to reset the jump otherwise the desiredJump is still true, and we'll try to jump as soon as we
			// hit dry land
			_jumpBehavior.Reset();
		}

		base.SetState(newState);
	}

	protected void OnHandleAttackColliders(string AnimationName)
	{
		if (_attackColliders.ContainsKey(AnimationName))
		{
			_attackColliders[AnimationName].Activate();
		}
		else
		{
			_attackColliders["default"].Activate();
		}
	}


	protected void OnFinishedAttackAnimation(StringName AnimationName)
	{
		if (State == ACTOR_STATES.ATTACKING)
		{
			SetState(ACTOR_STATES.DEFAULT);
		}
		if (State == ACTOR_STATES.SWIM_ATTACKING)
		{
			SetState(ACTOR_STATES.SWIMMING);
		}
	}

	protected async override Task OnTakeDamage(int damage, Node2D source = null)
	{
		AudioManager.Play("res://assets/sfx/robot_hit.wav", -12);
		// jump a little
		var vel = Velocity;
		vel.Y = -300;
		if (source != null)
		{
			if (source.GlobalPosition.X > GlobalPosition.X)
			{
				vel.X = -300;
			}
			else if (source.GlobalPosition.X < GlobalPosition.X)
			{
				vel.X = 300;
			}
		}

		HurtBehavior();

		Velocity = vel;
		IsInvincible = true;
		await (ToSignal(GetTree().CreateTimer(InvincibleTimer), SceneTreeTimer.SignalName.Timeout));
		IsInvincible = false;
	}

	protected async void HurtBehavior()
	{
		SetState(ACTOR_STATES.HURT);
		flashEffect.SetFlash(true);
		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		if (PreviousState == ACTOR_STATES.SWIM_ATTACKING || PreviousState == ACTOR_STATES.SWIMMING || PreviousState == ACTOR_STATES.SWIM_DASHING)
		{
			SetState(ACTOR_STATES.SWIMMING);
		}
		else
		{
			SetState(ACTOR_STATES.DEFAULT);
		}

		flashEffect.SetFlash(false);
	}

	// global get player
	public static Player GetPlayer(Node searcher)
	{
		var PossiblePlayers = searcher.GetTree().GetNodesInGroup("player");
		if (PossiblePlayers.Count == 0)
		{
			return null;
		}
		return (Player)PossiblePlayers[0];
	}

	protected override void OnDie()
	{
		if (DeathExplosion != null)
		{
			var explosion = DeathExplosion.Instantiate<Node2D>();
			explosion.Position = Position;
			GetParent().AddChild(explosion);
		}
		AudioStreamManager.Get(this).Play("res://assets/sfx/jumpscare_gameover.wav", 0);
		base.OnDie();
	}
}
