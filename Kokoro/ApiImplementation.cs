﻿using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModManifests;
using daisyowl.text;
using Nanoray.Pintail;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Shockah.Kokoro;

public sealed partial class ApiImplementation(
	IManifest manifest
) : IKokoroApi
{
	private static ModEntry Instance => ModEntry.Instance;

	private static readonly Dictionary<Type, ConditionalWeakTable<object, object?>> ProxyCache = [];

	#region Generic
	public TimeSpan TotalGameTime
		=> TimeSpan.FromSeconds(MG.inst.g?.time ?? 0);

	public IEnumerable<Card> GetCardsEverywhere(State state, bool hand = true, bool drawPile = true, bool discardPile = true, bool exhaustPile = true)
	{
		if (drawPile)
			foreach (var card in state.deck)
				yield return card;
		if (state.route is Combat combat)
		{
			if (hand)
				foreach (var card in combat.hand)
					yield return card;
			if (discardPile)
				foreach (var card in combat.discard)
					yield return card;
			if (exhaustPile)
				foreach (var card in combat.exhausted)
					yield return card;
		}
	}

	public bool TryProxy<T>(object @object, [MaybeNullWhen(false)] out T proxy) where T : class
	{
		if (!typeof(T).IsInterface)
		{
			proxy = null;
			return false;
		}
		if (!ProxyCache.TryGetValue(typeof(T), out var table))
		{
			table = [];
			ProxyCache[typeof(T)] = table;
		}
		if (table.TryGetValue(@object, out var rawProxy))
		{
			proxy = (T)rawProxy!;
			return rawProxy is not null;
		}

		var newNullableProxy = Instance.Helper.Utilities.ProxyManager.TryProxy<string, T>(@object, "Unknown", Instance.Name, out var newProxy) ? newProxy : null;
		table.AddOrUpdate(@object, newNullableProxy);
		proxy = newNullableProxy;
		return newNullableProxy is not null;
	}

	#endregion

	#region ExtensionData
	public void RegisterTypeForExtensionData(Type type)
	{
	}

	public T GetExtensionData<T>(object o, string key)
		=> Instance.ExtensionDataManager.GetExtensionData<T>(manifest, o, key);

	public bool TryGetExtensionData<T>(object o, string key, [MaybeNullWhen(false)] out T data)
		=> Instance.ExtensionDataManager.TryGetExtensionData(manifest, o, key, out data);

	public T ObtainExtensionData<T>(object o, string key, Func<T> factory)
		=> Instance.ExtensionDataManager.ObtainExtensionData(manifest, o, key, factory);

	public T ObtainExtensionData<T>(object o, string key) where T : new()
		=> Instance.ExtensionDataManager.ObtainExtensionData<T>(manifest, o, key);

	public bool ContainsExtensionData(object o, string key)
		=> Instance.ExtensionDataManager.ContainsExtensionData(manifest, o, key);

	public void SetExtensionData<T>(object o, string key, T data)
		=> Instance.ExtensionDataManager.SetExtensionData(manifest, o, key, data);

	public void RemoveExtensionData(object o, string key)
		=> Instance.ExtensionDataManager.RemoveExtensionData(manifest, o, key);
	#endregion

	#region CardRenderHook
	public void RegisterCardRenderHook(ICardRenderHook hook, double priority)
		=> Instance.CardRenderManager.Register(hook, priority);

	public void UnregisterCardRenderHook(ICardRenderHook hook)
		=> Instance.CardRenderManager.Unregister(hook);

	public Font PinchCompactFont
		=> ModEntry.Instance.Content.PinchCompactFont;
	#endregion

	#region Actions
	public IKokoroApi.IActionApi Actions { get; } = new ActionApiImplementation();

	public sealed partial class ActionApiImplementation : IKokoroApi.IActionApi
	{
		public CardAction MakeExhaustEntireHandImmediate()
			=> new AExhaustEntireHandImmediate();

		public CardAction MakePlaySpecificCardFromAnywhere(int cardId, bool showTheCardIfNotInHand = true)
			=> new APlaySpecificCardFromAnywhere { CardId = cardId, ShowTheCardIfNotInHand = showTheCardIfNotInHand };

		public CardAction MakePlayRandomCardsFromAnywhere(IEnumerable<int> cardIds, int amount = 1, bool showTheCardIfNotInHand = true)
			=> new APlayRandomCardsFromAnywhere { CardIds = cardIds.ToHashSet(), Amount = amount, ShowTheCardIfNotInHand = showTheCardIfNotInHand };

		public CardAction MakeContinue(out Guid id)
		{
			id = Guid.NewGuid();
			return new AContinue { Id = id, Continue = true };
		}

		public CardAction MakeContinued(Guid id, CardAction action)
			=> new AContinued { Id = id, Continue = true, Action = action };

		public IEnumerable<CardAction> MakeContinued(Guid id, IEnumerable<CardAction> action)
			=> action.Select(a => MakeContinued(id, a));

		public CardAction MakeStop(out Guid id)
		{
			id = Guid.NewGuid();
			return new AContinue { Id = id, Continue = false };
		}

		public CardAction MakeStopped(Guid id, CardAction action)
			=> new AContinued { Id = id, Continue = false, Action = action };

		public IEnumerable<CardAction> MakeStopped(Guid id, IEnumerable<CardAction> action)
			=> action.Select(a => MakeStopped(id, a));

		public CardAction MakeSpoofed(CardAction renderAction, CardAction realAction)
			=> new ASpoofed { RenderAction = renderAction, RealAction = realAction };

		public CardAction MakeHidden(CardAction action, bool showTooltips = false)
			=> new AHidden { Action = action, ShowTooltips = showTooltips };

		public AVariableHint SetTargetPlayer(AVariableHint action, bool targetPlayer)
		{
			var copy = Mutil.DeepCopy(action);
			Instance.Api.SetExtensionData(copy, "targetPlayer", targetPlayer);
			return copy;
		}

		public AVariableHint MakeEnergyX(AVariableHint? action = null, bool energy = true, int? tooltipOverride = null)
		{
			var copy = action is null ? new() : Mutil.DeepCopy(action);
			copy.status = Status.tempShield; // it doesn't matter, but it has to be *anything*
			Instance.Api.SetExtensionData(copy, "energy", energy);
			Instance.Api.SetExtensionData(copy, "energyTooltipOverride", tooltipOverride);
			return copy;
		}

		public AStatus MakeEnergy(AStatus action, bool energy = true)
		{
			var copy = Mutil.DeepCopy(action);
			copy.targetPlayer = true;
			Instance.Api.SetExtensionData(copy, "energy", energy);
			return copy;
		}

		public ACardOffering WithDestination(ACardOffering action, CardDestination? destination, bool? insertRandomly = null)
		{
			var copy = Mutil.DeepCopy(action);

			if (destination is null)
				Instance.Api.RemoveExtensionData(copy, "destination");
			else
				Instance.Api.SetExtensionData(copy, "destination", destination.Value);

			if (insertRandomly is null)
				Instance.Api.RemoveExtensionData(copy, "destinationInsertRandomly");
			else
				Instance.Api.SetExtensionData(copy, "destinationInsertRandomly", insertRandomly.Value);

			return copy;
		}

		public CardReward WithDestination(CardReward route, CardDestination? destination, bool? insertRandomly = null)
		{
			var copy = Mutil.DeepCopy(route);

			if (destination is null)
				Instance.Api.RemoveExtensionData(copy, "destination");
			else
				Instance.Api.SetExtensionData(copy, "destination", destination.Value);

			if (insertRandomly is null)
				Instance.Api.RemoveExtensionData(copy, "destinationInsertRandomly");
			else
				Instance.Api.SetExtensionData(copy, "destinationInsertRandomly", insertRandomly.Value);

			return copy;
		}

		public List<CardAction> GetWrappedCardActions(CardAction action)
			=> Instance.WrappedActionManager.GetWrappedCardActions(action).ToList();

		public List<CardAction> GetWrappedCardActionsRecursively(CardAction action)
			=> Instance.WrappedActionManager.GetWrappedCardActionsRecursively(action, includingWrapperActions: false).ToList();

		public List<CardAction> GetWrappedCardActionsRecursively(CardAction action, bool includingWrapperActions)
			=> Instance.WrappedActionManager.GetWrappedCardActionsRecursively(action, includingWrapperActions).ToList();

		public void RegisterWrappedActionHook(IWrappedActionHook hook, double priority)
			=> Instance.WrappedActionManager.Register(hook, priority);

		public void UnregisterWrappedActionHook(IWrappedActionHook hook)
			=> Instance.WrappedActionManager.Unregister(hook);
	}
	#endregion

	#region ComplexActions
	public IKokoroApi.IConditionalActionApi ConditionalActions { get; } = new ConditionalActionApiImplementation();
	public IKokoroApi.IActionCostApi ActionCosts { get; } = new ActionCostApiImplementation();

	public sealed class ConditionalActionApiImplementation : IKokoroApi.IConditionalActionApi
	{
		public CardAction Make(IKokoroApi.IConditionalActionApi.IBoolExpression expression, CardAction action, bool fadeUnsatisfied = true)
			=> new AConditional { Expression = expression, Action = action, FadeUnsatisfied = fadeUnsatisfied };

		public IKokoroApi.IConditionalActionApi.IIntExpression Constant(int value)
			=> new ConditionalActionIntConstant(value);

		public IKokoroApi.IConditionalActionApi.IIntExpression HandConstant(int value)
			=> new ConditionalActionHandConstant(value);

		public IKokoroApi.IConditionalActionApi.IIntExpression XConstant(int value)
			=> new ConditionalActionXConstant(value);

		public IKokoroApi.IConditionalActionApi.IIntExpression ScalarMultiplier(IKokoroApi.IConditionalActionApi.IIntExpression expression, int scalar)
			=> new ConditionalActionScalarMultiplier(expression, scalar);

		public IKokoroApi.IConditionalActionApi.IBoolExpression HasStatus(Status status, bool targetPlayer = true, bool countNegative = false)
			=> new ConditionalActionHasStatusExpression(status, targetPlayer, countNegative);

		public IKokoroApi.IConditionalActionApi.IIntExpression Status(Status status, bool targetPlayer = true)
			=> new ConditionalActionStatusExpression(status, targetPlayer);

		public IKokoroApi.IConditionalActionApi.IBoolExpression Equation(
			IKokoroApi.IConditionalActionApi.IIntExpression lhs,
			IKokoroApi.IConditionalActionApi.EquationOperator @operator,
			IKokoroApi.IConditionalActionApi.IIntExpression rhs,
			IKokoroApi.IConditionalActionApi.EquationStyle style,
			bool hideOperator = false
		)
			=> new ConditionalActionEquation(lhs, @operator, rhs, style, hideOperator);
	}

	public sealed class ActionCostApiImplementation : IKokoroApi.IActionCostApi
	{
		public CardAction Make(IKokoroApi.IActionCostApi.IActionCost cost, CardAction action)
			=> new AResourceCost { Costs = [cost], Action = action };

		public CardAction Make(IReadOnlyList<IKokoroApi.IActionCostApi.IActionCost> costs, CardAction action)
			=> new AResourceCost { Costs = costs.ToList(), Action = action };

		public IKokoroApi.IActionCostApi.IActionCost Cost(IReadOnlyList<IKokoroApi.IActionCostApi.IResource> potentialResources, int amount = 1, int? iconOverlap = null, Spr? costUnsatisfiedIcon = null, Spr? costSatisfiedIcon = null, int? iconWidth = null, IKokoroApi.IActionCostApi.CustomCostTooltipProvider? customTooltipProvider = null)
			=> new ActionCostImpl(potentialResources, amount, iconOverlap, costUnsatisfiedIcon, costSatisfiedIcon, iconWidth, customTooltipProvider);

		public IKokoroApi.IActionCostApi.IActionCost Cost(IKokoroApi.IActionCostApi.IResource resource, int amount = 1, int? iconOverlap = null, IKokoroApi.IActionCostApi.CustomCostTooltipProvider? customTooltipProvider = null)
			=> new ActionCostImpl(new List<IKokoroApi.IActionCostApi.IResource> () { resource }, amount, iconOverlap, null, null, null, customTooltipProvider);

		public IKokoroApi.IActionCostApi.IResource StatusResource(Status status, Spr costUnsatisfiedIcon, Spr costSatisfiedIcon, int? iconWidth = null)
			=> new ActionCostStatusResource(status, target: IKokoroApi.IActionCostApi.StatusResourceTarget.Player, costUnsatisfiedIcon, costSatisfiedIcon, iconWidth);

		public IKokoroApi.IActionCostApi.IResource StatusResource(Status status, IKokoroApi.IActionCostApi.StatusResourceTarget target, Spr costUnsatisfiedIcon, Spr costSatisfiedIcon, int? iconWidth = null)
			=> new ActionCostStatusResource(status, target, costUnsatisfiedIcon, costSatisfiedIcon, iconWidth);

		public IKokoroApi.IActionCostApi.IResource EnergyResource()
			=> new ActionCostEnergyResource();
	}
	#endregion
}