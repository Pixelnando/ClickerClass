using ClickerClass.Buffs;
using ClickerClass.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ClickerClass.Projectiles
{
	public class SeafoamClickerPro : ClickerProjectile
	{
		public bool Spawned
		{
			get => Projectile.ai[0] == 1f;
			set => Projectile.ai[0] = value ? 1f : 0f;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 10;
			Projectile.alpha = 255;
			Projectile.extraUpdates = 3;
		}

		public override void AI()
		{
			if (!Spawned)
			{
				Spawned = true;

				SoundEngine.PlaySound(SoundID.Item86, Projectile.Center);

				for (int k = 0; k < 30; k++)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 99, Main.rand.NextFloat(-15f, 15f), Main.rand.NextFloat(-15f, 15f), 50, default, 1.5f);
					dust.noGravity = true;
				}

				for (int u = 0; u < Main.maxNPCs; u++)
				{
					NPC target = Main.npc[u];
					if (target.CanBeChasedBy() && target.DistanceSQ(Projectile.Center) < 200 * 200)
					{
						target.AddBuff(ModContent.BuffType<Seafoam>(), 300, false);

						for (int i = 0; i < 15; i++)
						{
							int index = Dust.NewDust(target.position, target.width, target.height, 99, 0f, 0f, 100, default(Color), 1.5f);
							Dust dust = Main.dust[index];
							dust.noGravity = true;
							dust.velocity *= 0.75f;
							int x = Main.rand.Next(-50, 51);
							int y = Main.rand.Next(-50, 51);
							dust.position.X += x;
							dust.position.Y += y;
							dust.velocity.X = -x * 0.075f;
							dust.velocity.Y = -y * 0.075f;
						}
					}
				}
			}
		}
	}
}
