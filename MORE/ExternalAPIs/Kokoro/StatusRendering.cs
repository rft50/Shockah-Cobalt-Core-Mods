﻿using System.Collections.Generic;

namespace Shockah.Kokoro;

public partial interface IKokoroApi
{
	public partial interface IV2
	{
		IStatusRenderingApi StatusRendering { get; }

		public interface IStatusRenderingApi
		{
			void RegisterHook(IHook hook, double priority = 0);
			void UnregisterHook(IHook hook);
			
			Color DefaultActiveStatusBarColor { get; }
			Color DefaultInactiveStatusBarColor { get; }
			
			public interface IHook : IKokoroV2ApiHook
			{
				IEnumerable<(Status Status, double Priority)> GetExtraStatusesToShow(IGetExtraStatusesToShowArgs args) => [];
				bool? ShouldShowStatus(IShouldShowStatusArgs args) => null;
				bool? ShouldOverrideStatusRenderingAsBars(IShouldOverrideStatusRenderingAsBarsArgs args) => null;
				(IReadOnlyList<Color> Colors, int? BarTickWidth) OverrideStatusRendering(IOverrideStatusRenderingArgs args) => new(); // TODO: combine with `ShouldOverrideStatusRenderingAsBars`
				List<Tooltip> OverrideStatusTooltips(IOverrideStatusTooltipsArgs args) => args.Tooltips;
				
				public interface IGetExtraStatusesToShowArgs
				{
					State State { get; }
					Combat Combat { get; }
					Ship Ship { get; }
				}
				
				public interface IShouldShowStatusArgs
				{
					State State { get; }
					Combat Combat { get; }
					Ship Ship { get; }
					Status Status { get; }
					int Amount { get; }
				}
				
				public interface IShouldOverrideStatusRenderingAsBarsArgs
				{
					State State { get; }
					Combat Combat { get; }
					Ship Ship { get; }
					Status Status { get; }
					int Amount { get; }
				}
				
				public interface IOverrideStatusRenderingArgs
				{
					State State { get; }
					Combat Combat { get; }
					Ship Ship { get; }
					Status Status { get; }
					int Amount { get; }
				}
				
				public interface IOverrideStatusTooltipsArgs
				{
					Status Status { get; }
					int Amount { get; }
					Ship? Ship { get; }
					List<Tooltip> Tooltips { get; }
				}
			}
		}
	}
}
