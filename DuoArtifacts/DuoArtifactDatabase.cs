﻿using CobaltCoreModding.Definitions.ExternalItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.DuoArtifacts;

internal sealed class DuoArtifactDatabase
{
	private const double SingleColorTransitionAnimationLengthSeconds = 1;

	internal ExternalDeck DuoArtifactDeck { get; set; } = null!;
	internal ExternalDeck TrioArtifactDeck { get; set; } = null!;
	internal ExternalDeck ComboArtifactDeck { get; set; } = null!;
	private readonly Dictionary<HashSet<Deck>, HashSet<Type>> ComboToTypeDictionary = new(HashSet<Deck>.CreateSetComparer());
	private readonly Dictionary<Type, HashSet<Deck>> TypeToComboDictionary = [];

	public bool IsDuoArtifactType(Type type)
		=> TypeToComboDictionary.ContainsKey(type);

	public bool IsDuoArtifact(Artifact artifact)
		=> IsDuoArtifactType(artifact.GetType());

	public IReadOnlySet<Deck>? GetDuoArtifactTypeOwnership(Type type)
		=> TypeToComboDictionary.GetValueOrDefault(type);

	public IReadOnlySet<Deck>? GetDuoArtifactOwnership(Artifact artifact)
		=> GetDuoArtifactTypeOwnership(artifact.GetType());

	public IEnumerable<Type> GetAllDuoArtifactTypes()
		=> TypeToComboDictionary.Keys;

	public IEnumerable<Artifact> InstantiateAllDuoArtifacts()
		=> TypeToComboDictionary.Keys.Select(t => (Artifact)Activator.CreateInstance(t)!);

	public IEnumerable<Type> GetExactDuoArtifactTypes(IEnumerable<Deck> combo)
		=> ComboToTypeDictionary.TryGetValue(FixCombo(new HashSet<Deck>(combo)), out var types) ? types : Array.Empty<Type>();

	public IEnumerable<Artifact> InstantiateExactDuoArtifacts(IEnumerable<Deck> combo)
		=> GetExactDuoArtifactTypes(combo).Select(t => (Artifact)Activator.CreateInstance(t)!);

	public IEnumerable<Type> GetMatchingDuoArtifactTypes(IEnumerable<Deck> combo)
	{
		var comboSet = new HashSet<Deck>(combo);
		foreach (var (keySet, types) in ComboToTypeDictionary)
			if (!keySet.Except(comboSet).Any())
				foreach (var type in types)
					yield return type;
	}

	public IEnumerable<Artifact> InstantiateMatchingDuoArtifacts(IEnumerable<Deck> combo)
		=> GetMatchingDuoArtifactTypes(combo).Select(t => (Artifact)Activator.CreateInstance(t)!);

	public Color GetDynamicColorForArtifact(Artifact artifact, Deck? ignoreDeck = null)
	{
		var colors = GetDuoArtifactOwnership(artifact)
			?.Select(c => c == Deck.catartifact ? Deck.colorless : c)
			.OrderBy(NewRunOptions.allChars.IndexOf)
			.Where(key => key != ignoreDeck)
			.Select(key => DB.decks[key].color)
			.ToList();
		if (colors is null || colors.Count == 0)
			return DB.decks[artifact.GetMeta().owner].color;
		if (colors.Count == 1)
			return colors[0];

		static (Color, Color, double) GetLerpInfo(List<Color> colors, double totalFraction)
		{
			double singleFraction = 1.0 / colors.Count;
			int whichFraction = ((int)Math.Round(totalFraction / singleFraction) + colors.Count - 1) % colors.Count;
			double fractionStart = singleFraction * whichFraction;
			double fractionEnd = singleFraction * (whichFraction + 1);
			double fraction = (totalFraction - fractionStart) / (fractionEnd - fractionStart);
			return (colors[whichFraction], colors[(whichFraction + 1) % colors.Count], fraction);
		}

		double animationLength = colors.Count * SingleColorTransitionAnimationLengthSeconds;
		double animationPosition = ModEntry.Instance.KokoroApi.TotalGameTime.TotalSeconds % animationLength;
		double totalFraction = animationPosition / animationLength;
		var (fromColor, toColor, fraction) = GetLerpInfo(colors, totalFraction);
		double lerpFraction = Math.Sin(fraction * Math.PI) * 0.5 + 0.5;
		return Color.Lerp(fromColor, toColor, lerpFraction);
	}

	public void RegisterDuoArtifact(Type type, IEnumerable<Deck> combo)
	{
		if (TypeToComboDictionary.ContainsKey(type))
			throw new ArgumentException($"Artifact type {type} is already registered as a duo.");
		if (!type.IsAssignableTo(typeof(Artifact)))
			throw new ArgumentException($"Type {type} is not a subclass of the {typeof(Artifact)} type.");

		var comboSet = FixCombo(new HashSet<Deck>(combo));
		if (comboSet.Count < 2)
			throw new ArgumentException("Tried to register a duo artifact for less than 2 characters.");
		TypeToComboDictionary[type] = comboSet;

		if (!ComboToTypeDictionary.TryGetValue(comboSet, out var types))
		{
			types = [];
			ComboToTypeDictionary[comboSet] = types;
		}
		types.Add(type);
	}

	private static HashSet<Deck> FixCombo(HashSet<Deck> combo)
	{
		if (!combo.Contains(Deck.catartifact))
			return combo;

		var result = new HashSet<Deck>(combo);
		result.Remove(Deck.catartifact);
		result.Add(Deck.colorless);
		return result;
	}
}