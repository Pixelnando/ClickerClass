﻿using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace ClickerClass
{
	public class ClickerConfigClient : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		// Automatically set by tModLoader
		public static ClickerConfigClient Instance;

		[Header("DisplayOptions")]

		[DefaultValue(true)]
		public bool ShowClassTags;

		[DefaultValue(true)]
		public bool ShowEffectSuggestion;

		[DefaultValue(true)]
		public bool ShowCustomCursors;

		[DefaultValue(false)]
		public bool ShowClickIndicator;

		[DefaultValue(true)]
		public bool ShowOthersClickIndicator;

		[Header("GameplayOptions")]

		[BackgroundColor(255, 175, 100)]
		[DefaultValue(true)]
		public bool ToggleAutoreuseLimiter;
		
		[BackgroundColor(255, 175, 100)]
		[Range(1, 10), Increment(1), DefaultValue(1)]
		public int ToggleAutoreuseLimiterValue { get; set; }
		
		[BackgroundColor(255, 175, 100)]
		[DefaultValue(true)]
		public bool ToggleAutoreuseLimiterAccessibility;
	}
}
