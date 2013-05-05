using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class TimedImageLayer : Layer
	{
		/**
		 * the number of milliseconds left before we show the image again
		 */
		int msLeftBeforeShow;

		/**
		 * the minimum time to wait before showing the image again
		 */
		int minMSTimeBeforeShow;

		/**
		 * the maximum time to wait before showing the image again
		 */
		int maxMSTimeBeforeShow;

		Random randGenerator;

		int maxLoopCount;
		int curLoopCount;

		bool showImage;


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
		public TimedImageLayer(GameData gameData, TileSet tileSet, int minMSTimeBeforeShow, int maxMSTimeBeforeShow)
		{
			this.minMSTimeBeforeShow = minMSTimeBeforeShow;
			this.maxMSTimeBeforeShow = maxMSTimeBeforeShow;

			this.tileSet = tileSet;
			this.imageIndex = 0;
			this.gameData = gameData;

			pxPerFrameSpeed = 0;
			pixelShiftSizeAccumulator = 0;

			xOffset = 0;
			yOffset = 0;

			fixedXOffset = 0;
			fixedYOffset = 0;

			maxLoopCount = 0;
			curLoopCount = 0;
			showImage = false;

			randGenerator = new Random();

			msLeftBeforeShow = getNextTime();
		}

		public TimedImageLayer(GameData gameData, TileSet tileSet, int imageIndex, int minMSTimeBeforeShow, int maxMSTimeBeforeShow)
		{
			this.minMSTimeBeforeShow = minMSTimeBeforeShow;
			this.maxMSTimeBeforeShow = maxMSTimeBeforeShow;

			this.tileSet = tileSet;
			this.imageIndex = imageIndex;
			this.gameData = gameData;

			pxPerFrameSpeed = 0;
			pixelShiftSizeAccumulator = 0;

			xOffset = 0;
			yOffset = 0;

			fixedXOffset = 0;
			fixedYOffset = 0;

			maxLoopCount = 0;
			curLoopCount = 0;
			showImage = false;

			randGenerator = new Random();

			msLeftBeforeShow = getNextTime();
		}

		public void start()
		{
			//show the image
			showImage = true;
			msLeftBeforeShow = 0;
			curLoopCount = maxLoopCount;
			reset();
		}

		public override void Update(GameTime gameTime)
		{
			if (msLeftBeforeShow > 0)
			{
				msLeftBeforeShow -= gameTime.ElapsedGameTime.Milliseconds;
				if (msLeftBeforeShow <= 0)
				{
					start();
				}
			}
			else
			{
				//add up the speed into an accumulator (takes account for fractional speeds
				pixelShiftSizeAccumulator += pxPerFrameSpeed;
				int pxShiftSize = (int)pixelShiftSizeAccumulator;

				//remove the integer value of the accumulator, which leaves just the fractional part for next time
				pixelShiftSizeAccumulator -= pxShiftSize;

				xOffset += pxShiftSize;

				xOffset %= tileSet.coords[imageIndex].Width + gameData.ScreenWidth;


				//check if we just looped
				if (DynamicXOffset == 0)
				{
					if (--curLoopCount < 0)
					{
						showImage = false;

						msLeftBeforeShow = getNextTime();
					}
				}
			}

		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (!showImage)
				return;

			Rectangle imgRect = tileSet.coords[imageIndex];

			int dstLeft = fixedXOffset + xOffset;
			int dstTop = fixedYOffset + yOffset;

			Rectangle dest = new Rectangle(dstLeft, dstTop, imgRect.Width, imgRect.Height);

			spriteBatch.Draw(tileSet.texture, dest, imgRect, Color.White);
		}

		private int getNextTime()
		{
			return randGenerator.Next(minMSTimeBeforeShow, maxMSTimeBeforeShow);
		}

		public int LoopCount
		{
			get { return maxLoopCount; }
			set { maxLoopCount = value; }
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
