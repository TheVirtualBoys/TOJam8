using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public abstract class Layer
	{

		public abstract void Update(GameTime gameTime);

		public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

		public abstract void setSpeed(double pxPerFrame);

		public abstract double getSpeed();
	}
}
