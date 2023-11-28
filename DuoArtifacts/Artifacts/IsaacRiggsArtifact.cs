﻿using HarmonyLib;
using System.Linq;

namespace Shockah.DuoArtifacts;

internal sealed class IsaacRiggsArtifact : DuoArtifact, IEvadeHook, IDroneShiftHook
{
	private static ModEntry Instance => ModEntry.Instance;

	protected internal override void ApplyPatches(Harmony harmony)
	{
		base.ApplyPatches(harmony);
		Instance.KokoroApi.RegisterEvadeHook(this, -100);
		Instance.KokoroApi.RegisterDroneShiftHook(this, -100);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		base.OnTurnStart(state, combat);
		if (combat.turn == 1)
			combat.QueueImmediate(new AStatus
			{
				status = Status.evade,
				statusAmount = 1,
				targetPlayer = true
			});
	}

	bool? IEvadeHook.IsEvadePossible(State state, Combat combat, EvadeHookContext context)
		=> state.EnumerateAllArtifacts().Any(a => a is IsaacRiggsArtifact) && state.ship.Get(Status.droneShift) > 0;

	void IEvadeHook.PayForEvade(State state, Combat combat, int direction)
	{
		var artifact = state.EnumerateAllArtifacts().First(a => a is IsaacRiggsArtifact);
		artifact.Pulse();
		state.ship.Add(Status.droneShift, -1);
	}

	bool? IDroneShiftHook.IsDroneShiftPossible(State state, Combat combat, DroneShiftHookContext context)
		=> state.EnumerateAllArtifacts().Any(a => a is IsaacRiggsArtifact) && state.ship.Get(Status.evade) > 0;

	void IDroneShiftHook.PayForDroneShift(State state, Combat combat, int direction)
	{
		var artifact = state.EnumerateAllArtifacts().First(a => a is IsaacRiggsArtifact);
		artifact.Pulse();
		state.ship.Add(Status.evade, -1);
	}
}