using Godot;
using System;
using System.Collections.Generic;
using deVoid.Utils;

public partial class World : Node2D
{
	List<Room> Rooms;
	Script FadeClass;
	Room ActiveRoom;
	Camera2D MainCamera;
	CameraFollower CameraFollower;

	public override void _Ready()
	{
		Rooms = new List<Room>();
		// get all the rooms in this world
		foreach (var node in GetChildren())
		{
			if (node is Zone)
			{
				Zone nodeAsZone = (Zone)node;
				Rooms.Add(nodeAsZone.Room);
			}
			if (node is Room)
			{
				Rooms.Add((Room)node);
			}
		}
		Signals.Get<OnActivatedRoomSignal>().AddListener(OnNewRoomActivated);
		FadeClass = ResourceLoader.Load("res://addons/UniversalFade/Fade.gd") as Script;

		MainCamera = GetNode<Camera2D>("Camera2D");
		CameraFollower = GetNode<CameraFollower>("CameraFollower");
	}

	public override void _Process(double delta)
	{
		MainCamera.GlobalPosition = CameraFollower.GlobalPosition;
	}

	protected async void OnNewRoomActivated(Room activeRoom, Node2D player)
	{
		if (!Node.IsInstanceValid(this)) {
			Signals.Get<OnActivatedRoomSignal>().RemoveListener(OnNewRoomActivated);
			return;
		}
		if (ActiveRoom == null) {
			await ToSignal((GodotObject)FadeClass.Call("fade_out", 0.25f), "finished");
			TransitionRooms(activeRoom, player);
			await ToSignal((GodotObject)FadeClass.Call("fade_in", 0.25f), "finished");
			
			return;
		}

		GetTree().Paused = true;
		await ToSignal((GodotObject)FadeClass.Call("fade_out", 0.25f), "finished");
		GetTree().Paused = false;
		// deactivate currently active rooms
		TransitionRooms(activeRoom, player);

		await ToSignal((GodotObject)FadeClass.Call("fade_in", 0.25f), "finished");
	}

	protected async void TransitionRooms(Room activeRoom, Node2D player)
	{
		ActiveRoom = activeRoom;
			
		var OtherActiveRooms = GetTree().GetNodesInGroup("active_rooms");
		activeRoom.CallDeferred(Room.MethodName.Activate);
		foreach (var node in OtherActiveRooms)
		{
			if (node == activeRoom)
			{
				continue;
			}
			Room room = (Room)node;
			room.CallDeferred(Room.MethodName.Deactivate);
		}
		// push player into active room.
		var RoomRect = activeRoom.GetBounds();
		var padding = 16;
		var position = player.GlobalPosition;
		position -= activeRoom.GlobalPosition;
		position.X = Mathf.Clamp(position.X, RoomRect.Position.X + padding, RoomRect.Position.X + RoomRect.Size.X - padding);
		position.Y = Mathf.Clamp(position.Y, RoomRect.Position.Y + padding, RoomRect.Position.Y + RoomRect.Size.Y - padding);
		position += activeRoom.GlobalPosition;
		player.GlobalPosition = position;

		// move camera
		if (RoomRect.Size.X < MainCamera.GetViewportRect().Size.X && RoomRect.Size.Y < MainCamera.GetViewportRect().Size.Y) {
			MainCamera.PositionSmoothingEnabled = false;
			CameraFollower.ObjectToFollow = null;
			CameraFollower.Position = activeRoom.GlobalPosition + RoomRect.Position + RoomRect.Size / 2;
			MainCamera.Position = CameraFollower.Position;
			await ToSignal(GetTree(), "process_frame");
			MainCamera.PositionSmoothingEnabled = true;
		} else {
			MainCamera.PositionSmoothingEnabled = false;
			CameraFollower.ObjectToFollow = player;
			MainCamera.GlobalPosition = player.GlobalPosition;
			await ToSignal(GetTree(), "process_frame");
			MainCamera.PositionSmoothingEnabled = true;
		}
	}
}