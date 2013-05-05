using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class MapLayer : Layer
	{
		private const int MAPS_FOREGROUND = 0;
		private const int MAPS_BACKGROUND = 1;

		GameData gameData;

		int numScreenTilesWide;
		int numScreenTilesHigh;

		int tileWidth;
		int tileHeight;

		/**
		 * The tile column offset into the current tileset
		 */
		int tileOffset;

		int pixelOffset;

		/**
		 * The first map that is shown on the screen
		 */
		int firstMap;

		/**
		 * The second map that is shown beside the first map
		 */
		int secondMap;


		double pxPerFrameSpeed;
		double pixelShiftSizeAccumulator;

		public MapLayer(GameData gameData, int tileWidth, int tileHeight)
		{
			this.gameData = gameData;
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;

			numScreenTilesWide = gameData.ScreenWidth / tileWidth;
			numScreenTilesHigh = gameData.ScreenHeight / tileHeight;

			firstMap = MAPS_FOREGROUND;
			secondMap = MAPS_FOREGROUND;

			pxPerFrameSpeed = 0;
			pixelShiftSizeAccumulator = 0;
		}

		public override void Update(GameTime gameTime)
		{
			//add up the speed into an accumulator (takes account for fractional speeds
			pixelShiftSizeAccumulator += pxPerFrameSpeed;
			int pxShiftSize = (int)pixelShiftSizeAccumulator;

			//remove the integer value of the accumulator, which leaves just the fractional part for next time
			pixelShiftSizeAccumulator -= pxShiftSize;

			//moves the map by 'pxShiftSize' pixels
			moveRightByPixels(pxShiftSize);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			int tileSetIndex = gameData.getTileSetIndex(gameData.maps[MAPS_FOREGROUND].tileset);
			if (tileSetIndex >= 0)
			{
				int numRows = numScreenTilesHigh;
				int numCols = numScreenTilesWide + 1;	//get the # cols with one extra past screen width so that per pixel shifting doesn't look bad
				for (int row = 0; row < numRows; ++row)
				{
					for (int col = 0; col < numCols; ++col)
					{
						Map mapData = getMapData(row, col);
						TileSet tileSet = gameData.getTileSet(mapData.tileset);

						int tileSetRectIndex = getMapDataRectIndex(mapData, row, col);
						if (tileSetRectIndex == -1)		//shouldn't draw these tiles so continue
							continue;

						Color color = Color.White;
						if (col == numCols / 2)
						{
							int trueIndex = getMapDataTrueIndex(mapData, row, col);
							int boundFlags = (int)tileSet.bounds[trueIndex];
							if (boundFlags == 15) color = Color.Red;
							if (boundFlags == (int)TileSet.Bounds.BOUNDS_BOTTOM) color = Color.Green;
							/*                            if ( (boundFlags & (int)TileSet.Bounds.BOUNDS_TOP) == 1 ) color = Color.Red;
														if ( (boundFlags & (int)TileSet.Bounds.BOUNDS_RIGHT) == 1 ) color = Color.Red;
															case TileSet.Bounds.BOUNDS_LEFT: color = Color.Green; break;
															case TileSet.Bounds.BOUNDS_BOTTOM: color = Color.Blue; break;
															case TileSet.Bounds.BOUNDS_RIGHT: color = Color.Yellow; break;
															case TileSet.Bounds.BOUNDS_SLASH: color = Color.Purple; break;
															case TileSet.Bounds.BOUNDS_BSLASH: color = Color.Orange; break;
														};
							  */
						}
						Rectangle dims = tileSet.coords[tileSetRectIndex];
						//NOTE: row * dims.Height only works if ALL tiles have the same height
						spriteBatch.Draw(tileSet.texture, new Rectangle(col * dims.Width - pixelOffset, row * dims.Height, dims.Width, dims.Height), dims, color);
					}
				}
			}

		}


		public override void setSpeed(double pxPerFrame)
		{
			this.pxPerFrameSpeed = pxPerFrame;
		}

		public override double getSpeed()
		{
			return pxPerFrameSpeed;
		}

		public void moveRightByPixels(int numPixels)
		{
			if (numPixels <= 0)
				return;

			pixelOffset += numPixels;

			if (pixelOffset >= tileWidth)
			{
				int numTiles = pixelOffset / tileWidth;
				pixelOffset = pixelOffset % tileWidth;

				moveRightByTiles(numTiles);
			}
		}

		public void moveRightByTiles(int numTiles)
		{
			if (numTiles <= 0)
				return;

			tileOffset += numTiles;

			if (tileOffset >= gameData.maps[firstMap].width)
			{
				//completely moved past the first map, so shift things
				tileOffset -= gameData.maps[firstMap].width;
				firstMap = secondMap;
				secondMap = getNextMap();
			}
		}

		public Map getMapData(int screenRow, int screenCol)
		{
			int index = getMapDataIndex(screenRow, screenCol);
			return (index >= 0) ? gameData.maps[index] : null;
		}

		public int getMapDataIndex(int screenRow, int screenCol)
		{
			int col = screenCol + tileOffset;

			int mapIndex;
			if (col < gameData.maps[firstMap].width)
				mapIndex = firstMap;
			else
				mapIndex = secondMap;

			return mapIndex;
		}

		public int getMapDataRectIndex(Map mapData, int screenRow, int screenCol)
		{
			int col = screenCol + tileOffset;
			if (col >= gameData.maps[firstMap].width)
				col -= gameData.maps[firstMap].width;

			int index = mapData.data[screenRow][col] - 1;
			if (index < -1)
				index = 0;
			return index;
		}

		public int getMapDataTrueIndex(Map mapData, int screenRow, int screenCol)
		{
			int col = screenCol + tileOffset;
			if (col >= gameData.maps[firstMap].width)
				col -= gameData.maps[firstMap].width;

			int index = mapData.data[screenRow][col];
			if (index < -1)
				index = 0;
			return index;
		}

		public TileSet.Bounds getMapTileBounds(int px, int py, Map mapData, TileSet tileSet)
		{
			TileSet.Bounds bounds = TileSet.Bounds.BOUNDS_NONE;

			//get the tile position for the pixel coords
			int row, col;
			convertScreenPxToTile(px, py, out row, out col);

			//get the bounds for the tile
			int tileTypeIndex = mapData.data[row][col];
			bounds = tileSet.bounds[tileTypeIndex];

			return bounds;
		}

		/**
		 * Gives the screen pixel of the top-left corner of the tile
		 */
		public void convertTileToScreenPx(int row, int col, out int pX, out int pY)
		{
			pX = (col - tileOffset) * tileWidth - pixelOffset;
			pY = row * tileHeight;
		}

		public void convertScreenPxToTile(int pX, int pY, out int row, out int col)
		{
			col = (pX + pixelOffset) / tileWidth + tileOffset;
			row = pY / tileHeight;
		}


		/**
		 * Returns the map index for the next map to be used
		 */
		private int getNextMap()
		{
			//TODO: should probably pick a random mapset next
			return MAPS_FOREGROUND;
		}

	}
}
