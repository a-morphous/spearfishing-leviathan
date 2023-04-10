using Godot;
using System;
using deVoid.Utils;

public class OnActivatedRoomSignal : ASignal<Room, Node2D> { };
public class OnDeactivatedRoomSignal : ASignal<Room> { };

public partial class Room : Node2D
{
	TileMap CollisionMap;
	Area2D RoomBounds;
	Node2D ObjectContainer;
	// Called when the node enters the scene tree for the first time.

	bool IsActive = true;
	bool HasSignals = false;
	public override void _Ready()
	{
		AddToGroup("rooms");
		CollisionMap = GetNode<TileMap>("CollisionMap");
		RoomBounds = GetNode<Area2D>("ActiveArea");
		ObjectContainer = GetNode<Node2D>("Objects");

		RoomBounds.Position = GetBounds().GetCenter();

		var RoomBoundsShape = GetNode<CollisionShape2D>("ActiveArea/CollisionShape2D");
		var rectShape = new RectangleShape2D();
		rectShape.Size = GetBounds().Size;
		RoomBoundsShape.Shape = rectShape;

		RoomBounds.BodyEntered += EnterRoom;
		RoomBounds.BodyExited += ExitRoom;

		Signals.Get<PlayerCreatedSignal>().AddListener(Reinitialize);
		Signals.Get<BodySpawnSignal>().AddListener(Reinitialize);
		HasSignals = true;
	}

	public override void _EnterTree()
	{
		if (HasSignals)
		{
			return;
		}
		Signals.Get<PlayerCreatedSignal>().AddListener(Reinitialize);
		Signals.Get<BodySpawnSignal>().AddListener(Reinitialize);
		HasSignals = true;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (!HasSignals)
		{
			return;
		}
		HasSignals = false;
		Signals.Get<PlayerCreatedSignal>().RemoveListener(Reinitialize);
		Signals.Get<BodySpawnSignal>().RemoveListener(Reinitialize);
	}

	public void Reinitialize(Player player)
	{
		Reinitialize();
	}

	public void Reinitialize()
	{
		if (!Node.IsInstanceValid(this))
		{
			Signals.Get<PlayerCreatedSignal>().RemoveListener(Reinitialize);
			Signals.Get<BodySpawnSignal>().RemoveListener(Reinitialize);
			return;
		}
		var bodies = RoomBounds.GetOverlappingBodies();
		var shouldActivate = false;
		foreach (var body in bodies)
		{
			if (body.IsInGroup("player") || body.IsInGroup("room_enterer"))
			{
				shouldActivate = true;
			}
		}

		if (!shouldActivate)
		{
			CallDeferred(nameof(Deactivate));
		}
		else
		{
			GetParent<Zone>().CallDeferred(Zone.MethodName.Activate);
			CallDeferred(nameof(Activate));
		}
	}

	public Rect2 GetBounds()
	{
		var TileRectSize = CollisionMap.GetUsedRect().Size;
		TileRectSize.X *= CollisionMap.TileSet.TileSize.X;
		TileRectSize.Y *= CollisionMap.TileSet.TileSize.Y;

		var TileRectPosition = CollisionMap.GetUsedRect().Position;
		TileRectPosition.X *= CollisionMap.TileSet.TileSize.X;
		TileRectPosition.Y *= CollisionMap.TileSet.TileSize.Y;
		return new Rect2(TileRectPosition, TileRectSize);
	}

	public void ShouldActivate(Node2D activator)
	{
		if (IsActive)
		{
			return;
		}
		Signals.Get<OnActivatedRoomSignal>().Dispatch(this, activator);
	}

	public void Activate()
	{
		AddToGroup("active_rooms");
		IsActive = true;
		Visible = true;
		if (ObjectContainer.GetParent() == this)
		{
			return;
		}
		AddChild(ObjectContainer);
	}

	public void Deactivate()
	{
		if (!IsActive)
		{
			return;
		}
		Signals.Get<OnDeactivatedRoomSignal>().Dispatch(this);
		RemoveFromGroup("active_rooms");
		IsActive = false;
		Visible = false;
		if (ObjectContainer.GetParent() == null)
		{
			return;
		}
		RemoveChild(ObjectContainer);

	}

	protected void EnterRoom(Node2D body)
	{
		if (body.IsInGroup("player") || body.IsInGroup("room_enterer"))
		{
			ShouldActivate(body);
		}
	}

	protected void ExitRoom(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			Player player = (Player)body;
			if (player.HP <= 0)
			{
				return;
			}
			Deactivate();
		}
	}
}
