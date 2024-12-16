﻿using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System;
using HarmonyLib;
using Shockah.Kokoro;
using System.Linq;
using Shockah.Shared;

namespace Shockah.Destiny;

public sealed class ModEntry : SimpleMod
{
	internal static ModEntry Instance { get; private set; } = null!;
	internal readonly IHarmony Harmony;
	internal readonly IKokoroApi.IV2 KokoroApi;
	internal readonly ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations;
	internal readonly ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations;

	internal IDeckEntry DestinyDeck { get; }

	private static readonly IReadOnlyList<Type> CommonCardTypes = [
		typeof(HoneCard),
	];

	private static readonly IReadOnlyList<Type> UncommonCardTypes = [
	];

	private static readonly IReadOnlyList<Type> RareCardTypes = [
	];

	private static readonly IEnumerable<Type> AllCardTypes
		= [
			.. CommonCardTypes,
			.. UncommonCardTypes,
			.. RareCardTypes,
			//typeof(BlochExeCard),
		];

	private static readonly IReadOnlyList<Type> CommonArtifacts = [
	];

	private static readonly IReadOnlyList<Type> BossArtifacts = [
	];

	private static readonly IReadOnlyList<Type> DuoArtifacts = [
	];

	private static readonly IEnumerable<Type> AllArtifactTypes
		= [
			.. CommonArtifacts,
			.. BossArtifacts,
		];

	private static readonly IEnumerable<Type> RegisterableTypes
		= [
			.. AllCardTypes,
			.. AllArtifactTypes,
			.. DuoArtifacts,
			typeof(EnchantedManager),
			typeof(MagicFindManager),
		];

	public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		Instance = this;
		Harmony = helper.Utilities.Harmony;
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;

		this.AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
		);
		this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
		);

		DestinyDeck = helper.Content.Decks.RegisterDeck("Destiny", new()
		{
			Definition = new() { color = new("23EEB6"), titleColor = Colors.black },
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/CardFrame.png")).Sprite,
			Name = this.AnyLocalizations.Bind(["character", "name"]).Localize,
			ShineColorOverride = _ => DB.decks[DestinyDeck!.Deck].color.normalize().gain(2.5),
		});

		foreach (var type in RegisterableTypes)
			AccessTools.DeclaredMethod(type, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);

		helper.Content.Characters.V2.RegisterPlayableCharacter("Destiny", new()
		{
			Deck = DestinyDeck.Deck,
			Description = this.AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/CharacterFrame.png")).Sprite,
			NeutralAnimation = new()
			{
				CharacterType = DestinyDeck.UniqueName,
				LoopTag = "neutral",
				Frames = package.PackageRoot.GetRelativeDirectory("assets/Character/Neutral")
					.GetSequentialFiles(i => $"{i}.png")
					.Select(f => helper.Content.Sprites.RegisterSprite(f).Sprite)
					.ToList()
			},
			MiniAnimation = new()
			{
				CharacterType = DestinyDeck.UniqueName,
				LoopTag = "mini",
				Frames = [
					helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Character/Mini.png")).Sprite
				]
			},
			Starters = new()
			{
				cards = [
					new HoneCard(),
				]
			},
			//ExeCardType = typeof(BlochExeCard),
		});
		
		// helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		// {
		// 	CharacterType = DestinyDeck.UniqueName,
		// 	LoopTag = "gameover",
		// 	Frames = package.PackageRoot.GetRelativeDirectory("assets/Character/GameOver")
		// 		.GetSequentialFiles(i => $"{i}.png")
		// 		.Select(f => helper.Content.Sprites.RegisterSprite(f).Sprite)
		// 		.ToList()
		// });
		// helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		// {
		// 	CharacterType = DestinyDeck.UniqueName,
		// 	LoopTag = "squint",
		// 	Frames = package.PackageRoot.GetRelativeDirectory("assets/Character/Squint")
		// 		.GetSequentialFiles(i => $"{i}.png")
		// 		.Select(f => helper.Content.Sprites.RegisterSprite(f).Sprite)
		// 		.ToList()
		// });
	}

	internal static Rarity GetCardRarity(Type type)
	{
		if (RareCardTypes.Contains(type))
			return Rarity.rare;
		if (UncommonCardTypes.Contains(type))
			return Rarity.uncommon;
		return Rarity.common;
	}

	internal static ArtifactPool[] GetArtifactPools(Type type)
	{
		if (BossArtifacts.Contains(type))
			return [ArtifactPool.Boss];
		if (CommonArtifacts.Contains(type))
			return [ArtifactPool.Common];
		return [];
	}
}