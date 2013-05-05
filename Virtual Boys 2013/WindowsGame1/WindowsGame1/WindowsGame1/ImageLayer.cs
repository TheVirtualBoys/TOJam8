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

		int fixedXOffset;
		int fixedYOffset;

		TileSet tileSet;
		int imageIndex;

		GameData gameData;

		/**
		 * Creates a new ImageLayer with the assumption that the tileset only contains 1 image
		 */
		public ImageLayer(GameData gameData, TileSet tileSet)
		{
			this.tileSet = tileSet;
			this.imageIndex = 0;
			this.gameData = gameData;

			pxPerFrameSpeed = 0;
			pixelShiftSizeAccumulator = 0;

			xOffset = 0;
			yOffset = 0;

			fixedXOffset = 0;
			fixedYOffset = 0;
		}

		public ImageLayer(GameData gameData, TileSet tileSet, int imageIndex)
		{
			this.tileSet = tileSet;
			this.imageIndex = imageIndex;
			this.gameData = gameData;

			pxPerFrameSpeed = 0;
			pixelShiftSizeAccumulator = 0;

			xOffset = 0;
			yOffset = 0;

			fixedXOffset = 0;
			fixedYOffset = 0;
		}

		public override void Update(GameTime gameTime)
		{
			//add up the speed into an accumulator (takes account for fractional speeds
			pixelShiftSizeAccumulator += pxPerFrameSpeed;
			int pxShiftSize = (int)pixelShiftSizeAccumulator;

			//remove the integer value of the accumulator, which leaves just the fractional part for next time
			pixelShiftSizeAccumulator -= pxShiftSize;

			xOffset += pxShiftSize;

			xOffset %= tileSet.coords[imageIndex].Width;
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			Rectangle imgRect = tileSet.coords[imageIndex];

			int dstLeft = Math.Max(xOffset, 0);
			int dstRight = Math.Min(xOffset + imgRect.Width, gameData.ScreenWidth);
			int dstTop = Math.Max(yOffset, 0);
			int dstBottom = Math.Min(yOffset + imgRect.Height, gameData.ScreenHeight);
			int dstHeight = dstBottom - dstTop;

			Rectangle dest = new Rectangle(dstLeft + fixedXOffset, dstTop + fixedYOffset, dstRight - dstLeft, dstHeight);

			int srcLeft = Math.Max(0, -xOffset);
			int srcTop = Math.Max(0, -yOffset);
			int srcRight = Math.Min(gameData.ScreenWidth - xOffset, imgRect.Width);
			int srcBottom = Math.Min(gameData.ScreenHeight - yOffset, imgRect.Height);
			int srcHeight = srcBottom - srcTop;

			Rectangle src = new Rectangle(srcLeft, srcTop, srcRight - srcLeft, srcHeight);

			spriteBatch.Draw(tileSet.texture, dest, src, Color.White);

			if (dstRight < gameData.ScreenWidth)
			{
				//need to draw again to wrap image
				int remainingWidth = gameData.ScreenWidth - dstRight;

				dest = new Rectangle(dstRight + fixedXOffset, dstTop + fixedYOffset, remainingWidth, dstHeight);
				src = new Rectangle(0, srcTop, remainingWidth, srcHeight);
				spriteBatch.Draw(tileSet.texture, dest, src, Color.White);
			}
			else if (dstLeft > 0)
			{
				//need to draw again to wrap image
				int remainingWidth = dstLeft;

				dest = new Rectangle(0 + fixedXOffset, dstTop + fixedYOffset, remainingWidth, dstHeight);
				src = new Rectangle(imgRect.Width - remainingWidth, srcTop, remainingWidth, srcHeight);
				spriteBatch.Draw(tileSet.texture, dest, src, Color.White);
			}
		}

		public void reset()
		{
			pixelShiftSizeAccumulator = 0;
			xOffset = 0;
			yOffset = 0;
		}

		public override void setSpeed(double pxPerFrame)
		{
			this.pxPerFrameSpeed = pxPerFrame;
		}

		public override double getSpeed()
		{
			return pxPerFrameSpeed;
		}

		public int FixedXOffset
		{
			get { return fixedXOffset; }
			set { fixedXOffset = value; }
		}

		public int FixedYOffset
		{
			get { return fixedYOffset; }
			set { fixedYOffset = value; }
		}

		public int DynamicXOffset
		{
			get { return xOffset; }
			set { xOffset = value; }
		}

		public int DynamicYOffset
		{
			get { return yOffset; }
			set { yOffset = value; }
		}
	}
}
