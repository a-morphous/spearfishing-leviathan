using Godot;
using System;

public partial class Zone : Node2D
{
	public int ZoneBuffer = 200;

	Area2D ZoneArea;

	public Room Room { get; protected set; }
	CollisionShape2D Rectangle;
	public override void _Ready()
	{
		ZoneArea = GetNode<Area2D>("ZoneArea");
		Room = GetNode<Room>("Room");
		var roomBounds = Room.GetBounds();
		
		Rectangle = GetNode<CollisionShape2D>("ZoneArea/Rectangle");
		var rectShape = new RectangleShape2D();
		rectShape.Size = new Vector2(roomBounds.Size.X + ZoneBuffer * 2, roomBounds.Size.Y + ZoneBuffer * 2);
		Rectangle.Shape = rectShape;
		ZoneArea.Position = roomBounds.GetCenter();

		ZoneArea.BodyEntered += PlayerEntered;
		ZoneArea.BodyExited += PlayerExited;

		CallDeferred(nameof(Deactivate));
	}

	public void Activate() {
		if (Room.GetParent() == this) {
			return;
		}
		AddChild(Room);
	}

	public void Deactivate() {
		if (Room.GetParent() == null) {
			return;
		}
		RemoveChild(Room);
	}

	protected void PlayerEntered(Node2D body) {
		if (body.IsInGroup("player") || body.IsInGroup("room_enterer")) {
			CallDeferred(nameof(Activate));
		}
	}

	protected void PlayerExited (Node2D body) {
	if (body.IsInGroup("player")) {
			// dpn't deactivate if the player died
			if ((body as Player).HP <= 0) {
				return;
			}
			CallDeferred(nameof(Deactivate));
		}
	}
}
