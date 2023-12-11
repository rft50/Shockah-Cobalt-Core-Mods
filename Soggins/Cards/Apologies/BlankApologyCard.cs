﻿using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using System.Collections.Generic;

namespace Shockah.Soggins;

[CardMeta(dontOffer = true, rarity = Rarity.common, unreleased = true)]
public sealed class BlankApologyCard : ApologyCard, IRegisterableCard
{
	public void RegisterCard(ICardRegistry registry)
	{
		ExternalCard card = new(
			globalName: $"{GetType().Namespace}.Card.Apology.Blank",
			cardType: GetType(),
			cardArt: ModEntry.Instance.SogginsDeckBorder,
			actualDeck: ModEntry.Instance.SogginsDeck
		);
		card.AddLocalisation(I18n.ApologyCardName);
		registry.RegisterCard(card);
	}

	public override CardData GetData(State state)
	{
		var data = base.GetData(state);
		data.art = StableSpr.cards_colorless;
		data.description = I18n.BlankApologyCardText;
		return data;
	}

	public override double GetApologyWeight(State state, Combat combat, int timesGiven)
		=> 0;

	public override List<CardAction> GetActions(State s, Combat c)
		=> new() { new ADummyAction() };
}