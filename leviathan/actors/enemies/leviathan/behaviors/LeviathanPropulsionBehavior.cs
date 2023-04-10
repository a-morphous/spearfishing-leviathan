using Godot;

public class LeviathanPropulsionBehavior : IActorBehavior
{
	public Node2D Target;
	public float TimeBetweenJets = 4f; // in seconds
	public float JetAcceleration = 600f;
	public float Decay = 0.97f;
	public bool ShouldWander = false;
	public float WanderRadius = 120f;

	public float _jetTime = 0;
	public LeviathanPropulsionBehavior(Node2D target)
	{
		Target = target;
	}

	public void Update(Actor actor, double delta)
	{
		if (Target != null && !Node.IsInstanceValid(Target)) {
			Target = null;
		}
		Vector2 velocity = actor.Velocity;
		if (_jetTime <= 0)
		{
			_jetTime = TimeBetweenJets;
			Vector2 DesiredDirection;
			if (Target == null || (ShouldWander && actor.GlobalPosition.DistanceTo(Target.GlobalPosition) < WanderRadius))
			{
				// wander
				DesiredDirection = Vector2.FromAngle(GD.Randf() * Mathf.Pi * 2);
			}
			else
			{
				DesiredDirection = actor.GlobalPosition.DirectionTo(Target.GlobalPosition);
			}

			velocity = JetAcceleration * DesiredDirection;
		}
		else
		{
			velocity *= Decay;
			_jetTime -= (float)delta;
		}
		actor.Velocity = velocity;
	}
}