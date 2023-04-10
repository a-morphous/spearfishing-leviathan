using System.Threading.Tasks;
using Godot;

public partial class Flower : Enemy {
	public FlowerSprite Sprite;
	public RayCast2D LineofSight;
	public float VisibilityRange = 400;
	float VisibilitySquared;

	[Export]
	PackedScene Bullet; 

	[Export]
	PackedScene DeathAnimation;

	Player Target;

	public float FireTimer = 3f;
	float _currentFireTimer;
	float[] _bulletAngles = new float[3]{
			Mathf.DegToRad(-45),
			Mathf.DegToRad(-90),
			Mathf.DegToRad(-135)
		};

	public override void _Ready()
	{
		base._Ready();
		Sprite = GetNode<FlowerSprite>("FlowerSprite");
		Sprite.Fire += OnFire;
		LineofSight = GetNode<RayCast2D>("RayCast2D");
		VisibilitySquared = VisibilityRange * VisibilityRange;
		_currentFireTimer = GD.Randf() * FireTimer;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Target != null) {
			_currentFireTimer -= (float)delta;
			if (_currentFireTimer <= 0) {
				_currentFireTimer = FireTimer;
				Sprite.PlayFireAnimation();
			}
		}

	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Player player = Player.GetPlayer(this);
		if (player == null) {
			Target = null;
			return;
		}
		if (player.GlobalPosition.DistanceSquaredTo(GlobalPosition) > VisibilitySquared) {
			Target = null;
			return;
		}
		/* blah blah blah fix later maybe
		LineofSight.TargetPosition = ToLocal(player.GlobalPosition);
		if (!(LineofSight.GetCollider() is Player)) {
			Target = null;
			return;
		}
		*/
		Target = player;
		return;
	}

	protected void OnFire() {
		for (var i = 0; i < _bulletAngles.Length; i++) {
			var _bullet = Bullet.Instantiate<EnemyBullet>();
			_bullet.Rotation = Rotation + _bulletAngles[i];
			_bullet.BulletGravity = 500;
			_bullet.Speed = 500;
			_bullet.Position = Position + Vector2.FromAngle(_bullet.Rotation) * 30;
			GetParent().AddChild(_bullet);
		}
	}

	protected override Task OnTakeDamage(int damage, Node2D source = null)
	{
		AudioStreamManager.Get(this).Play("res://assets/sfx/fish_hit.wav", -9);
		return base.OnTakeDamage(damage, source);
	}

	protected override void OnDie()
	{
		AudioStreamManager.Get(this).Play("res://assets/sfx/fish_hit.wav", -9);
		OnDieAnimation();
	}

	protected async void OnDieAnimation()
	{
		flashEffect.SetFlash(true);
		
		await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
		if (DeathAnimation != null)
		{
			var anim = DeathAnimation.Instantiate<Node2D>();
			anim.Position = Position;
			GetParent().AddChild(anim);
		}
		base.OnDie();
		AudioStreamManager.Get(this).Play("res://assets/sfx/fish_die.wav", -9);
	}
}