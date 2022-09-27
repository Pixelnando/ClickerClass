using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ClickerClass.Dusts;

namespace ClickerClass.Walls
{
	public class MiceBrickWall : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = ModContent.DustType<MiceDust>();
			ItemDrop = ModContent.ItemType<Items.Placeable.Mice.MiceBrickWall>();
			AddMapEntry(new Color(88, 92, 222));
		}
	}
}