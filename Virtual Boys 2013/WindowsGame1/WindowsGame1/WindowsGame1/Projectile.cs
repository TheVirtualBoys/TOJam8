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

		double velX;
		double velY;

		public Projectile(double velX, double velY)
		{
			this.velX = velX;
			this.velY = velY;
		}

		public void update(GameTime gameTime)
		{
		}

		public void draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
		}

	}
}
