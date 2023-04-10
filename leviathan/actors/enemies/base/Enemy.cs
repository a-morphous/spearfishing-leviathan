using Godot;
using System;
using System.Threading.Tasks;

public partial class Enemy : Actor
{
	[Signal]
	public delegate void OnStartTakeDamageEventHandler(Enemy owner);

	[Signal]
	public delegate void OnEndTakeDamageEventHandler(Enemy owner);

	ShaderMaterial _material;
	protected FlashOnHurt flashEffect;

	[Export]
	public int DamageOnContact = 1;

	public override void _Ready()
	{
		AddToGroup("enemies");
		flashEffect = new FlashOnHurt(this);
	}

	protected override async Task OnTakeDamage(int damage, Node2D source = null)
	{
		// TODO: 
		IsInvincible = true;
		flashEffect.SetFlash(true);

		GetTree().Paused = true;
		await ToSignal(GetTree().CreateTimer(0.05), SceneTreeTimer.SignalName.Timeout);
		GetTree().Paused = false;
		flashEffect.SetFlash(false);

		EmitSignal(SignalName.OnStartTakeDamage);
		await base.OnTakeDamage(damage);
		EmitSignal(SignalName.OnEndTakeDamage);
	}
}
