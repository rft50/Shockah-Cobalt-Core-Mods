﻿using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using Shockah.Shared;
using System.IO;

namespace Shockah.Kokoro;

internal sealed class Content
{
	private static ModEntry Instance => ModEntry.Instance;

	internal ExternalSprite WormSprite { get; private set; } = null!;
	internal ExternalStatus WormStatus { get; private set; } = null!;

	internal void RegisterArt(ISpriteRegistry registry)
	{
		WormSprite = registry.RegisterArtOrThrow(
			id: $"{typeof(ModEntry).Namespace}.Icon.Worm",
			file: new FileInfo(Path.Combine(Instance.ModRootFolder!.FullName, "assets", "WormStatus.png"))
		);
	}

	internal void RegisterStatuses(IStatusRegistry registry)
	{
		{
			WormStatus = new(
				$"{typeof(ModEntry).Namespace}.Status.Worm",
				isGood: false,
				mainColor: System.Drawing.Color.FromArgb(unchecked((int)0xFF009900)),
				borderColor: System.Drawing.Color.FromArgb(unchecked((int)0xFF879900)),
				WormSprite,
				affectedByTimestop: true
			);
			WormStatus.AddLocalisation(I18n.WormStatusName, I18n.WormStatusDescription);
			registry.RegisterStatus(WormStatus);
		}
	}
}
