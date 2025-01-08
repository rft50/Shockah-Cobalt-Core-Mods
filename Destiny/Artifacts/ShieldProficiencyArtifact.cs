﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Nickel;

namespace Shockah.Destiny;

internal sealed class ShieldProficiencyArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("ShieldProficiency", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.DestinyDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Artifacts/ShieldProficiency.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ShieldProficiency", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ShieldProficiency", "description"]).Localize
		});
		
		ModEntry.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.NormalDamage)),
			transpiler: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Ship_NormalDamage_Transpiler))
		);
	}

	public override List<Tooltip> GetExtraTooltips()
		=> [
			.. StatusMeta.GetTooltips(PristineShield.PristineShieldStatus.Status, 1),
			.. StatusMeta.GetTooltips(Status.perfectShield, 1),
		];

	private static IEnumerable<CodeInstruction> Ship_NormalDamage_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
		// ReSharper disable PossibleMultipleEnumeration
		try
		{
			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.Find([
					ILMatches.Ldloc<int>(originalMethod).CreateLdlocaInstruction(out var ldlocaPostArmorDamage).ExtractLabels(out var labels),
					ILMatches.LdcI4(0),
					ILMatches.Ble,
					ILMatches.Ldarg(0),
					ILMatches.Ldarg(0),
					ILMatches.Ldfld("shake"),
					ILMatches.LdcR8(1),
					ILMatches.Instruction(OpCodes.Add),
					ILMatches.Stfld("shake"),
				])
				.Insert(SequenceMatcherPastBoundsDirection.Before, SequenceMatcherInsertionResultingBounds.IncludingInsertion, [
					new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
					new CodeInstruction(OpCodes.Ldarg_1),
					ldlocaPostArmorDamage,
					new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Ship_NormalDamage_Transpiler_HandleShields))),
				])
				.AllElements();
		}
		catch (Exception ex)
		{
			ModEntry.Instance.Logger.LogError("Could not patch method {Method} - {Mod} probably won't work.\nReason: {Exception}", originalMethod, ModEntry.Instance.Package.Manifest.GetDisplayName(@long: false), ex);
			return instructions;
		}
		// ReSharper restore PossibleMultipleEnumeration
	}

	private static void Ship_NormalDamage_Transpiler_HandleShields(Ship ship, State state, ref int postArmorDamage)
	{
		if (postArmorDamage <= 0)
			return;
		if (state.EnumerateAllArtifacts().FirstOrDefault(a => a is ShieldProficiencyArtifact) is not { } artifact)
			return;

		if (ship.Get(Status.perfectShield) > 0)
		{
			postArmorDamage = 0;
			artifact.Pulse();
			return;
		}

		if (ship.Get(PristineShield.PristineShieldStatus.Status) > 0)
		{
			postArmorDamage = 0;
			ship.Add(PristineShield.PristineShieldStatus.Status, -1);
			artifact.Pulse();
		}
	}
}