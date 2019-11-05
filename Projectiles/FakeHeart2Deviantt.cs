﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Fargowiltas.Projectiles
{
    public class FakeHeart2Deviantt : ModProjectile
    {
        public override string Texture => "Fargowiltas/Projectiles/FakeHeartDeviantt";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fake Heart");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.timeLeft = 600;
            projectile.friendly = true;
            projectile.aiStyle = -1;

            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            float rand = Main.rand.Next(90, 111) * 0.01f * (Main.essScale * 0.5f);
            Lighting.AddLight(projectile.Center, 0.5f * rand, 0.1f * rand, 0.1f * rand);

            if (++projectile.localAI[0] == 30)
            {
                projectile.localAI[1] = projectile.velocity.ToRotation();
                projectile.velocity = Vector2.Zero;
            }

            if (--projectile.ai[1] == 0)
            {
                projectile.velocity = projectile.localAI[1].ToRotationVector2() * -20;
            }
            else if (projectile.ai[1] < 0)
            {
                if (projectile.ai[0] >= 0 && projectile.ai[0] < 200)
                {
                    int ai0 = (int)projectile.ai[0];
                    if (Main.npc[ai0].CanBeChasedBy())
                    {
                        double num4 = (Main.npc[ai0].Center - projectile.Center).ToRotation() - projectile.velocity.ToRotation();
                        if (num4 > Math.PI)
                            num4 -= 2.0 * Math.PI;
                        if (num4 < -1.0 * Math.PI)
                            num4 += 2.0 * Math.PI;
                        projectile.velocity = projectile.velocity.RotatedBy(num4 * 0.2f);
                    }
                    else
                    {
                        projectile.ai[0] = -1f;
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    if (++projectile.localAI[1] > 6f)
                    {
                        projectile.localAI[1] = 0f;
                        float maxDistance = 700f;
                        int possibleTarget = -1;
                        for (int i = 0; i < 200; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.CanBeChasedBy())
                            {
                                float npcDistance = projectile.Distance(npc.Center);
                                if (npcDistance < maxDistance)
                                {
                                    maxDistance = npcDistance;
                                    possibleTarget = i;
                                }
                            }
                        }
                        projectile.ai[0] = possibleTarget;
                        projectile.netUpdate = true;
                    }
                }
            }

            if (projectile.velocity != Vector2.Zero)
                projectile.rotation = projectile.velocity.ToRotation();

            projectile.rotation -= (float)Math.PI / 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Lovestruck, 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, lightColor.B, lightColor.A);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}