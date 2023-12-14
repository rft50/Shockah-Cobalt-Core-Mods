﻿namespace Shockah.Soggins;

internal static class I18n
{
	public static string SogginsName => "Soggins";
	public static string SogginsDescription => "<c=B79CE5>SOGGINS</c>\nThis is that frog who keeps making mistakes. Why did we let him on the ship?";

	public static string SmugArtifactName => "Smug";
	public static string SmugArtifactDescription => "Start each combat with <c=status>SMUG</c>.";

	public static string VideoWillArtifactName => "Video Will";
	public static string VideoWillArtifactDescription => "Start each combat with 3 <c=status>FROGPROOFING</c>.";
	public static string PiratedShipCadArtifactName => "Pirated C.A.T.";
	public static string PiratedShipCadArtifactDescription => "Whenever you <c=downside>botch</c> a card, gain 1 <c=status>TEMP SHIELD</c>.";
	public static string HotTubArtifactName => "Hot Tub";
	public static string HotTubArtifactDescription => "At the end of your turn, if <c=status>SMUG</c> is <c=boldPink>{0}</c>, gain 1 <c=status>SMUG</c>; if <c=status>SMUG</c> is <c=boldPink>{1}</c>, lose 1 <c=status>SMUG</c>.";
	public static string MisprintedApologyArtifactName => "Misprinted Apology";
	public static string MisprintedApologyArtifactDescription => "The first <c=card>Halfhearted Apology</c> you receive each turn is <c=cardtrait>DUAL</c>.";

	public static string HijinksArtifactName => "Hijinks";
	public static string HijinksArtifactDescription => "Gain 1 extra <c=energy>ENERGY</c> every turn. <c=downside>Your chance to botch card effects through <c=status>SMUG</c> is doubled.</c>";
	public static string RepeatedMistakesArtifactName => "Repeated Mistakes";
	public static string RepeatedMistakesArtifactDescription => "Start each combat with 3 <c=status>MISSILE MALFUNCTION</c>. At the start of each turn, <c=action>LAUNCH</c> a <c=midrow>SEEKER</c>.";

	public static string FrogproofCardTraitName => $"Frogproof";
	public static string FrogproofCardTraitText => $"This card ignores <c=status>SMUG</c>.";

	public static string SmugStatusName => "Smug";
	public static string SmugStatusDescription => "Affects the chance to <c=downside>botch</c> or <c=cheevoGold>double</c> cards.\nLow smug will <c=downside>botch</c> more often, while high smug will <c=cheevoGold>double</c> more often.\n<c=downside>Beware of oversmugging.</c>";
	public static string SmugStatusCurrentChancesDescription => "<c=downside>Botch chance:</c> <c=boldPink>{1:N0}%</c>\n<c=cheevoGold>Double chance:</c> <c=boldPink>{0:N0}%</c>";
	public static string FrogproofingStatusName => "Frogproofing";
	public static string FrogproofingStatusDescription => "Whenever you play a card that is not <c=cardtrait>FROGPROOF</c>, temporarily give it <c=cardtrait>FROGPROOF</c> <c=downside>and decrease this by 1.</c>";
	public static string BotchesStatusName => "Botches";
	public static string BotchesStatusDescription => "The number of cards <c=downside>botched</c> this combat through <c=status>SMUG</c>.";
	public static string ExtraApologiesStatusName => "Extra Apologies";
	public static string ExtraApologiesStatusDescription => "<c=downside>Botching</c> a card grants you <c=boldPink>{0}</c> extra <c=card>Halfhearted Apology</c>.";
	public static string ConstantApologiesStatusName => "Constant Apologies";
	public static string ConstantApologiesStatusDescription => "Gain {0} <c=card>Halfhearted Apology</c> at the start of every turn.";
	public static string BidingTimeStatusName => "Biding Time";
	public static string BidingTimeStatusDescription => "Gain <c=status>DOUBLE TIME</c> at the start of your turn and <c=downside>decrease this by 1.</c>";
	public static string DoubleTimeStatusName => "Double Time";
	public static string DoubleTimeStatusDescription => "All your card effects this turn are <c=cheevoGold>doubled</c> and ignore <c=status>SMUG</c>. <c=downside>Lose this at the end of your turn.</c>";
	public static string DoublersLuckStatusName => "Doubler's Luck";
	public static string DoublersLuckStatusDescription => "Your chance to <c=cheevoGold>double</c> card effects through <c=status>SMUG</c> is {0}x higher.";

