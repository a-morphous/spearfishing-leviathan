using Godot;

public class LeviathanShootingBehavior : IActorBehavior
{
	public float TimeBetweenAttacks = 2f; // in seconds

	float _attackTime = 0;
	float _internalRotation = 0;

	public float RotationIncrease = Mathf.DegToRad(10);

	PackedScene Bullet;

	public LeviathanShootingBehavior(PackedScene bulletScene) {
		this.Bullet = bulletScene;
	}

	float[] _bulletAngles = new float[15]{
			Mathf.DegToRad(24),
			Mathf.DegToRad(48),
			Mathf.DegToRad(72),
			Mathf.DegToRad(96),
			Mathf.DegToRad(120),
			Mathf.DegToRad(144),
			Mathf.DegToRad(168),
			Mathf.DegToRad(192),
			Mathf.DegToRad(216),
			Mathf.DegToRad(240),
			Mathf.DegToRad(264),
			Mathf.DegToRad(288),
			Mathf.DegToRad(312),
			Mathf.DegToRad(336),
			Mathf.DegToRad(360),
		};
	public void Update(Actor actor, double delta)
	{
		Vector2 velocity = actor.Velocity;
		_attackTime -= (float)delta;
		if (_attackTime <= 0)
		{
			_attackTime = TimeBetweenAttacks;

			AudioStreamManager.Get(actor).Play("res://assets/sfx/leviathan_drone.wav", -6);
			// attack

			for (var i = 0; i < _bulletAngles.Length; i++)
			{
				var _bullet = Bullet.Instantiate<EnemyBullet>();
				_bullet.Rotation = _internalRotation + _bulletAngles[i];
				_bullet.BulletGravity = 0;
				_bullet.Speed = 350;
				_bullet.Position = actor.Position + Vector2.FromAngle(_bullet.Rotation) * 30;
				actor.GetParent().AddChild(_bullet);
			}
			_internalRotation += RotationIncrease;
		}
	}
}