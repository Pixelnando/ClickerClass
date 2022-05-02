﻿using Terraria;
using Terraria.UI;
using ClickerClass.Utilities;

namespace ClickerClass.UI
{
	internal class FMedalGauge : MedalGaugeBase
	{
		public FMedalGauge() : base("ClickerClass: F Medal Gauge", InterfaceScaleType.UI) { }

		protected override int GetValue()
		{
			Player player = Main.LocalPlayer;
			ClickerPlayer clickerPlayer = player.GetClickerPlayer();
			return clickerPlayer.accFMedalAmount;
		}

		protected override int TextColor()
		{
			Player player = Main.LocalPlayer;
			ClickerPlayer clickerPlayer = player.GetClickerPlayer();
			return clickerPlayer.AccSMedal ? Terraria.ID.ItemRarityID.Blue : Terraria.ID.ItemRarityID.LightPurple;
		}

		protected override string TexturePath()
		{
			return "UI/FMedalGauge_Sheet";
		}
	}
}