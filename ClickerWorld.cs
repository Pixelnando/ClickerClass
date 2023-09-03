using ClickerClass.Items.Accessories;
using ClickerClass.Items.Weapons.Clickers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using ClickerClass.Utilities;

namespace ClickerClass
{
	public class ClickerWorld : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			//Biome Chest
			int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
			int indexToInsert = genIndex + 1;
			if (genIndex != -1)
			{
				tasks.Insert(indexToInsert, new PassLegacy($"Clicker Class: Biome Chests", GenerateBiomeChests));
			}
			
			genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			indexToInsert = genIndex + 1;
			if (genIndex != -1)
			{
				// Extra Chest Loot
				tasks.Insert(indexToInsert, new PassLegacy("Clicker Class: Extra Loot", GenerateExtraLoot));
			}
		}
		
		#region Chest Generation
		private static void GenerateBiomeChests(GenerationProgress progress, GameConfiguration configuration)
		{
			// Get dungeon size
			int MinX = GenVars.dMinX + 25;
			int MaxX = GenVars.dMaxX - 25;
			int MaxY = GenVars.dMaxY - 25;
			int[] ChestTypes = new int[] { ModContent.TileType<Tiles.DungeonChest>() };
			int[] ItemTypes = new int[] { ModContent.ItemType<MouseClicker>() };

			//progress.Message = $"{ClickerClass.mod.DisplayName}: " + BiomeChestsText.ToString();

			int rounds = 1;

			for (int i = 0; i < ChestTypes.Length * rounds; i++)
			{
				Chest chest = null;
				int attempts = 0;
				while (chest == null && attempts < 1000)
				{
					attempts++;
					int x = WorldGen.genRand.Next(MinX, MaxX);
					int y = WorldGen.genRand.Next((int)Main.worldSurface, MaxY);
					if (Main.wallDungeon[Main.tile[x, y].WallType] && !Main.tile[x, y].HasTile)
					{
						chest = AddChestWithDefaultLoot(x, y, (ushort)ChestTypes[i % ChestTypes.Length], 1, 2);
					}
				}
				if (chest != null)
				{
					chest.item[0].SetDefaults(ItemTypes[i % ChestTypes.Length]);
					chest.item[0].Prefix(-1);
				}
			}
		}
		
		public static Chest AddChestWithDefaultLoot(int i, int j, ushort type = TileID.Containers, uint emptySlots = 1, int Style = 0, bool notNearOtherChests = false)
		{
			//TODO 1.4 revisit for new chests (i.e. dead mans, see clicker class)
			Chest chest = null;
			while (j < Main.maxTilesY - 210)
			{
				if (!WorldGen.SolidTile(i, j))
				{
					j++;
					continue;
				}
				int chestIndex = WorldGen.PlaceChest(i - 1, j - 1, type, notNearOtherChests, Style);
				if (chestIndex < 0)
				{
					break;
				}

				chest = Main.chest[chestIndex];
				uint itemIndex = emptySlots;
				while (itemIndex == emptySlots)
				{
					Mod mod = ClickerClass.mod;
					bool AquaticChest = type == ModContent.TileType<Tiles.DungeonChest>();
					bool DesertChest = type == ModContent.TileType<Tiles.DungeonChest>();
					bool UnderworldChest = type == ModContent.TileType<Tiles.DungeonChest>();
					bool ScarletChest = type == ModContent.TileType<Tiles.DungeonChest>();
					bool VanillaChest = type == TileID.Containers;

					if (VanillaChest && j < Main.rockLayer) // default vanilla surface - cavern loot. Nothing changed here.
					{
						if (WorldGen.genRand.NextBool(3))
						{
							chest.item[itemIndex++].SetDefaults(ItemID.Bomb, WorldGen.genRand.Next(10, 20));
						}

						if (WorldGen.genRand.NextBool(5))
						{
							chest.item[itemIndex++].SetDefaults(ItemID.AngelStatue);
						}

						if (WorldGen.genRand.NextBool(3))
						{
							chest.item[itemIndex++].SetDefaults(ItemID.Rope, WorldGen.genRand.Next(50, 101));
						}

						if (WorldGen.genRand.NextBool())
						{
							chest.item[itemIndex++].SetDefaults(WorldGen.genRand.NextBool() ? GenVars.ironBar : GenVars.silverBar, WorldGen.genRand.Next(5, 15));
						}

						if (WorldGen.genRand.NextBool())
						{
							chest.item[itemIndex++].SetDefaults(WorldGen.genRand.NextBool() ? ItemID.WoodenArrow : ItemID.Shuriken, WorldGen.genRand.Next(25, 50));
						}

						if (WorldGen.genRand.NextBool())
						{
							chest.item[itemIndex++].SetDefaults(ItemID.LesserHealingPotion, WorldGen.genRand.Next(3, 6));
						}

						if (!WorldGen.genRand.NextBool(3))
						{
							int[] items = new int[] {
								ItemID.RegenerationPotion, ItemID.ShinePotion, ItemID.NightOwlPotion,
								ItemID.SwiftnessPotion, ItemID.ArcheryPotion, ItemID.GillsPotion,
								ItemID.HunterPotion, ItemID.MiningPotion, ItemID.TrapsightPotion
							};
							chest.item[itemIndex++].SetDefaults(WorldGen.genRand.Next(items), WorldGen.genRand.Next(1, 3));
						}
						if (!WorldGen.genRand.NextBool(3))
						{
							chest.item[itemIndex++].SetDefaults(ItemID.RecallPotion, WorldGen.genRand.Next(1, 3));
						}

						if (WorldGen.genRand.NextBool())
						{
							chest.item[itemIndex++].SetDefaults(ItemID.Torch, WorldGen.genRand.Next(10, 21));
						}

						if (WorldGen.genRand.NextBool())
						{
							chest.item[itemIndex++].SetDefaults(ItemID.SilverCoin, WorldGen.genRand.Next(50, 90));
						}

						continue;
					}
					// default cavern - underworld top loot + our loot					
					int cItem;
					bool HellChest = j > Main.maxTilesY - 205;
					if ((VanillaChest && !HellChest || ScarletChest) && WorldGen.genRand.NextBool(5))
					{
						cItem = ItemID.SuspiciousLookingEye;
						chest.item[itemIndex++].SetDefaults(cItem);
					}
					if (WorldGen.genRand.NextBool(3))
					{
						chest.item[itemIndex++].SetDefaults(ItemID.Dynamite);
					}

					if (!ScarletChest && !HellChest && WorldGen.genRand.NextBool(4))
					{
						chest.item[itemIndex++].SetDefaults(ItemID.JestersArrow, WorldGen.genRand.Next(25, 51));
					}

					if (WorldGen.genRand.NextBool())
					{
						cItem = WorldGen.genRand.NextBool() ? GenVars.goldBar : GenVars.silverBar;
						int addAmount = 0;
						
						chest.item[itemIndex++].SetDefaults(cItem, WorldGen.genRand.Next(3 + addAmount, 11 + addAmount * 2));
					}
					if (WorldGen.genRand.NextBool())
					{
						cItem = ItemID.FlamingArrow;
						int addAmount = 0;
						
						chest.item[itemIndex++].SetDefaults(cItem, WorldGen.genRand.Next(25 + addAmount, 51 + addAmount * 2));
					}
					if (!HellChest && WorldGen.genRand.NextBool())
					{
						chest.item[itemIndex++].SetDefaults(ItemID.HealingPotion, WorldGen.genRand.Next(3, 6));
					}
					if (!WorldGen.genRand.NextBool(HellChest ? 4 : 3))
					{
						int[] items = new int[] {
							ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion,
							ItemID.WaterWalkingPotion, ItemID.ArcheryPotion, ItemID.GravitationPotion
						};
						
						chest.item[itemIndex++].SetDefaults(WorldGen.genRand.Next(items), WorldGen.genRand.Next(1, 3));
					}
					if (WorldGen.genRand.NextBool(3) ^ HellChest)
					{
						int[] items = new int[] {
							ItemID.ThornsPotion, ItemID.WaterWalkingPotion, ItemID.InvisibilityPotion,
							ItemID.HunterPotion, ItemID.TeleportationPotion, ItemID.TrapsightPotion, ItemID.TrapsightPotion // yes, dangersense potions have double the chance as other potions in vanilla for some reason
						};
						
						chest.item[itemIndex++].SetDefaults(WorldGen.genRand.Next(items), WorldGen.genRand.Next(1, 3));
					}
					if (WorldGen.genRand.NextBool(HellChest ? 3 : 2))
					{
						cItem = ItemID.RecallPotion;
						int addAmount = 0;
						
						chest.item[itemIndex++].SetDefaults(cItem, WorldGen.genRand.Next(1, 3) * addAmount);
					}
					if (WorldGen.genRand.NextBool())
					{
						cItem = WorldGen.genRand.NextBool() ? ItemID.Torch : ItemID.Glowstick;

						chest.item[itemIndex++].SetDefaults(cItem, WorldGen.genRand.Next(15, 30));
					}
					if (WorldGen.genRand.NextBool())
					{
						int addAmount = ScarletChest ? 3 : HellChest ? 2 : 1;
						chest.item[itemIndex++].SetDefaults(ItemID.GoldCoin, WorldGen.genRand.Next(1, 3) * addAmount);
					}
				}
			}
			return chest;
		}
		#endregion

		private void GenerateExtraLoot(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Clicker Class: Generating Extra Loot";

			HashSet<ChestStyle> chestStyles = new HashSet<ChestStyle>
			{
				ChestStyle.Wooden, ChestStyle.Gold, ChestStyle.LockedGold, ChestStyle.RichMahogany,
				ChestStyle.Ivy, ChestStyle.LivingWood, ChestStyle.WebCovered, ChestStyle.Water,
				ChestStyle.Mushroom, ChestStyle.Granite, ChestStyle.Marble, ChestStyle.Lihzahrd,
				ChestStyle.LockedShadow, ChestStyle.Skyware, ChestStyle.Ice, ChestStyle.DeadMans
			};

			Dictionary<ChestStyle, List<Chest>> chestLists = new Dictionary<ChestStyle, List<Chest>>();

			for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
			{
				Chest chest = Main.chest[chestIndex];
				if (chest == null || !WorldGen.InWorld(chest.x, chest.y, 42)) // don't include chests generated outside the playable area of the map
				{
					continue;
				}

				Tile tile = Main.tile[chest.x, chest.y];
				if (chest.item == null || (tile.TileType != TileID.Containers && tile.TileType != TileID.Containers2))
				{
					continue;
				}

				int chestStyleOffset = 0;
				if (tile.TileType == TileID.Containers2)
				{
					chestStyleOffset = (int)ChestStyle.Containers2Offset;
				}
				int styleNum = tile.TileFrameX / 36 + chestStyleOffset;

				ChestStyle style = (ChestStyle)styleNum;
				if (chestStyles.Contains(style))
				{
					if (style == ChestStyle.LockedGold && !Main.wallDungeon[tile.WallType]) // not actually a dungeon chest, maybe some mod added this
					{
						continue;
					}

					if (style == ChestStyle.Wooden && Main.wallDungeon[tile.WallType]) // wooden chests generated inside the dungeon always have golden keys
					{
						continue;
					}

					//Mushroom is not recongized as gold here for the sake of the 'Mycelium Clicker'
					if (style == ChestStyle.Granite || style == ChestStyle.Marble) // consider all these as gold chests, since that's what vanilla does
					{
						style = ChestStyle.Gold;
					}

					if (!chestLists.ContainsKey(style))
					{
						chestLists[style] = new List<Chest>();
					}

					chestLists[style].Add(chest);
				}
			}

			if (chestLists.ContainsKey(ChestStyle.Gold))
			{
				//Make it more likely to appear by adding twice
				ReplaceRareItemsInChests(chestLists[ChestStyle.Gold], new int[] { ModContent.ItemType<EnchantedLED>(), ModContent.ItemType<EnchantedLED>() });

				var pyramidChests = new List<Chest>();
				foreach (var chest in chestLists[ChestStyle.Gold])
				{
					if (Main.tile[chest.x, chest.y].WallType == WallID.SandstoneBrick)
					{
						pyramidChests.Add(chest);
					}
				}

				AddRareItemToChests(pyramidChests, ModContent.ItemType<PharaohsClicker>(), 1f);
			}

			if (chestLists.ContainsKey(ChestStyle.DeadMans))
			{
				ReplaceRareItemsInChests(chestLists[ChestStyle.DeadMans], new int[] { ModContent.ItemType<FaultyClicker>() });
			}

			if (chestLists.ContainsKey(ChestStyle.Skyware))
			{
				ReplaceRareItemsInChests(chestLists[ChestStyle.Skyware], new int[] { ModContent.ItemType<StarryClicker>() });
			}

			if (chestLists.ContainsKey(ChestStyle.Ice))
			{
				ReplaceRareItemsInChests(chestLists[ChestStyle.Ice], new int[] { ModContent.ItemType<IcePack>() });
			}

			if (chestLists.ContainsKey(ChestStyle.Mushroom))
			{
				ReplaceRareItemsInChests(chestLists[ChestStyle.Mushroom], new int[] { ModContent.ItemType<MyceliumClicker>() });
			}

			if (chestLists.ContainsKey(ChestStyle.LockedGold))
			{
				ReplaceRareItemsInChests(chestLists[ChestStyle.LockedGold], new int[] { ModContent.ItemType<SlickClicker>() });
			}

			if (chestLists.ContainsKey(ChestStyle.LockedShadow))
			{
				ReplaceRareItemsInChests(chestLists[ChestStyle.LockedShadow], new int[] { ModContent.ItemType<UmbralClicker>() });
			}
		}

		public static void AddRareItemToChests(IList<Chest> chestList, int newItem, float chance, int min = 1, int max = 1)
		{
			for (int i = 0; i < chestList.Count; i++)
			{
				if (WorldGen.genRand.NextFloat() > chance)
				{
					continue;
				}

				Chest chest = chestList[i];
				//if (CountFreeSlotsInChest(chest) == 0)
				//{
				//	//No space left, don't attempt shifting OR adding
				//	continue;
				//}

				(int start, int end) = GetFirstConsecutiveItemChainIndexes(chest);
				if (start == 0 && end == chest.item.Length - 1)
				{
					//No space left, don't attempt shifting OR adding
					continue;
				}
				
				if (!(end == -1 || start > 0 || end == chest.item.Length - 1))
				{
					//Don't shift if empty, first slot is not occupied, or chain ends on the last slot
					ShiftSlotRangeBy1Up(chest, start, end);
				}
				//Else no point in shifting, just insert directly

				//Insert in the now guaranteed empty slot
				Item item = chest.item[0];
				int stack = item.stack;
				int maxExclusive = max + 1;
				if (min > 0 && maxExclusive > min)
				{
					stack = max > min ? WorldGen.genRand.Next(min, maxExclusive) : min;
				}
				item.SetDefaults(newItem);
				item.stack = stack;
			}
		}

		public static int CountFreeSlotsInChest(Chest chest)
		{
			int count = 0;
			for (int k = 0; k < chest.item.Length; k++)
			{
				Item item = chest.item[k];
				if (item == null || !item.IsAir)
				{
					continue;
				}

				count++;
			}

			return count;
		}

		public static (int start, int end) GetFirstConsecutiveItemChainIndexes(Chest chest)
		{
			int start = -1;
			int end = -1;
			for (int k = 0; k < chest.item.Length; k++)
			{
				Item item = chest.item[k];
				if (item == null || item.IsAir)
				{
					if (end != -1)
					{
						//Found new empty slot: sequence has ended
						break;
					}

					continue;
				}

				if (start == -1)
				{
					start = k;
				}

				end = k;
			}

			return (start, end);
		}

		public static void ShiftSlotRangeBy1Up(Chest chest, int start, int end)
		{
			//Start from the end (on the empty slot after), shift the slot one up, repeat
			for (int k = end + 1; k > start; k--)
			{
				ref Item itemToShiftTo = ref chest.item[k];
				Utils.Swap(ref chest.item[k], ref chest.item[k - 1]);
			}
		}

		public static void ReplaceRareItemsInChests(IList<Chest> chestList, IList<int> itemsToPlaceInChests, int rareSlots = 1, Func<int, IList<Chest>> generateChestFunc = null)
		{
			Dictionary<int, List<Chest>> chestsWithItem = new Dictionary<int, List<Chest>>(); // A dictionary where the key is an item id, and the value is a list of chests the item is in
			List<Chest> availableChests = new List<Chest>(); // A list of chests we can place our items into

			int itemCount = itemsToPlaceInChests.Count;
			int itemChoice = 0;
			int slot = 0;

			for (int i = 0; i < chestList.Count; i++) // Loop through all chest in the list provided as a parameter
			{
				Chest chest = chestList[i];
				if (chest.item[slot] == null || chest.item[slot].IsAir)
				{
					continue;
				}

				if (!chestsWithItem.ContainsKey(chest.item[slot].type)) // check if the item in the slot already has an entry in the dictionary
				{
					chestsWithItem[chest.item[slot].type] = new List<Chest>(); // if not, create it
				}

				chestsWithItem[chest.item[slot].type].Add(chest); // then we add the chest to the list of chests we can find the item in
			}

			foreach (var list in chestsWithItem.Values) // then, we loop through all entries in the dictionary
			{
				list.RemoveAt(WorldGen.genRand.Next(list.Count)); // and remove one chest from the list of chest in the current entry at random
				if (list.Count > 0) // if the list still has chests left then an item generated in more than 1 chest
				{
					availableChests.AddRange(list); // so we add the chests left to the list of chests we can add items to
				}
			}

			int itemsInChestsCount = chestsWithItem.Keys.Count;

			// now comes the hard part

			if (availableChests.Count < itemCount) // If we got bad luck and didn't get enough chests to place our items into
			{
				int neededChests = itemCount - availableChests.Count;
				if (generateChestFunc != null) // we see if we provided a method to generate more chests
				{
					// if we did, we just generate enough to add our items
					IList<Chest> chests = generateChestFunc(neededChests);
					availableChests.AddRange(chests); // then we add the chests to our list
					for (int i = 0; i < chests.Count; i++)
					{
						chestList.Add(chests[i]); // and also add them to the list provided as a parameter
					}
				}
				else // if we didn't
				{
					availableChests = new List<Chest>(chestList); // we reset the list of available chests to the list of chest we were provided at the beginning
					while (availableChests.Count > neededChests) // but remove chest at random until we only have enough chests to generate our items
					{
						availableChests.RemoveAt(WorldGen.genRand.Next(availableChests.Count));
					}

					for (int i = 0; i < availableChests.Count; i++) // then we loop through chests on the list
					{
						Chest chest = availableChests[i];
						for (int k = chest.item.Length - 1; k > slot + rareSlots; k--) // then through all items in the chest, except for the ones specified as rare
						{
							chest.item[k] = chest.item[k - 1]; // and make room for our item
						}

						chest.item[slot + rareSlots] = new Item();
						// this means that if for some reason a chest was full, the last item on the chest will be deleted... >:)
					}
					slot += rareSlots;
					rareSlots = 1;
				}
			}

			while (availableChests.Count > 0) // now we generate our items
			{
				int index = WorldGen.genRand.Next(availableChests.Count);
				Chest chest = availableChests[index]; // we chose a random chest on the list

				// we check if we already added at least one of each item we wanted to add
				if (itemChoice < itemCount || WorldGen.genRand.Next(itemsInChestsCount + itemCount) < itemCount) // if we did, we randomly generate a number to see if we can generate another one
				{
					int tempRareSlots = rareSlots;
					if (chest.item[slot].type == ItemID.FlareGun) // the flaregun is a special case, is the only item in vanilla that generates along with another item (flares) on a pool (gold chest) full of single rare items
					{
						tempRareSlots = 2;
					}

					while (tempRareSlots > 1) // if the amount of slots the rare items take is higher than 1
					{
						for (int i = slot + tempRareSlots - 1; i < chest.item.Length - 1; i++) // since we are replacing the rare items, we remove all of them
						{
							chest.item[i] = chest.item[i + 1];
						}

						chest.item[chest.item.Length - 1] = new Item();
						tempRareSlots--;
					}
					chest.item[slot].SetDefaults(itemChoice < itemCount ? itemsToPlaceInChests[itemChoice] : WorldGen.genRand.Next(itemsToPlaceInChests)); // we replace the item in the slot with ours
					chest.item[slot].Prefix(-1); // and prefix it for good measure
					itemChoice++;
				}
				availableChests.RemoveAt(index); // then we remove the current chest from the list
			}
			return; // note that this doesn't handle cases where the list of chests provided as a parameter is smaller than the amount of items to generate
		}
	}
}
