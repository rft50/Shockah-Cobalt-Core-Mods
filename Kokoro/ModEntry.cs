﻿using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Newtonsoft.Json.Serialization;
using Nickel;
using Nickel.Legacy;
using System.Collections.Generic;
using System.IO;
using IModManifest = CobaltCoreModding.Definitions.ModManifests.IModManifest;

namespace Shockah.Kokoro;

public sealed class ModEntry : IModManifest, IPrelaunchManifest, IApiProviderManifest, ISpriteManifest, IStatusManifest, INickelManifest
{
	internal const string ExtensionDataJsonKey = "KokoroModData";
	internal const string ScorchingTag = "Scorching";

	public static ModEntry Instance { get; private set; } = null!;
	internal IModHelper Helper { get; private set; } = null!;
	internal ApiImplementation Api { get; private set; } = null!;
	internal IHarmony Harmony { get; private set; } = null!;

	public string Name { get; init; } = typeof(ModEntry).Namespace!;
	public IEnumerable<DependencyEntry> Dependencies => [];

	public DirectoryInfo? GameRootFolder { get; set; }
	public DirectoryInfo? ModRootFolder { get; set; }
	public ILogger? Logger { get; set; }

	internal readonly Content Content = new();
	internal readonly ExtensionDataManager ExtensionDataManager = new();
	internal readonly CardRenderManager CardRenderManager = new();
	internal readonly WrappedActionManager WrappedActionManager = new();

	public void BootMod(IModLoaderContact contact)
	{
		Instance = this;
		Api = new(this);
	}

	public void OnNickelLoad(IPluginPackage<Nickel.IModManifest> package, IModHelper helper)
	{
		Helper = helper;
		Harmony = helper.Utilities.Harmony;

		ACardOfferingPatches.Apply(Harmony);
		ACardSelectPatches.Apply(Harmony);
		AStatusPatches.Apply(Harmony);
		AVariableHintPatches.Apply(Harmony);
		BigStatsPatches.Apply(Harmony);
		CardPatches.Apply(Harmony);
		CardRewardPatches.Apply(Harmony);
		CombatPatches.Apply(Harmony);
		DrawPatches.Apply(Harmony);

		CustomTTGlossary.ApplyPatches(Harmony);
		APlaySpecificCardFromAnywhere.ApplyPatches(Harmony);

		ArtifactIconManager.Setup(Harmony);
		CustomCardBrowseManager.Setup(Harmony);
		DroneShiftManager.Setup(Harmony);
		EvadeManager.Setup(Harmony);
		MidrowScorchingManager.Setup(Harmony);
		MultiCardBrowseManager.Setup(Harmony);
		RedrawStatusManager.Setup(Harmony);
		StatusLogicManager.Setup(Harmony);
		StatusRenderManager.Setup(Harmony);
		
		StatusLogicManager.Instance.Register(WormStatusManager.Instance, 0);
		StatusLogicManager.Instance.Register(OxidationStatusManager.Instance, 0);
		StatusLogicManager.Instance.Register(StatusNextTurnManager.Instance, 0);
		StatusRenderManager.Instance.Register(WormStatusManager.Instance, 0);
		StatusRenderManager.Instance.Register(OxidationStatusManager.Instance, 0);
		StatusRenderManager.Instance.Register(StatusNextTurnManager.Instance, 0);

		SetupSerializationChanges();
	}

	public void FinalizePreperations(IPrelaunchContactPoint prelaunchManifest)
		=> MidrowScorchingManager.SetupLate(Harmony);

	public object GetApi(IManifest requestingMod)
		=> new ApiImplementation(requestingMod);

	public void LoadManifest(ISpriteRegistry registry)
		=> Content.RegisterArt(registry);

	public void LoadManifest(IStatusRegistry registry)
		=> Content.RegisterStatuses(registry);

	private void SetupSerializationChanges()
	{
		JSONSettings.indented.ContractResolver = new ConditionalWeakTableExtensionDataContractResolver(
			JSONSettings.indented.ContractResolver ?? new DefaultContractResolver(),
			ExtensionDataJsonKey,
			ExtensionDataManager
		);
		JSONSettings.serializer.ContractResolver = new ConditionalWeakTableExtensionDataContractResolver(
			JSONSettings.serializer.ContractResolver ?? new DefaultContractResolver(),
			ExtensionDataJsonKey,
			ExtensionDataManager
		);
	}
}
