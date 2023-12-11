﻿using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using HarmonyLib;
using Shockah.Shared;
using System.Collections.Generic;

namespace Shockah.Soggins;

[CardMeta(rarity = Rarity.uncommon)]
public sealed class SoSorryCard : Card, IRegisterableCard, IFrogproofCard
{
	private static ModEntry Instance => ModEntry.Instance;

	private static bool IsDuringTryPlayCard = false;

	public void RegisterCard(ICardRegistry registry)
	{
		ExternalCard card = new(
			globalName: $"{GetType().Namespace}.Card.SoSorry",
			cardType: GetType(),
			cardArt: ModEntry.Instance.SogginsDeckBorder,
			actualDeck: ModEntry.Instance.SogginsDeck
		);
		card.AddLocalisation(I18n.SoSorryCardName);
		registry.RegisterCard(card);
	}

	public void ApplyPatches(Harmony harmony)
	{
		harmony.TryPatch(
			logger: Instance.Logger!,
			original: () => AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.TryPlayCard)),
			prefix: new HarmonyMethod(GetType(), nameof(Combat_TryPlayCard_Prefix)),
			finalizer: new HarmonyMethod(GetType(), nameof(Combat_TryPlayCard_Finalizer))
		);
	}

	public override CardData GetData(State state)
	{
		var data = base.GetData(state);
		data.art = StableSpr.cards_colorless;
		data.description = state.route is Combat combat ? string.Format(I18n.SoSorryCardTextCurrent, Instance.Api.GetTimesBotchedThisCombat(state, combat)) : I18n.SoSorryCardTextGeneric;
		data.cost = 2;
		return data;
	}

	public override List<CardAction> GetActions(State s, Combat c)
	{
		List<CardAction> actions = new()
		{
			Instance.Api.MakeAddSmugAction(s, 1)
		};

		int amount = Instance.Api.GetTimesBotchedThisCombat(s, c);
		if (IsDuringTryPlayCard)
		{
			for (int i = 0; i < amount; i++)
				actions.Add(new AAddCard
				{
					card = SmugStatusManager.GenerateAndTrackApology(s, c, s.rngActions),
					destination = CardDestination.Hand,
					omitFromTooltips = i != 0
				});
		}
		else
		{
			actions.Add(new AAddCard
			{
				card = new RandomPlaceholderApologyCard(),
				destination = CardDestination.Hand,
				amount = amount
			});
		}

		return actions;
	}

	private static void Combat_TryPlayCard_Prefix()
		=> IsDuringTryPlayCard = true;

	private static void Combat_TryPlayCard_Finalizer()
		=> IsDuringTryPlayCard = false;
}
