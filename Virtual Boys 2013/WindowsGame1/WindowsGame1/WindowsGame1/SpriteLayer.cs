using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class SpriteLayer : Layer
	{
		GameData gameData;

		public SpriteLayer(GameData gameData)
		{
			this.gameData = gameData;
		}

		public override int Update(GameTime gameTime)
		{
			//walk through all the sprites and update them
			foreach (Sprite sprite in gameData.sprites)
			{
				sprite.update(gameTime);
			}
			return 0;
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (Sprite sprite in gameData.sprites)
			{
				sprite.draw(gameTime, spriteBatch);
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
