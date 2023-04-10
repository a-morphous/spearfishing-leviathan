using Godot;
using deVoid.Utils;

// HACKETY HACK HACK
public class BodySpawnSignal : ASignal { }

public partial class PlayerSpawner : Node
{
	[Export]
	PackedScene PlayerScene;

	PhysicsBody2D Body;

	Script FadeClass;

	PackedScene WorldScene;

	public override async void _Ready()
	{
		base._Ready();
		WorldScene = new PackedScene();
		WorldScene.Pack(GetParent());
		Body = GetNode<PhysicsBody2D>("StaticBody2D");
		SetRoomPreviewBodyEnabled(false);
		Body.AddToGroup("room_enterer");

		FadeClass = ResourceLoader.Load("res://addons/UniversalFade/Fade.gd") as Script;
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
		EnableRoom();
		await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
		SpawnPlayer();
	}

	protected void SetRoomPreviewBodyEnabled(bool enabled)
	{
		Body.GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, !enabled);
		if (!enabled && Body.GetParent() == this)
		{
			RemoveChild(Body);
		}
		else if (enabled && Body.GetParent() != this)
		{
			AddChild(Body);
		}

	}

	public async void EnableRoom()
	{
		if (Player.GetPlayer(this) != null)
		{
			return;
		}
		Vector2 SpawnPosition = Globals.GetCurrentSpawnPoint();
		Body.GlobalPosition = SpawnPosition;
		SetRoomPreviewBodyEnabled(true);
		await ToSignal(GetTree(), "process_frame");
		Signals.Get<BodySpawnSignal>().Dispatch();
	}

	public void SpawnPlayer()
	{
		if (Player.GetPlayer(this) != null)
		{
			return;
		}
		SetRoomPreviewBodyEnabled(false);
		Vector2 SpawnPosition = Globals.GetCurrentSpawnPoint();
		Player player = PlayerScene.Instantiate<Player>();
		player.GlobalPosition = SpawnPosition;
		player.Died += OnPlayerDied;
		AddChild(player);
	}

	public async void OnPlayerDied(Actor player)
	{
		Body.GlobalPosition = player.GlobalPosition;
		SetRoomPreviewBodyEnabled(true);
		await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);

		//SpawnPlayer();

		// new thing, completely batshit: Create a new World.
		await ToSignal((GodotObject)FadeClass.Call("fade_out", 0.25f), "finished");
		// easy version, might not work if we have a higher-up switcher.
		// GetTree().ReloadCurrentScene();

		var newWorld = WorldScene.Instantiate<World>();
		GetParent().GetParent().AddChild(newWorld);
		GetParent().QueueFree();
	}
}