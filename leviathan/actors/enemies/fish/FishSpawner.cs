using Godot;

public partial class FishSpawner : Node2D {
	[Export]
	PackedScene FishScene;
	protected Fish fish;
	VisibleOnScreenNotifier2D Notifier;
	public Area2D ChaseArea;
	FishTarget Target;

	bool IsReady = false;

	public override void _Ready()
	{
		base._Ready();
		Notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		Target = GetNode<FishTarget>("FishTarget");
		Target.Home = this;
		ChaseArea = GetNode<Area2D>("ChaseArea");
		ChaseArea.BodyEntered += Target.OnPlayerEntered;
		ChaseArea.BodyExited += Target.OnPlayerExited;
		Notifier.ScreenExited += SpawnFish;
		IsReady = true;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		if (!IsReady) {
			return;
		}
		SpawnFish();
	}

	protected void SpawnFish() {
		if (fish != null) {
			return;
		}
		fish = FishScene.Instantiate<Fish>();
		fish.Target = Target;
		fish.Died += (Actor died) => {
			fish = null;
		};
		CallDeferred(MethodName.AddChild, fish);
	}
}
