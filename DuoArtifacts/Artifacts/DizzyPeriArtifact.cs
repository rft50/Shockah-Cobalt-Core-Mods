﻿using Shockah.Kokoro;
using System;

namespace Shockah.DuoArtifacts;

public sealed class DizzyPeriArtifact : DuoArtifact, IKokoroApi.IV2.IStatusLogicApi.IHook, IKokoroApi.IV2.IHookPriority
{
	public override void OnTurnStart(State state, Combat combat)
	{
		base.OnTurnStart(state, combat);

		var toSubtract = Math.Clamp(state.ship.Get(Status.overdrive), 0, state.ship.Get(Status.shield));
		if (toSubtract > 0)
			combat.QueueImmediate(new AStatus
			{
				status = Status.shield,
				statusAmount = -toSubtract,
				targetPlayer = true
			});

		toSubtract = Math.Clamp(state.ship.Get(Status.perfectShield), 0, state.ship.Get(Status.overdrive));
		if (toSubtract > 0)
			combat.QueueImmediate(new AStatus
			{
				status = Status.overdrive,
				statusAmount = -toSubtract,
				targetPlayer = true
			});
	}

	public double HookPriority
		=> double.MinValue;

	public int ModifyStatusChange(IKokoroApi.IV2.IStatusLogicApi.IHook.IModifyStatusChangeArgs args)
	{
		if (args.Status != Status.shield)
			return args.NewAmount;

		var maxShield = args.Ship.GetMaxShield();
		var overshield = Math.Max(0, args.NewAmount - maxShield);
		if (overshield <= 0)
			return args.NewAmount;

		var newAmount = args.NewAmount - overshield;
		args.Combat.QueueImmediate(new AStatus
		{
			status = Status.overdrive,
			statusAmount = overshield,
			targetPlayer = true
		});
		Pulse();
		return newAmount;
	}
}
