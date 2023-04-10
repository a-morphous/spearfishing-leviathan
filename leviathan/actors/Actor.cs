using Godot;
using System;
using System.Threading.Tasks;

public partial class Actor : CharacterBody2D
{
	[Signal]
	public delegate void TookDamageEventHandler(Actor self, int damage);
	[Signal]
	public delegate void HealedEventHandler(Actor self, int hp);
	[Signal]
	public delegate void DiedEventHandler(Actor self);

	public enum ACTOR_STATES
	{
		DEFAULT,
		DASHING,
		SWIMMING,
		SWIM_DASHING,
		HURT,
		DEAD,

		// attack states
		ATTACKING,

		SWIM_ATTACKING,
	}

	public float GravityFactor = 1f;

	public bool IsInWater = false;

	public ACTOR_STATES PreviousState { get; protected set; } = ACTOR_STATES.DEFAULT;

	public ACTOR_STATES State { get; protected set; } = ACTOR_STATES.DEFAULT;

	// hp
	public int HP = 3;
	public int MaxHP = 3;
	public bool IsInvincible = false;
	protected float InvincibleTimer = 0.3f;

	public virtual void SetState(ACTOR_STATES newState)
	{
		if (State == newState)
		{
			return;
		}
		PreviousState = State;
		State = newState;
	}

	public void Heal(int hp) {
		HP = Math.Clamp(HP + hp, 0, MaxHP);
		EmitSignal(SignalName.Healed, this, hp);
	}

	public async void TakeDamage(int damage, Node2D source = null)
	{
		if (IsInvincible || State == ACTOR_STATES.DEAD)
		{
			return;
		}
		HP = Math.Clamp(HP - damage, 0, MaxHP);
		if (HP <= 0)
		{
			SetState(ACTOR_STATES.DEAD);
			EmitSignal(SignalName.Died, this);
			OnDie();
		}
		else
		{
			EmitSignal(SignalName.TookDamage, this, damage);
			await OnTakeDamage(damage, source);
		}


	}

	// can be overridden by inherited classes to have different behaviors
	protected virtual async Task OnTakeDamage(int damage, Node2D source = null)
	{
		// override to do more interesting things
		IsInvincible = true;

		await (ToSignal(GetTree().CreateTimer(InvincibleTimer), SceneTreeTimer.SignalName.Timeout));
		IsInvincible = false;
		return;
	}

	protected virtual void OnDie()
	{
		// override to actually play animations

		QueueFree();

	}
}