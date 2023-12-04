﻿using HarmonyLib;
using System.Linq;

namespace Shockah.DuoArtifacts;

internal sealed class DrakeRiggsArtifact : DuoArtifact, IEvadeHook
{
	public bool UsedThisTurn = false;

	protected internal override void ApplyPatches(Harmony harmony)
	{
		base.ApplyPatches(harmony);
		Instance.KokoroApi.RegisterEvadeHook(this, -10);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		UsedThisTurn = false;
	}

	bool? IEvadeHook.IsEvadePossible(State state, Combat combat, EvadeHookContext context)
	{
		var artifact = state.EnumerateAllArtifacts().OfType<DrakeRiggsArtifact>().FirstOrDefault();
		if (artifact is null)
			return null;
		if (artifact.UsedThisTurn)
			return null;
		return true;
	}

	void IEvadeHook.PayForEvade(State state, Combat combat, int direction)
	{
		var artifact = state.EnumerateAllArtifacts().OfType<DrakeRiggsArtifact>().First();
		artifact.Pulse();
		artifact.UsedThisTurn = true;
		combat.QueueImmediate(new AStatus
		{
			status = Status.heat,
			statusAmount = 1,
			targetPlayer = true
		});
	}
}