using Godot;
using System;

public struct PlayerAnimations
{
	public const string GroundIdle = "Ground_Idle";
	public const string GroundRun = "Ground_Run";
	public const string JumpRising = "Jump_Rising";
	public const string JumpFalling = "Jump_Falling";

	public const string Floating = "Floating";

	// attacks
	public const string GroundAttack = "Ground_Attack";
	public const string AerialAttack = "Aerial_Attack";
	public const string FloatAttackForward = "Float_Attack_Forward";
	public const string FloatAttackUpForward = "Float_Attack_UpForward";
	public const string FloatAttackDownForward = "Float_Attack_DownForward";
	public const string FloatAttackUp = "Float_Attack_Up";
	public const string FloatAttackDown = "Float_Attack_Down";
}

public partial class PlayerSprite : Node2D
{
	[Signal]
	public delegate void OnAttackEventHandler(string AnimationName);

	public AnimationPlayer Animations { get; protected set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Animations = GetNode<AnimationPlayer>("AnimationPlayer");

	}

	public bool IsPlaying(string animationName)
	{
		if (!Animations.IsPlaying())
		{
			return false;
		}
		if (Animations.CurrentAnimation != animationName)
		{
			return false;
		}
		return true;
	}

	public void Play(string animationName)
	{
		if (IsPlaying(animationName))
		{
			return;
		}
		Animations.Play(animationName);
	}

	public void Play(string animationName, float customBlend)
	{
		if (IsPlaying(animationName))
		{
			return;
		}
		Animations.Play(animationName, customBlend);
	}

	public void FireAttackSignal()
	{
		EmitSignal(SignalName.OnAttack, Animations.CurrentAnimation);
	}

	public void PlayFootstep()
	{
		var coinflip = GD.Randi() % 2;
		if (coinflip == 1)
		{
			AudioStreamManager.Get(this).Play("res://assets/sfx/footstep05.ogg", -20);
		}
		else
		{
			AudioStreamManager.Get(this).Play("res://assets/sfx/footstep09.ogg", -20);
		}

	}
}
