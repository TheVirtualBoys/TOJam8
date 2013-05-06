using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class Projectile
	{
		double posX;
		double posY;
		double velX;
		double velY;

		PhysicsSprite.PlayerType creatorId;
		PhysicsSprite.GlowType glowType;

		Animation ani;

		bool inHitState = false;

		const int ANI_RED_ORB_FLY = 27;
		const int ANI_RED_ORB_HIT = 29;
		const int ANI_BLUE_ORB_FLY = 28;
		const int ANI_BLUE_ORB_HIT = 30;

		public Projectile(double pX, double pY, double velX, double velY, PhysicsSprite.PlayerType creatorId, PhysicsSprite.GlowType glowType)
		{
			this.posX = pX;
			this.posY = pY;
			this.velX = velX / 100;
			this.velY = velY / 100;
			this.creatorId = creatorId;
			this.glowType = glowType;

			ani = new Animation(Game1.Instance.gameData.animations[getGlowFlyAniIndex(glowType)]);
			ani.start();
		}

		private int getGlowFlyAniIndex(PhysicsSprite.GlowType glowType)
		{
			return (glowType == PhysicsSprite.GlowType.Red_Glow) ? ANI_RED_ORB_FLY : ANI_BLUE_ORB_FLY;
		}

		private int getGlowHitAniIndex(PhysicsSprite.GlowType glowType)
		{
			return (glowType == PhysicsSprite.GlowType.Red_Glow) ? ANI_RED_ORB_HIT : ANI_BLUE_ORB_HIT;
		}

		public void update(GameTime gameTime)
		{
			ani.update(gameTime);
			updatePosition(gameTime);

			if (inHitState)
			{
				if (ani.IsFinished)
				{
					destroyProjectile();
				}
			}
			else if (posX < 0 || posX > Game1.Instance.gameData.ScreenWidth)
			{
				destroyProjectile();
			}
			else if (posY < 0 || posY > Game1.Instance.gameData.ScreenHeight)
			{
				destroyProjectile();
			}
			else if (collidesWithPlayer())
			{
				if (playerGlowMatches(getOtherPlayer(), glowType))
				{
					reverseDirection();
				}
				else
				{
					addScoreToPlayer(getPlayer());
					affectPlayer(getOtherPlayer());
					playHitAni();
				}


			}
		}

		private void updatePosition(GameTime gameTime)
		{
			//don't update the position once we've been hit
			if (inHitState)
				return;

			//TODO
			posX += velX;
			posY += velY;
		}

		private void playHitAni()
		{
			ani = new Animation(Game1.Instance.gameData.animations[getGlowHitAniIndex(glowType)]);
			ani.start();
		}

		private void reverseDirection()
		{
			//swap the creator
			creatorId = (creatorId == PhysicsSprite.PlayerType.Player1) ? PhysicsSprite.PlayerType.Player2 : PhysicsSprite.PlayerType.Player1;

			//reverse the direction
			velX = -velX;
			velY = -velY;
		}

		private bool playerGlowMatches(PhysicsSprite player, PhysicsSprite.GlowType glow)
		{
			return player.PlayerGlowType == glow;
		}

		public void draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			drawFrame(ani.CurFrame, spriteBatch);
		}

		protected void drawFrame(Frame frame, SpriteBatch spriteBatch)
		{
			if (frame == null)
				return;

			int numComponents = frame.NumComponents;
			for (int i = 0; i < numComponents; ++i)
			{
				FrameComponent component = frame.getComponent(i);
				TileSet tileSet = Game1.Instance.gameData.tileSets[component.TileSetIndex];
				Rectangle tileDims = component.getTileRect(tileSet);

				Rectangle pos = new Rectangle(Left + component.Left, Top + component.Top, tileDims.Width, tileDims.Height);

				spriteBatch.Draw(tileSet.texture, pos, tileDims, Color.White);
			}
		}


		private void destroyProjectile()
		{
			Game1.Instance.gameData.projectilesToRemove.Add(this);
		}

		private PhysicsSprite getPlayer()
		{
			return (creatorId == PhysicsSprite.PlayerType.Player1) ? Game1.Instance.player1 : Game1.Instance.player2;
		}

		private PhysicsSprite getOtherPlayer()
		{
			return (creatorId == PhysicsSprite.PlayerType.Player1) ? Game1.Instance.player2 : Game1.Instance.player1;
		}

		private void affectPlayer(PhysicsSprite player)
		{
			//TODO:
		}

		private void addScoreToPlayer(PhysicsSprite player)
		{
			//TODO:
		}

		private bool collidesWithPlayer()
		{
			//TODO:
			return false;
		}

		private int Left
		{
			get { return (int)posX; }
		}

		private int Top
		{
			get { return (int)posY; }
		}
	}
}
