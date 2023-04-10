using Godot;
using System;

public partial class VisualWaterSpring : Node2D
{

	public float Velocity = 0;
	float Force = 0;
	public float Height = 0;
	float Target_Height = 0;

	Area2D Area;
	CollisionShape2D Collider;

	int index = 0;

	float MotionFactor = 0.015f;
	CharacterBody2D CurrentBody;

	[Signal]
	public delegate void SplashEventHandler(int index, float speed);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Area = GetNode<Area2D>("Area2D");
		Collider = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");

		Area.BodyEntered += OnBodyEntered;
	}

	public void WaterUpdate(float SpringConstant, float Dampening) {
		// This function applies the hooke's law force to the spring!!
		// This function will be called in each frame
		// hooke's law ---> F =  - K * x 
	
		Height = Position.Y;
		var X = Height - Target_Height;
		
		var Loss = -Dampening * Velocity;

		Force = -SpringConstant * X + Loss;
		Velocity += Force;

		var position = Position;

		Velocity = Mathf.Clamp(Velocity, -300, 300);

		position.Y += Velocity;
		Position = position;
	}

	public void Initialize(float XPosition, int Id) {
		Height = Position.Y;
		Target_Height = Position.Y;
		Velocity = 0;
		Position = new Vector2(XPosition, Position.Y);
		index = Id;
	}

	public void SetCollisionWidth(float SpaceBetween) {
		RectangleShape2D Shape = (RectangleShape2D)Collider.Shape;
		var Extents = Shape.Size;
		var NewExtents = new Vector2(SpaceBetween/2, Extents.Y);
		Shape.Size = NewExtents;
	}
	protected void OnBodyEntered(Node2D Body) {
		if (!(Body is CharacterBody2D)) {
			return;
		}
		
		CharacterBody2D Actor = (CharacterBody2D)Body;
		
		var Speed = Actor.Velocity.Y * MotionFactor;

		EmitSignal(SignalName.Splash, index, Speed);
	}
}
