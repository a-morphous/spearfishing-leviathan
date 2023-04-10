using Godot;
using System.Collections.Generic;

public struct AudioQueue {
	public string StreamPath;
	public float Volume;

	public AudioQueue(string path, float vol) {
		StreamPath = path;
		Volume = vol;
	}
}

public partial class AudioStreamManager : Node {
	int NumPlayers = 8;
	string Bus = "master";

	Queue<AudioStreamPlayer> Available;
	Queue<AudioQueue> Queue;
	float ExpectedVolume = 0;

	public override void _Ready()
	{
		Available = new Queue<AudioStreamPlayer>();
		Queue = new Queue<AudioQueue>();
		base._Ready();
		for (var i = 0; i < NumPlayers; i++) {
			var p = new AudioStreamPlayer();
			AddChild(p);
			Available.Enqueue(p);
			p.Finished += () => {
				Available.Enqueue(p);
			};
			p.Bus = Bus;
		}
	}

	public static AudioStreamManager Get(Node caller) {
		// assumes autoload
		return caller.GetNode<AudioStreamManager>("/root/AudioStreamManager");
	}

	public void Play(string SoundPath, float Volume) {
		Queue.Enqueue(new AudioQueue(SoundPath, Volume));
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Queue.Count > 0 && Available.Count > 0) {
			var currentStream = Available.Dequeue();
			var streamData = Queue.Dequeue();
			currentStream.Stream = ResourceLoader.Load<AudioStream>(streamData.StreamPath);
			currentStream.VolumeDb = streamData.Volume;
			currentStream.Play();
		}
	}
}