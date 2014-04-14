using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class ProjectileLayer : Layer
	{
		GameData gameData;

		public ProjectileLayer(GameData gameData)
		{
			this.gameData = gameData;
		}

		public override int Update(GameTime gameTime)
		{
			//walk through all the projectiles and update them
			foreach (Projectile projectile in gameData.projectiles)
			{
				projectile.update(gameTime);
			}
			return 0;
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (Projectile projectile in gameData.projectiles)
			{
				projectile.draw(gameTime, spriteBatch);
			}
		}

		public override void setSpeed(double pxPerFrame)
		{
		}

		public override double getSpeed()
		{
			return 0;
		}

	}
}
