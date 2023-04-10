using Godot;

public partial class LeviathanChargeTest : Actor {
	[Export]
	Node2D Target;
	LeviathanChargeBehavior ChargeBehavior;

	public override void _Ready()
	{
		base._Ready();
		ChargeBehavior = new LeviathanChargeBehavior(Target);
	}

	public override void _PhysicsProcess(double delta)
	{
		ChargeBehavior.Update(this, delta);
		MoveAndSlide();
		QueueRedraw();
	}

	public override void _Draw()
	{
		base._Draw();
		DrawLine(new Vector2(), Vector2.FromAngle(ChargeBehavior.CurrentAngle) * 100, new Color (1,1,1,1), 15);
	}
}