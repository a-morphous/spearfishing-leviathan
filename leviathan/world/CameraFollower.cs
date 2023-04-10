using Godot;
using System;

public partial class CameraFollower : Node2D
{
	public Node2D ObjectToFollow;
	float JumpCameraThreshold = 200;
	bool FallFollowTriggered = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var _pos = Position;
		if (ObjectToFollow == null)
		{
			return;
		}
		if (!Node.IsInstanceValid(ObjectToFollow)) {
			ObjectToFollow = null;
			return;
		}
		if (!ObjectToFollow.IsInsideTree())
		{
			return;
		}

		if (ObjectToFollow is Actor)
		{
			Actor actor = (Actor)ObjectToFollow;
			if (actor.State == Actor.ACTOR_STATES.SWIMMING || actor.State == Actor.ACTOR_STATES.SWIM_ATTACKING)
			{
				Position = ObjectToFollow.Position;
				return;
			}
			if (actor.IsOnFloor())
			{
				Position = ObjectToFollow.Position;
				FallFollowTriggered = false;
				return;
			}
			_pos.X = ObjectToFollow.Position.X;

			if (FallFollowTriggered || MathF.Abs(ObjectToFollow.Position.Y - Position.Y) > JumpCameraThreshold)
			{
				_pos.Y = ObjectToFollow.Position.Y;
				FallFollowTriggered = true;
			}
		}
		else
		{
			Position = ObjectToFollow.Position;
			return;
		}

		Position = _pos;
	}
}
