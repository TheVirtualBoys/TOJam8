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

		private enum State
		{
			STATE_SPLASH,
			STATE_INTRO,
			STATE_GAMEPLAY,
			STATE_SCORES
		}
		private State state = State.STATE_SPLASH;

		KeyboardState oldKeyState;
		GamePadState[] oldPadState;

		//FIXME: this is temp
		MapLayer mapLayer;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 256;
			graphics.PreferredBackBufferHeight = 240;
			graphics.ApplyChanges();
			Content.RootDirectory = "Content";
			audioSys = new AudioSys();
			gameData = new GameData();
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

			gameData.ScreenWidth = graphics.PreferredBackBufferWidth;
			gameData.ScreenHeight = graphics.PreferredBackBufferHeight;

			oldKeyState = Keyboard.GetState();

			PhysicsSprite sprite = new PhysicsSprite(gameData.animations[1]);
			sprite.Ani.start();
			gameData.sprites.Add(sprite);

			TileSet bgTileSet = gameData.getTileSet("bg");
			ImageLayer bgLayer = new ImageLayer(bgTileSet);
			gameData.layers.Add(bgLayer);
			bgLayer.setSpeed(-0.25);

			mapLayer = new MapLayer(gameData, 16, 16);
			gameData.layers.Add(mapLayer);
			mapLayer.setSpeed(2.25);
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
			KeyboardState	newKeyState = Keyboard.GetState();
			GamePadState[] newPadState = { GamePad.GetState(PlayerIndex.One), GamePad.GetState(PlayerIndex.Two) };

			switch (state)
			{
				case State.STATE_GAMEPLAY:
					gameplayInput(oldKeyState, newKeyState, oldPadState, newPadState);
					gameplayUpdate(gameTime);
				break;
				case State.STATE_INTRO:
					introInput(oldKeyState, newKeyState, oldPadState, newPadState);
					introUpdate(gameTime);
				break;
				case State.STATE_SCORES:
					scoresInput(oldKeyState, newKeyState, oldPadState, newPadState);
					scoresUpdate(gameTime);
				break;
				case State.STATE_SPLASH:
					splashInput(oldKeyState, newKeyState, oldPadState, newPadState);
					splashUpdate(gameTime);
				break;
			}
			
			base.Update(gameTime);
			oldKeyState = newKeyState;
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			switch (state)
			{
				case State.STATE_GAMEPLAY:
					gameplayDraw(gameTime);
				break;
				case State.STATE_INTRO:
					introDraw(gameTime);
				break;
				case State.STATE_SCORES:
					scoresDraw(gameTime);
				break;
				case State.STATE_SPLASH:
					splashDraw(gameTime);
				break;
			}
			base.Draw(gameTime);
		}

		public void gameplayUpdate(GameTime gameTime)
		{
			//walk through and update all layers
			foreach (Layer layer in gameData.layers)
			{
				layer.Update(gameTime);
			}

			//walk through all the sprites and update them
			foreach (Sprite sprite in gameData.sprites)
			{
				sprite.update(gameTime);
			}

		}

		public void introUpdate(GameTime gameTime)
		{
			
		}

		public void scoresUpdate(GameTime gameTime)
		{
			
		}

		public void splashUpdate(GameTime gameTime)
		{
			
		}

		public void gameplayInput(KeyboardState oldKeyState, KeyboardState newKeyState, GamePadState[] oldPadState, GamePadState[] newPadState)
		{
			gameData.sprites[0].input(newKeyState);
			if (!oldKeyState.IsKeyDown(Keys.Q) && newKeyState.IsKeyDown(Keys.Q)) state = State.STATE_SCORES;
		}

		public void introInput(KeyboardState oldKeyState, KeyboardState newKeyState, GamePadState[] oldPadState, GamePadState[] newPadState)
		{
			if (oldKeyState.GetPressedKeys().Length == 0 && newKeyState.GetPressedKeys().Length > 0) state = State.STATE_GAMEPLAY;
		}

		public void scoresInput(KeyboardState oldKeyState, KeyboardState newKeyState, GamePadState[] oldPadState, GamePadState[] newPadState)
		{
			// wait for any input, then return to intro state
			if (newKeyState.GetPressedKeys().Length > 0) state = State.STATE_INTRO;
		}

		public void splashInput(KeyboardState oldKeyState, KeyboardState newKeyState, GamePadState[] oldPadState, GamePadState[] newPadState)
		{
			if (newKeyState.GetPressedKeys().Length > 0) state = State.STATE_INTRO;
		}

		public void gameplayDraw(GameTime gameTime)
		{
			spriteBatch.Begin();

			//walk through and draw all layers
			foreach (Layer layer in gameData.layers)
			{
				layer.Draw(gameTime, spriteBatch);
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
		}

		public void introDraw(GameTime gameTime)
		{

		}

		public void scoresDraw(GameTime gameTime)
		{

		}

		public void splashDraw(GameTime gameTime)
		{

		}

		public TileSet.Bounds ray(int x0, int y0, int x1, int y1, out int boundsX, out int boundsY, Map mapData, TileSet tileSet)
		{
			//defaults
			boundsX = -1;
			boundsY = -1;

			TileSet.Bounds bounds = TileSet.Bounds.BOUNDS_NONE;

			//bresenham's line alg
			/*
			   dx := abs(x1-x0)
			   dy := abs(y1-y0) 
			   if x0 < x1 then sx := 1 else sx := -1
			   if y0 < y1 then sy := 1 else sy := -1
			   err := dx-dy
 
			   loop
				 plot(x0,y0)
				 if x0 = x1 and y0 = y1 exit loop
				 e2 := 2*err
				 if e2 > -dy then 
				   err := err - dy
				   x0 := x0 + sx
				 end if
				 if e2 <  dx then 
				   err := err + dx
				   y0 := y0 + sy 
				 end if
			   end loop
			 */

			int dx = Math.Abs(x1 - x0);
			int dy = Math.Abs(y1 - y0);
			int sx = (x0 < x1) ? 1 : -1;
			int sy = (y0 < y1) ? 1 : -1;
			int err = dx - dy;

			while (true)
			{
				//FIXME: mapLayer here is local and probably shouldn't be
				bounds = mapLayer.getMapTileBounds(x0, y0, mapData, tileSet);
				if (bounds != TileSet.Bounds.BOUNDS_NONE)	//found a collision bounds so return
				{
					boundsX = x0;
					boundsY = y0;
					break;
				}

				if (x0 == x1 && y0 == y1)
					break;
				int e2 = 2 * err;
				if (e2 > -dy)
				{
					err -= dy;
					x0 += sx;
				}
				if (e2 < dx)
				{
					err += dx;
					y0 += sy;
				}
			}

			return bounds;
		}



	}
}
