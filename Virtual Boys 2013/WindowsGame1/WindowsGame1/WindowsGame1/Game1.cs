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

		private const int tileWidth = 16;
		private const int tileHeight = 16;

		private int pixelShiftSize;

		/**
		 * The tile column offset into the current tileset
		 */
		int startingTileOffset;

		int startingPixelOffset;

		/**
		 * The first map that is shown on the screen
		 */
		int firstMap;

		/**
		 * The second map that is shown beside the first map
		 */
		int secondMap;

		KeyboardState oldKeyState;


		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 256;
			graphics.PreferredBackBufferHeight = 240;
			graphics.ApplyChanges();
			Content.RootDirectory = "Content";
			audioSys = new AudioSys();
			gameData = new GameData();

			numScreenTilesWide = graphics.PreferredBackBufferWidth / tileWidth;
			numScreenTilesHigh = graphics.PreferredBackBufferHeight / tileHeight;
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
			audioSys.loadNSF("Content/cv3.nsf");
			audioSys.play();
			gameData.init(Content);
			base.Initialize();

			oldKeyState = Keyboard.GetState();

			startingTileOffset = 0;
			startingPixelOffset = 0;

			firstMap = MAPS_FOREGROUND;
			secondMap = MAPS_FOREGROUND;

			pixelShiftSize = 2;

            PhysicsSprite sprite = new PhysicsSprite(gameData.animations[1]);
            sprite.Ani.start();
            gameData.sprites.Add(sprite);
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
			//audioSys.loadNSF("Content/cv3.nsf");
			//audioSys.play();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

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

			//walk through all the sprites and update them
			foreach (Sprite sprite in gameData.sprites)
			{
				sprite.update(gameTime);
			}

			base.Update(gameTime);

			UpdateInput();

			//moves the map by 'pixelShiftSize' pixels. to speed up or slow down, call incPixelShiftSize
			moveRightByPixels(pixelShiftSize);
		}

		private void UpdateInput()
		{
			KeyboardState newKeyState = Keyboard.GetState();

/*			if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
				incPixelShiftSize(-1);
			else if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
				incPixelShiftSize(1);
			else if (newKeyState.IsKeyDown(Keys.Left) && !oldKeyState.IsKeyDown(Keys.Left))
			{
				//left key was pressed
				incPixelShiftSize(-1);
			}
			else if (newKeyState.IsKeyDown(Keys.Right) && !oldKeyState.IsKeyDown(Keys.Right))
			{
				//right key was pressed
				incPixelShiftSize(1);
			}
			else if (newKeyState.IsKeyDown(Keys.Up) && !oldKeyState.IsKeyDown(Keys.Up))
			{
				audioSys.playSFX(AudioSys.Effect.SFX_LAND);
			}
*/

            gameData.sprites[0].input( newKeyState );

			oldKeyState = newKeyState;
		}

		/**
		 * Changes the pixel shift size by 'inc' (either + or -).
		 * This makes the map scroll by faster or slower
		 */
		private void incPixelShiftSize(int inc)
		{
			pixelShiftSize += inc;
			if (pixelShiftSize < 0)
				pixelShiftSize = 0;
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
  */                      }
						Rectangle dims = tileSet.coords[tileSetRectIndex];
						//NOTE: row * dims.Height only works if ALL tiles have the same height
						spriteBatch.Draw(tileSet.texture, new Rectangle(col * dims.Width - startingPixelOffset, row * dims.Height, dims.Width, dims.Height), dims, color);
					}
				}
			}

			//walk through all the sprites and draw them
			foreach (Sprite sprite in gameData.sprites)
			{
				Frame frame = sprite.CurFrame;
				if (frame == null)
					continue;

				int numComponents = frame.NumComponents;
				for (int i = 0; i < numComponents; ++i)
				{
					FrameComponent component = frame.getComponent(i);
					TileSet tileSet = gameData.tileSets[component.TileSetIndex];
					Rectangle tileDims = component.getTileRect(tileSet);

					Rectangle pos = new Rectangle(sprite.Left + component.Left, sprite.Top + component.Top, tileDims.Width, tileDims.Height);

					spriteBatch.Draw(tileSet.texture, pos, tileDims, Color.White);
				}
				
			}

			spriteBatch.End();
			base.Draw(gameTime);
		}

		public void moveRightByPixels(int numPixels)
		{
			startingPixelOffset += numPixels;

			if (startingPixelOffset >= tileWidth)
			{
				int numTiles = startingPixelOffset / tileWidth;
				startingPixelOffset = startingPixelOffset % tileWidth;

				moveRightByTiles(numTiles);
			}
		}

		public Map getMapData(int screenRow, int screenCol)
		{
			int index = getMapDataIndex(screenRow, screenCol);
			return (index >= 0) ? gameData.maps[index] : null;
		}

		public int getMapDataIndex(int screenRow, int screenCol)
		{
			int col = screenCol + startingTileOffset;

			int mapIndex;
			if (col < gameData.maps[firstMap].width)
				mapIndex = firstMap;
			else
				mapIndex = secondMap;

			return mapIndex;
		}

		public int getMapDataRectIndex(Map mapData, int screenRow, int screenCol)
		{
			int col = screenCol + startingTileOffset;
			if (col >= gameData.maps[firstMap].width)
				col -= gameData.maps[firstMap].width;

			int index = mapData.data[screenRow][col] - 1;
			if (index < -1)
				index = 0;
			return index;
		}

        public int getMapDataTrueIndex(Map mapData, int screenRow, int screenCol)
        {
            int col = screenCol + startingTileOffset;
            if (col >= gameData.maps[firstMap].width)
                col -= gameData.maps[firstMap].width;

            int index = mapData.data[screenRow][col];
            if (index < -1)
                index = 0;
            return index;
        }

		public void moveRightByTiles(int numTiles)
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
