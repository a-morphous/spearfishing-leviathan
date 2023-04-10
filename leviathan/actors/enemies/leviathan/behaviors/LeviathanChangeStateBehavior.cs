using Godot;
using System.Collections.Generic;
using Medallion;

public class LeviathanChangeStateBehavior : IActorBehavior
{
	Dictionary<Leviathan.LEVIATHAN_STATES, float> TIME_IN_STATE;
	List<Leviathan.LEVIATHAN_STATES> StateCycle;

	float _timer = 0;
	int _currentStateIndex = -1;

	public LeviathanChangeStateBehavior()
	{
		TIME_IN_STATE = new Dictionary<Leviathan.LEVIATHAN_STATES, float>() {
			{Leviathan.LEVIATHAN_STATES.NIGHTMARE_CHARGE, 13f},
			{Leviathan.LEVIATHAN_STATES.SWIMMING, 15f},
			{Leviathan.LEVIATHAN_STATES.SHOOTING_BULLETS, 12f},
			{Leviathan.LEVIATHAN_STATES.ROARING, 8f},
			{Leviathan.LEVIATHAN_STATES.IDLE, 10f},
		};

		StateCycle = new List<Leviathan.LEVIATHAN_STATES>() {
			{ Leviathan.LEVIATHAN_STATES.NIGHTMARE_CHARGE },
			{ Leviathan.LEVIATHAN_STATES.SHOOTING_BULLETS },
			{ Leviathan.LEVIATHAN_STATES.SWIMMING },
			{ Leviathan.LEVIATHAN_STATES.ROARING },
		};
		StateCycle.Shuffle();
	}

	public void Update(Actor actor, double delta)
	{

		if (!(actor is Leviathan))
		{
			return;
		}
		Leviathan lev = (Leviathan)actor;
		_timer -= (float)delta;
		if (_timer < 0)
		{
			_currentStateIndex += 1;
			if (_currentStateIndex >= StateCycle.Count)
			{
				_currentStateIndex = -1;
				StateCycle.Shuffle();
				SetCycle(lev, Leviathan.LEVIATHAN_STATES.IDLE);
				return;
			}
			SetCycle(lev, StateCycle[_currentStateIndex]);
		}
	}

	protected void SetCycle(Leviathan actor, Leviathan.LEVIATHAN_STATES newState)
	{
		actor.SetLeviathanState(newState);
		if (TIME_IN_STATE.ContainsKey(newState))
		{
			_timer = TIME_IN_STATE[newState];
		}
		else
		{
			_timer = 10f;
		}

	}
}