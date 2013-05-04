using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SfxrSynth;
using System;

namespace SfxrSynthTest
{
	public class SfxrGame : Microsoft.Xna.Framework.Game
	{
		const int MAX_SOUNDS = 10;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
        Texture2D buttonsTexture;

		SoundEffect[] coinSoundEffects = new SoundEffect[MAX_SOUNDS];
		SoundEffect[] explosionSoundEffects = new SoundEffect[MAX_SOUNDS];
		SoundEffect[] laserSoundEffects = new SoundEffect[MAX_SOUNDS];

        KeyboardState oldKeyboardState;
        GamePadState oldGamePadState;

		Random random = new Random();

		bool playingMutations = false;

		public SfxrGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Synthesizer s = new Synthesizer();
			SfxrParams coinParams = new SfxrParams("0,,0.0736,0.4591,0.3858,0.5416,,,,,,0.5273,0.5732,,,,,,1,,,,,0.5");
            SfxrParams laserParams = new SfxrParams("0,,0.0359,,0.4491,0.2968,,0.2727,,,,,,0.0191,,0.5249,,,1,,,,,0.5");
			SfxrParams explosionParams = new SfxrParams("3,,0.3822,0.4799,0.4721,0.3917,,-0.3271,,,,-0.4969,0.8651,,,,0.5645,-0.1034,1,,,,,0.5");

			coinSoundEffects[0] = s.CreateSound(coinParams);
			laserSoundEffects[0] = s.CreateSound(laserParams);
			explosionSoundEffects[0] = s.CreateSound(explosionParams);

			for (int i = 1; i < MAX_SOUNDS; i++)
			{
				coinSoundEffects[i] = s.CreateMutation(coinParams, 0.1);
				laserSoundEffects[i] = s.CreateMutation(laserParams, 0.1);
				explosionSoundEffects[i] = s.CreateMutation(explosionParams, 0.1);
			}

            buttonsTexture = Content.Load<Texture2D>("buttons");
        }

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

			int r = playingMutations ? random.Next(MAX_SOUNDS) : 0;

            if (keyboardState.IsKeyDown(Keys.Escape) || gamePadState.IsButtonDown(Buttons.Back))
                this.Exit();
			if ((oldKeyboardState.IsKeyUp(Keys.D1) && keyboardState.IsKeyDown(Keys.D1)) ||
				(oldGamePadState.IsButtonUp(Buttons.A) && gamePadState.IsButtonDown(Buttons.A)))
				coinSoundEffects[r].Play();
            if ((oldKeyboardState.IsKeyUp(Keys.D2) && keyboardState.IsKeyDown(Keys.D2)) ||
                (oldGamePadState.IsButtonUp(Buttons.B) && gamePadState.IsButtonDown(Buttons.B)))
                laserSoundEffects[r].Play();
            if ((oldKeyboardState.IsKeyUp(Keys.D3) && keyboardState.IsKeyDown(Keys.D3)) ||
                (oldGamePadState.IsButtonUp(Buttons.X) && gamePadState.IsButtonDown(Buttons.X)))
                explosionSoundEffects[r].Play();
			if ((oldKeyboardState.IsKeyUp(Keys.D4) && keyboardState.IsKeyDown(Keys.D4)) ||
				(oldGamePadState.IsButtonUp(Buttons.Y) && gamePadState.IsButtonDown(Buttons.Y)))
				playingMutations = !playingMutations;

			oldKeyboardState = keyboardState;
            oldGamePadState = gamePadState;

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(buttonsTexture, new Vector2(100, 50), Color.White);
            spriteBatch.End();

			base.Draw(gameTime);
		}

        static void Main(string[] args)
        {
            using (SfxrGame game = new SfxrGame())
            {
                game.Run();
            }
        }
	}
}
