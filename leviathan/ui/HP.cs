using Godot;
using System;

using deVoid.Utils;

public partial class HP : Control
{
	Player Player;
	Label ActualHPLabel;
	Label MaxHPLabel;
	ProgressBar HPBar;

	public override void _Ready()
	{
		ActualHPLabel = GetNode<Label>("VBoxContainer/HFlowContainer/HPLabel");
		MaxHPLabel = GetNode<Label>("VBoxContainer/HFlowContainer/MaxHPLabel");
		HPBar = GetNode<ProgressBar>("VBoxContainer/ProgressBar");
		Initialize(Player.GetPlayer(this));
		Signals.Get<PlayerCreatedSignal>().AddListener(Initialize);
	}

	public void Initialize(Player player)
	{
		if (!Node.IsInstanceValid(this)) {
			Signals.Get<PlayerCreatedSignal>().RemoveListener(Initialize);
			return;
		}
		if (Player == player)
		{
			Visible = false;
			return;
		}
		Player = player;
		if (Player == null)
		{
			Visible = false;
			return;
		}
		Visible = true;
		Player.TookDamage += PlayerTookDamage;
		Player.Healed += PlayerTookDamage;
		Player.Died += (Actor player) =>
		{
			ActualHPLabel.Text = "0";
			HPBar.Value = 0;
			Player = null;
		};
		UpdatePlayerHP();
	}

	protected void PlayerTookDamage(Actor player, int damage)
	{
		UpdatePlayerHP();
	}

	protected void UpdatePlayerHP()
	{
		if (Player == null)
		{
			Visible = false;
			return;
		}
		Visible = true;
		ActualHPLabel.Text = Player.HP.ToString();
		MaxHPLabel.Text = "/ " + Player.MaxHP.ToString();
		HPBar.Value = Player.HP;
		HPBar.MaxValue = Player.MaxHP;
	}
}
