using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Linq;
using System.Diagnostics;

namespace WindowsGame1
{

	public class Util
	{

	};

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		AudioSys				audioSys;
		GameData				gameData;
		GraphicsDeviceManager	graphics;
		SpriteBatch				spriteBatch;

		private const int MAPS_FOREGROUND = 0;
		private const int MAPS_BACKGROUND = 1;

		private int numScreenTilesWide;
		private int numScreenTilesHigh;

		Dictionary<String, int> tileSetNameIdMap = new Dictionary<string, int>();

		/**
		 * The tile column offset into the current tileset
		 */
		int startingTileOffset;

		/**
		 * The first map that is shown on the screen
		 */
		int firstMap;

		/**
		 * The second map that is shown beside the first map
		 */
		int secondMap;


		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 256;
			graphics.PreferredBackBufferHeight = 240;
			graphics.ApplyChanges();
			Content.RootDirectory = "Content";
			audioSys = new AudioSys();
			gameData = new GameData();

			numScreenTilesWide = graphics.PreferredBackBufferWidth / 16;
			numScreenTilesHigh = graphics.PreferredBackBufferHeight / 16;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			audioSys.init();
//			audioSys.loadNSF("Content/cv3.nsf");
//			audioSys.play();
			gameData.init(Content);
			base.Initialize();

			//TODO: need to properly initialize the tileSetNameIdMap map
			tileSetNameIdMap.Add("Tile", 0);

			startingTileOffset = 0;

			firstMap = MAPS_FOREGROUND;
			secondMap = MAPS_FOREGROUND;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		int frameCount;
		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			// TODO: Add your update logic here

			base.Update(gameTime);

			++frameCount;
			if (frameCount % 10 == 0)
			{
				moveRight(1);
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			spriteBatch.Begin();

			int tileSetIndex = getTileSetIndex(gameData.maps[MAPS_FOREGROUND].tileset);
			if (tileSetIndex >= 0)
			{
				int numRows = numScreenTilesHigh;
				int numCols = numScreenTilesWide;
				for (int row = 0; row < numRows; ++row)
				{
					for (int col = 0; col < numCols; ++col)
					{
						Map mapData = getMapData(row, col);
						TileSet tileSet = getTileSet(mapData.tileset);

						int tileSetRectIndex = getMapDataRectIndex(mapData, row, col);
						if (tileSetRectIndex == -1)		//shouldn't draw these tiles so continue
							continue;

						Rectangle dims = tileSet.coords[tileSetRectIndex];
						//NOTE: row * dims.Height only works if ALL tiles have the same height
						spriteBatch.Draw(tileSet.texture, new Rectangle(col * dims.Width, row * dims.Height, dims.Width, dims.Height), dims, Color.White);
					}
				}
			}
			spriteBatch.End();
			base.Draw(gameTime);
		}

		private TileSet getTileSet(string tileSetName)
		{
			int index = getTileSetIndex(tileSetName);
			return (index >= 0) ? gameData.tileSets[index] : null;
		}

		/**
		 * Gets the tileset index from the tilename.
		 * Returns -1 if nothing was found.
		 */
		private int getTileSetIndex(string tileSetName)
		{
			int outIndex = -1;
			if (!tileSetNameIdMap.TryGetValue(tileSetName, out outIndex))
			{
				System.Console.Error.WriteLine("Couldn't find the tileset: " + tileSetName);
			}

			return outIndex;
		}

		private Map getMapData(int screenRow, int screenCol)
		{
			int index = getMapDataIndex(screenRow, screenCol);
			return (index >= 0) ? gameData.maps[index] : null;
		}

		private int getMapDataIndex(int screenRow, int screenCol)
		{
			int col = screenCol + startingTileOffset;

			int mapIndex;
			if (col < gameData.maps[firstMap].width)
				mapIndex = firstMap;
			else
				mapIndex = secondMap;

			return mapIndex;
		}

		private int getMapDataRectIndex(Map mapData, int screenRow, int screenCol)
		{
			int col = screenCol + startingTileOffset;
			if (col >= gameData.maps[firstMap].width)
				col -= gameData.maps[firstMap].width;

			int index = mapData.data[screenRow][col] - 1;
			if (index < -1)
				index = 0;
			return index;
		}

		public void moveRight(int numTiles)
		{
			startingTileOffset += numTiles;

			if (startingTileOffset >= gameData.maps[firstMap].width)
			{
				//completely moved past the first map, so shift things
				startingTileOffset -= gameData.maps[firstMap].width;
				firstMap = secondMap;
				secondMap = getNextMap();
			}
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
