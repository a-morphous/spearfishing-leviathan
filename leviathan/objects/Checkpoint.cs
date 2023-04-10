using Godot;

public partial class Checkpoint: Area2D {
	[Export]
	PackedScene CheckpointLabel;
	[Export]
	public int CheckPointIndex = 0;
	public Node2D SpawnPoint;


	public override void _Ready()
	{
		base._Ready();
		AddToGroup("checkpoints");
		SpawnPoint = GetNode<Node2D>("SpawnPoint");
		Globals.AddCheckpoint(this);
		BodyEntered += OnPlayerEntered;
	}

	protected void OnPlayerEntered(Node2D body) {
		if (!(body is Player)) {
			return;
		}
		if (Globals.CurrentCheckpoint == CheckPointIndex) {
			return;
		}
		if (CheckpointLabel != null) {
			var label = CheckpointLabel.Instantiate<Node2D>();
			label.Position = new Vector2(0, -60);
			body.AddChild(label);
		}
		Globals.SetCheckpoint(this);
	}
}