using Godot;

public class PlayerCanAttackBehavior : IActorBehavior
{
	public void Update(Actor actor, double delta)
	{
		if (!(actor is Player))
		{
			return;
		}

		Player player = (Player)actor;
		PlayerSprite _sprite = player.Sprite;

		if (Input.IsActionJustPressed("attack"))
		{
			AudioStreamManager.Get(actor).Play("res://assets/sfx/sword_swing.wav", -18);
			var velocity = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			if (actor.State == Actor.ACTOR_STATES.SWIMMING) {
				var velX = Mathf.Abs(velocity.X);
				if (velocity.Y < -0.5f) {
					// upwards
					if (velX > 0.5f) {
						//diagonal
						_sprite.Play(PlayerAnimations.FloatAttackUpForward);
					} else {
						// directly up
						_sprite.Play(PlayerAnimations.FloatAttackUp);
					}
				} else if (velocity.Y > 0.5f) {
					// downwards
					if (velX > 0.5f) {
						//diagonal
						_sprite.Play(PlayerAnimations.FloatAttackDownForward);
					} else {
						// directly down
						_sprite.Play(PlayerAnimations.FloatAttackDown);
					}
				} else {
					_sprite.Play(PlayerAnimations.FloatAttackForward);
				}

				actor.SetState(Actor.ACTOR_STATES.SWIM_ATTACKING);
				return;
			}

			if (actor.State == Actor.ACTOR_STATES.DEFAULT) {
				if (actor.IsOnFloor()) {
					_sprite.Play(PlayerAnimations.GroundAttack);
				} else {
					_sprite.Play(PlayerAnimations.AerialAttack);
				}
				actor.SetState(Actor.ACTOR_STATES.ATTACKING);
				return;
			}
		}
	}

}
