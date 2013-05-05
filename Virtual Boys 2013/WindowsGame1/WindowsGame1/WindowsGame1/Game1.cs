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
		static Game1 sm_game;
		public static Game1 Instance
		{
			get { return sm_game; }
		}   

		private enum State
		{
			STATE_SPLASH,
			STATE_INTRO,
			STATE_GAMEPLAY,
			STATE_SCORES
		}
		private State state;
		private void setState(State newState)
		{
			state = newState;
			foreach (RenderTarget2D tex in renderTarget) {
				graphics.GraphicsDevice.SetRenderTarget(tex);
				GraphicsDevice.Clear(Color.CornflowerBlue);
			}
			graphics.GraphicsDevice.SetRenderTarget(null);
		}
		

		KeyboardState oldKeyState;
		GamePadState[] oldPadState;
		RenderTarget2D[] renderTarget;
		Song music;

		//FIXME: this is temp
		MapLayer mapLayer;

		public Game1()
		{
			sm_game = this;
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 960;
			//graphics.PreferredBackBufferWidth = 1024;
			//graphics.PreferredBackBufferHeight = 960;
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
			gameData.init(Content);
			base.Initialize();

			gameData.ScreenWidth = 256;
			gameData.ScreenHeight = 240;

			oldKeyState = Keyboard.GetState();

			PhysicsSprite sprite = new PhysicsSprite(gameData.animations[1]);
			sprite.Ani.start();
			gameData.sprites.Add(sprite);

			TileSet bgTileSet = gameData.getTileSet("bg");
			ImageLayer bgLayer = new ImageLayer(gameData, bgTileSet);
			gameData.layers.Add(bgLayer);
			bgLayer.setSpeed(-0.25);

			TileSet subwayTileSet = gameData.getTileSet("subway");
			ImageLayer subwayLayer = new ImageLayer(gameData, subwayTileSet);
			gameData.layers.Add(subwayLayer);
			subwayLayer.setSpeed(-4);
			subwayLayer.YOffset = 120;

			TileSet fenceTileSet = gameData.getTileSet("fence");
			ImageLayer fenceLayer = new ImageLayer(gameData, fenceTileSet);
			gameData.layers.Add(fenceLayer);
			fenceLayer.setSpeed(-0.75);
			fenceLayer.YOffset = gameData.ScreenHeight - fenceTileSet.height;

			mapLayer = new MapLayer(gameData, 16, 16);
			gameData.layers.Add(mapLayer);
			mapLayer.setSpeed(2.25);

			// Create RenderTargets after gameData.layers is populated
			renderTarget = new RenderTarget2D[gameData.layers.Count + 1];
			for (int i = 0; i <= gameData.layers.Count; i++) {
				renderTarget[i] = new RenderTarget2D(graphics.GraphicsDevice, 256, 240, false, SurfaceFormat.Color, DepthFormat.Depth16);
			}
			setState(State.STATE_SPLASH);
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
			music = Content.Load<Song>("OFalconer-80sSciFi");
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
			Content.Unload();
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
					if (MediaPlayer.State == MediaState.Stopped) MediaPlayer.Play(music);
					gameplayInput(oldKeyState, newKeyState, oldPadState, newPadState);
					gameplayUpdate(gameTime);
				break;
				case State.STATE_INTRO:
					if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();
					introInput(oldKeyState, newKeyState, oldPadState, newPadState);
					introUpdate(gameTime);
				break;
				case State.STATE_SCORES:
					scoresInput(oldKeyState, newKeyState, oldPadState, newPadState);
					scoresUpdate(gameTime);
				break;
				case State.STATE_SPLASH:
					if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();
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
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			foreach (Texture2D tex in renderTarget) {
				spriteBatch.Draw(tex, new Rectangle(0, 0, 1024, 960), Color.White);
			}
			spriteBatch.End();
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
			if (!oldKeyState.IsKeyDown(Keys.Q) && newKeyState.IsKeyDown(Keys.Q)) setState(State.STATE_SCORES);
		}

		public void introInput(KeyboardState oldKeyState, KeyboardState newKeyState, GamePadState[] oldPadState, GamePadState[] newPadState)
		{
			if (oldKeyState.GetPressedKeys().Length == 0 && newKeyState.GetPressedKeys().Length > 0) setState(State.STATE_GAMEPLAY);
		}

		public void scoresInput(KeyboardState oldKeyState, KeyboardState newKeyState, GamePadState[] oldPadState, GamePadState[] newPadState)
		{
			// wait for any input, then return to intro state
			if (newKeyState.GetPressedKeys().Length > 0) setState(State.STATE_SPLASH);
		}

		public void splashInput(KeyboardState oldKeyState, KeyboardState newKeyState, GamePadState[] oldPadState, GamePadState[] newPadState)
		{
			if (newKeyState.GetPressedKeys().Length > 0) setState(State.STATE_INTRO);
		}

		public void gameplayDraw(GameTime gameTime)
		{
			//walk through and draw all layers
			int target = 0;
			foreach (Layer layer in gameData.layers)
			{
				graphics.GraphicsDevice.SetRenderTarget(renderTarget[target++]);
				GraphicsDevice.Clear(Color.Transparent);
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
				layer.Draw(gameTime, spriteBatch);
				spriteBatch.End();
			}

			//walk through all the sprites and draw them
			graphics.GraphicsDevice.SetRenderTarget(renderTarget[target++]);
			GraphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
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
			graphics.GraphicsDevice.SetRenderTarget(null);
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
		public TileSet.Bounds ray(int x0, int y0, int x1, int y1, out int boundsX, out int boundsY)
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
				int row, col;
				mapLayer.convertScreenPxToTile(x0, y0, out row, out col);
				Map mapData = mapLayer.getAbsoluteMapData(row, col);
				TileSet tileSet = gameData.getTileSet(mapData.tileset);
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
