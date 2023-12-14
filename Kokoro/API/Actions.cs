﻿using System.Collections.Generic;

namespace Shockah.Kokoro;

public partial interface IKokoroApi
{
	IActionApi Actions { get; }

	public partial interface IActionApi
	{
		CardAction MakeExhaustEntireHandImmediate();
		CardAction MakePlaySpecificCardFromAnywhere(int cardId, bool showTheCardIfNotInHand = true);
		CardAction MakePlayRandomCardsFromAnywhere(IEnumerable<int> cardIds, int amount = 1, bool showTheCardIfNotInHand = true);
	}
}