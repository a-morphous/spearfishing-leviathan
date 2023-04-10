using Godot;

public class PlayerAnimationBehavior : IActorBehavior
{
	Vector2 velocity;

	// used for sounds
	bool _PrevOnFloor = true;

	public void Update(Actor actor, double delta)
	{
		if (!(actor is Player))
		{
			return;
		}

		Player player = (Player)actor;
		PlayerSprite _sprite = player.Sprite;
		velocity = actor.Velocity;

		var scale = _sprite.Scale;
		var baseScale = Mathf.Abs(_sprite.Scale.X);
		var isMoving = false;
		var attackScale = player.AttackColliders.Scale;
		if (player.State != Actor.ACTOR_STATES.HURT && player.State != Actor.ACTOR_STATES.DEAD)
		{
			if (velocity.X > 0.01)
			{
				scale.X = baseScale;
				isMoving = true;
				attackScale.X = 1;

			}
			if (velocity.X < -0.01)
			{
				scale.X = -baseScale;
				isMoving = true;
				attackScale.X = -1;
			}

		}

		_sprite.Scale = scale;
		player.AttackColliders.Scale = attackScale;

		// check various states and see which animations should play
		switch (player.State)
		{
			case Actor.ACTOR_STATES.DEFAULT:
				// not swimming.
				// if we're on the ground and not moving, idle. Otherwise run.
				if (player.IsOnFloor())
				{
					if (!_PrevOnFloor) {
						AudioStreamManager.Get(player).Play("res://assets/sfx/footstep05.ogg", -7);
					}
					if (isMoving)
					{
						_sprite.Play(PlayerAnimations.GroundRun);
					}
					else
					{
						_sprite.Play(PlayerAnimations.GroundIdle);
					}
				}
				else
				{
					if (velocity.Y < 0)
					{
						_sprite.Play(PlayerAnimations.JumpRising);
					}
					else
					{
						// custom blend is in seconds
						_sprite.Play(PlayerAnimations.JumpFalling, 0.2f);
					}
				}
				break;
			case Actor.ACTOR_STATES.SWIMMING:
				if (player.IsInWater)
				{
					_sprite.Play(PlayerAnimations.Floating);
				}
				else
				{
					if (velocity.Y < 0)
					{
						_sprite.Play(PlayerAnimations.JumpRising);
					}
					else
					{
						// custom blend is in seconds
						_sprite.Play(PlayerAnimations.JumpFalling, 0.2f);
					}
				}

				break;
		}
		_PrevOnFloor = player.IsOnFloor();
	}

}