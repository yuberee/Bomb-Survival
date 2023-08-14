﻿namespace BombSurvival;

public partial class TimedBomb : Bomb
{
	public override string ModelPath { get; } = "models/explosive/timed_explosive.vmdl";

	public override void Spawn()
	{
		base.Spawn();
		UseAnimGraph = false;
		AnimateOnServer = true;
		PlaybackRate = 1;
	}

	[GameEvent.Tick]
	internal virtual void Explosion()
	{
		CurrentSequence.Time = Time.Now % CurrentSequence.Duration;

		if ( Game.IsServer )
			if ( CurrentSequence.Time >= 6.6f )
				Explode();
	}
}
