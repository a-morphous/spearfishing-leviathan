using Godot;
using System;
using System.Collections.Generic;

public partial class VisualWater : Node2D
{
	float k = 0.015f;
	float d = 0.05f;
	float spread = 0.2f;

	List<VisualWaterSpring> Springs;
	int passes = 12;

	int DistanceBetweenSprings = 32;
	float WaterLength;
	float WaterHeight;

	Polygon2D WaterPolygon;
	VisualCurve WaterBorder;
	Control VisualWaterBounds;

	float BorderThickness = 1.0f;

	[Export]
	protected PackedScene WaterSpring;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Springs = new List<VisualWaterSpring>();
		WaterPolygon = GetNode<Polygon2D>("WaterBody");
		WaterBorder = GetNode<VisualCurve>("Border");
		VisualWaterBounds = GetNode<Control>("VisualWaterBounds");

		WaterBorder.width = BorderThickness;
		spread = spread / 1000f;

		WaterLength = VisualWaterBounds.Size.X;
		WaterHeight = VisualWaterBounds.Size.Y;

		int SpringNumber = (int)(MathF.Ceiling(WaterLength / DistanceBetweenSprings)) + 1;
		for (var i = 0; i < SpringNumber; i++) {
			float XPosition = DistanceBetweenSprings * i;
			if (i == SpringNumber - 1) {
				XPosition = WaterLength;
			}
			var W = WaterSpring.Instantiate<VisualWaterSpring>();
			AddChild(W);
			Springs.Add(W);
			W.Initialize(XPosition, i);
			W.SetCollisionWidth(DistanceBetweenSprings);
			W.Splash += OnSplash;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach (var spring in Springs) {
			spring.WaterUpdate(k, d);
		}

		float[] LeftDeltas = new float[Springs.Count];
		float[] RightDeltas = new float[Springs.Count];

		for (var i = 0; i < Springs.Count; i++) {
			LeftDeltas[i] = 0;
			RightDeltas[i] = 0;
		}

		for (var j = 0; j < passes; j += 1) {
			for (var i = 0; i< Springs.Count; i+=1) {
				if (i > 0) {
					LeftDeltas[i] = spread * (Springs[i].Height - Springs[i-1].Height);
					Springs[i-1].Velocity += LeftDeltas[i];
				}
				if (i < Springs.Count - 1) {
					RightDeltas[i] = spread * (Springs[i].Height - Springs[i+1].Height);
					Springs[i+1].Velocity += RightDeltas[i];
				}
			}
		}

		NewBorder();
		DrawWaterBody();
	}

	protected void DrawWaterBody() {
		var curve = WaterBorder.Curve;

		var points = curve.GetBakedPoints();
		var waterPolygonPoints = new Vector2[points.Length + 2];
		points.CopyTo(waterPolygonPoints, 0);

		var FirstIndex = 0;
		var LastIndex = points.Length - 1;

		waterPolygonPoints[points.Length] = new Vector2(waterPolygonPoints[LastIndex].X, WaterHeight);
		waterPolygonPoints[points.Length + 1] = new Vector2(waterPolygonPoints[FirstIndex].X, WaterHeight);

		WaterPolygon.Polygon = waterPolygonPoints;
		WaterPolygon.UV = waterPolygonPoints;
	}

	protected void NewBorder() {
		var curve = new Curve2D();

		Vector2[] SurfacePoints = new Vector2[Springs.Count];
		for (var i = 0; i < Springs.Count; i++) {
			SurfacePoints[i] = Springs[i].Position;
			curve.AddPoint(SurfacePoints[i]);
		}

		WaterBorder.Curve = curve;
		WaterBorder.Smooth(true);
		WaterBorder.QueueRedraw();
	}

	protected void OnSplash(int index, float speed) {
		if (index >= 0 && index < Springs.Count) {
			Springs[index].Velocity += speed;
		}
	}
}
