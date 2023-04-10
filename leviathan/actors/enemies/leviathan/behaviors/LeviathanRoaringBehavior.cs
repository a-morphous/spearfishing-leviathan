using Godot;

public class LeviathanRoaringBehavior : IActorBehavior
{
	public float FishToSpawn = 3;
	public float DelayToFish = 2f;

	float _attackTime = 0;
	bool hasSpawned = false;

	PackedScene Fish;
	Control SpawnArea;

	public LeviathanRoaringBehavior(PackedScene fishScene, Control spawnArea)
	{
		this.Fish = fishScene;
		this.SpawnArea = spawnArea;
	}

	public void Reset()
	{
		_attackTime = DelayToFish;
		hasSpawned = false;
	}
	public void Update(Actor actor, double delta)
	{
		_attackTime -= (float)delta;
		if (_attackTime <= 0 && !hasSpawned)
		{
			hasSpawned = true;
			AudioStreamManager.Get(actor).Play("res://assets/sfx/leviathan_roar.wav", -6);
			// attack
			for (var i = 0; i < FishToSpawn; i++)
			{
				var fishInstance = Fish.Instantiate<Fish>();
				bool isValid = false;
				int failureCount = 0;
				Vector2 ProposedPosition = SpawnArea.Position + new Vector2(GD.Randf() * SpawnArea.Size.X, GD.Randf() * SpawnArea.Size.Y);
				while (!isValid)
				{
					if (ProposedPosition.DistanceTo(actor.Position) > 250)
					{
						isValid = true;
					}
					else
					{
						failureCount++;
					}
					ProposedPosition = SpawnArea.Position + new Vector2(GD.Randf() * SpawnArea.Size.X, GD.Randf() * SpawnArea.Size.Y);


					if (failureCount > 20)
					{
						break;
					}
				}
				fishInstance.Position = ProposedPosition;
				fishInstance.Target = Player.GetPlayer(actor);
				actor.GetParent().AddChild(fishInstance);
			}
		}
	}
}