	public static string ApologyCardName => "Halfhearted Apology";
	public static string BlankApologyCardText => $"*a random {ApologyCardName} card*";

	// starter common cards
	public static string SmugnessControlCardName => "Smugness Control";
	public static string PressingButtonsCardName => "Pressing Buttons";

	// common cards
	public static string TakeCoverCardName => "Take Cover!";
	public static string ZenCardName => "Zen";
	public static string MysteriousAmmoCardName => "Mysterious Ammo";
	public static string RunningInCirclesCardName => "Running in Circles";
	public static string BetterSpaceMineCardName => "Better Space Mine";
	public static string ThoughtsAndPrayersCardName => "Thoughts & Prayers";
	public static string StopItCardName => "Stop It!";

	// uncommon cards
	public static string HarnessingSmugnessCardName => "Harnessing Smugness";
	public static string SoSorryCardName => "So Sorry";
	public static string BetterThanYouCardName => "Better Than You";
	public static string BetterThanYouCardText0 => "Discard non <c=B79CE5>Soggins</c> cards. Draw as many <c=B79CE5>Soggins</c> cards.";
	public static string BetterThanYouCardTextA => "Discard non <c=B79CE5>Soggins</c> cards. Draw as many <c=B79CE5>Soggins</c> cards.";
	public static string BetterThanYouCardTextB => "Draw 10 <c=B79CE5>Soggins</c> cards.";
	public static string ImTryingCardName => "I'm Trying!";
	public static string BlastFromThePastCardName => "Blast from the Past";
	public static string HumiliatingAttackCardName => "Humiliating Attack";
	public static string BegForMercyCardName => "Beg for Mercy";

	// rare cards
	public static string ExtraApologyCardName => "Extra Apology";
	public static string DoSomethingCardName => "Do Something!";
	public static string DoSomethingCardText0 => "Play a random card from your <c=keyword>draw pile</c>.";
	public static string DoSomethingCardTextA => "Play a random card from your <c=keyword>discard pile</c>.";
	public static string DoSomethingCardTextB => "Play 2 random cards from your <c=keyword>draw, discard or exhaust pile</c>.";
	public static string ImAlwaysRightCardName => "I'm Always Right!";

	public static readonly string[] ApologyFlavorTexts = new[]
	{
		"I'm sorry you feel that way",
		"Sowwee",
		"That wasn't my fault",
		"It wasn't a big deal, right?",
		"Being the ship's captain is so difficult!",
		"I said sorry, can we move on?",
		"I said sorry already!",
		"Are you sure it was my fault?",
		"We can do better",
		"Your forgiveness is accepted",
		"We can hug it out",
		"This was a traumatic experience for me",
		"I O U",
		"XOXO",
		"*sniffles*",
		"Pwease?",
		"My sincerest apology for whatever that was",
		"I couldn't do any better!!",
		"Let's see the bright side of this",
		"I'm sorry I guess",
		"I'm soooo sorry",
		"What happened again?",
		"You were at fault too",
		"I'll be the better man and say sorry",
		"Could we move on?",
		"This is awkward",
		"But it was funny, right?",
		"I hope that wasn't important",
		"This makes up for it, don't you think?",
		"We'll remember this as a funny memory",
		"I'll make it up to you",
		"There's always next time",
		"It's always me, isn't it?",
		"Oops! Was that yours?",
		"Uh oh! That wasn't okay",
		"All is good now",
		"We're okay now",
		"This is fine",
		"*sob*",
		"Less than slash three",
		"Apology #{0}"
	};
}
