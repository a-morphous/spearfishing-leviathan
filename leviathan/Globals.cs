using Godot;
using System.Collections.Generic;
using deVoid.Utils;

public class ReachedCheckpointSignal: ASignal {};

public partial class Globals : Node {
	public static int CurrentCheckpoint = 0;
	public static Dictionary<int, Vector2> CheckpointPositions = new Dictionary<int, Vector2>();

	// CHECKPOINTS
	public static void AddCheckpoint(Checkpoint checkpoint) {
		if (CheckpointPositions.ContainsKey(checkpoint.CheckPointIndex)) {
			CheckpointPositions[checkpoint.CheckPointIndex] = checkpoint.SpawnPoint.GlobalPosition;
			return;
		}
		CheckpointPositions.Add(checkpoint.CheckPointIndex, checkpoint.SpawnPoint.GlobalPosition);
	}
	public static void SetCheckpoint(Checkpoint checkpoint) {
		CurrentCheckpoint = checkpoint.CheckPointIndex;
		Signals.Get<ReachedCheckpointSignal>().Dispatch();
	}

	public static Vector2 GetCurrentSpawnPoint() {
		if (!CheckpointPositions.ContainsKey(CurrentCheckpoint)) {
			return new Vector2();
		}
		return CheckpointPositions[CurrentCheckpoint];
	}
}