﻿using Nickel;

namespace Shockah.EventsGalore;

public sealed class ApiImplementation : IApi
{
	public IStatusEntry ActionReactionStatus
		=> EventsGalore.ActionReactionStatus.Instance.Entry;
	
	public IStatusEntry VolatileOverdriveStatus
		=> EventsGalore.VolatileOverdriveStatus.Instance.Entry;

	#region Self-destruct
	public IStatusEntry SelfDestructTimerStatus
		=> BombEnemy.SelfDestructTimerStatus.Instance.Entry;

	public Intent MakeSelfDestructIntent(int flatDamage = 0, double percentCurrentDamage = 0, double percentMaxDamage = 0, bool preventDeath = false)
		=> new BombEnemy.SelfDestructIntent
		{
			FlatDamage = flatDamage,
			PercentCurrentDamage = percentCurrentDamage,
			PercentMaxDamage = percentMaxDamage,
			PreventDeath = preventDeath,
		};
	#endregion
}
