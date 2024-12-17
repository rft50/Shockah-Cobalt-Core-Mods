﻿using System.Collections.Generic;
using Nanoray.PluginManager;
using Nickel;
using Shockah.Shared;

namespace Shockah.Dyna;

public sealed class BurstCharge() : BaseDynaCharge($"{ModEntry.Instance.Package.Manifest.UniqueName}::BurstCharge"), IRegisterable
{
	private static ISpriteEntry Sprite = null!;
	private static ISpriteEntry LightsSprite = null!;

	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		Sprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Charges/Burst.png"));
		LightsSprite = ModEntry.Instance.Helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Charges/BurstLight.png"));
	}

	public override int BonkDamage
		=> 3;

	public override Spr GetIcon(State state)
		=> Sprite.Sprite;

	public override Spr? GetLightsIcon(State state)
		=> LightsSprite.Sprite;

	public override List<Tooltip> GetTooltips(State state)
		=> [
			new GlossaryTooltip($"{ModEntry.Instance.Package.Manifest.UniqueName}::Charge::Burst")
			{
				Icon = GetIcon(state),
				TitleColor = Colors.parttrait,
				Title = ModEntry.Instance.Localizations.Localize(["charge", "Burst", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["charge", "Burst", "description"])
			}
		];

	public override void OnTrigger(State state, Combat combat, Ship ship, Part part)
	{
		base.OnTrigger(state, combat, ship, part);
		if (part.type == PType.empty)
			return;

		var partIndex = ship.parts.IndexOf(part);
		if (partIndex < 0)
			return;

		combat.QueueImmediate(new Action
		{
			TargetPlayer = ship.isPlayerShip,
			TargetKey = part.key ?? "<null>",
		});
	}

	private sealed class Action : CardAction
	{
		public required bool TargetPlayer;
		public required string TargetKey;

		public override void Begin(G g, State s, Combat c)
		{
			base.Begin(g, s, c);

			var targetShip = TargetPlayer ? s.ship : c.otherShip;
			if (targetShip.GetPart(TargetKey) is not { } part || part.type == PType.empty)
			{
				timer = 0;
				return;
			}
			
			var worldX = targetShip.x + targetShip.parts.IndexOf(part);
			var damageDone = targetShip.NormalDamage(s, c, 3, worldX);
			var raycastResult = new RaycastResult
			{
				hitShip = true,
				worldX = worldX
			};
			EffectSpawnerExt.HitEffect(MG.inst.g, TargetPlayer, raycastResult, damageDone);

			if (part.stunModifier == PStunMod.stunnable)
				c.QueueImmediate(new AStunPart { worldX = worldX });
		}
	}
}