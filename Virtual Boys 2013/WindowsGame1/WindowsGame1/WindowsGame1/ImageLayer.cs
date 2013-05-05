using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class ImageLayer : Layer
	{

		double pxPerFrameSpeed;
		double pixelShiftSizeAccumulator;

		int xOffset;
		int yOffset;

		TileSet tileSet;
		int imageIndex;

		public ImageLayer(TileSet tileSet, int imageIndex)
		{
			this.tileSet = tileSet;
			this.imageIndex = imageIndex;

			pxPerFrameSpeed = 0;
			pixelShiftSizeAccumulator = 0;

			xOffset = 0;
			yOffset = 0;
		}

		public override void Update(GameTime gameTime)
		{
			//add up the speed into an accumulator (takes account for fractional speeds
			pixelShiftSizeAccumulator += pxPerFrameSpeed;
			int pxShiftSize = (int)pixelShiftSizeAccumulator;

			//remove the integer value of the accumulator, which leaves just the fractional part for next time
			pixelShiftSizeAccumulator -= pxShiftSize;

		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			Rectangle srcRect = tileSet.coords[imageIndex];
			Rectangle dest = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
			spriteBatch.Draw(tileSet.texture, dest, srcRect, Color.White);
		}

		public override void setSpeed(double pxPerFrame)
		{
			this.pxPerFrameSpeed = pxPerFrame;
		}

		public override double getSpeed()
		{
			return pxPerFrameSpeed;
		}

	}
}
