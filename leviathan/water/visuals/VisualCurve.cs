using Godot;
using System;

public partial class VisualCurve : Path2D
{
	[Export]
	public float SplineLength = 8;

	[Export]
	public Color color;
	public float width = 8;

	public void Straighten(bool shouldStraighten)
	{
		if (!shouldStraighten)
		{
			return;
		}
		for (var i = 0; i < Curve.PointCount; i++)
		{
			Curve.SetPointIn(i, new Vector2());
			Curve.SetPointOut(i, new Vector2());
		}
	}

	public void Smooth(bool ShouldSmooth)
	{
		if (!ShouldSmooth)
		{
			return;
		}

		for (var i = 1; i < Curve.PointCount - 1; i++)
		{
			var Spline = GetSpline(i);
			Curve.SetPointIn(i, -Spline);
			Curve.SetPointOut(i, Spline);
		}
	}

	protected Vector2 GetSpline(int i)
	{
		var LastPoint = GetPoint(i - 1);
		var NextPoint = GetPoint(i + 1);
		var Spline = LastPoint.DirectionTo(NextPoint) * SplineLength;
		return Spline;
	}

	protected Vector2 GetPoint(int i)
	{
		i = Mathf.Wrap(i, 0, Curve.PointCount);
		if (i > 1 && i < Curve.PointCount - 1)
		{
			return Curve.GetPointPosition(i);
		}
		else if (i <= 1)
		{
			return new Vector2(Curve.GetPointPosition(1).X - SplineLength, Curve.GetPointPosition(1).Y);
		}
		else
		{
			return new Vector2(Curve.GetPointPosition(Curve.PointCount - 1).X + SplineLength, Curve.GetPointPosition(Curve.PointCount - 1).Y);
		}
	}

	public override void _Draw()
	{
		var Points = Curve.GetBakedPoints();
		if (Points != null && Points.Length > 2) {
			DrawPolyline(Points, color, width, true);
		}
	}
}
