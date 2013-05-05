using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class Sprite
	{
		protected Animation ani;
		protected Animation glowAni;
		private int left;
		private int top;
        protected Color m_color;

		public Sprite(Animation ani, Animation glowAni)
		{
			this.ani = ani;
			this.glowAni = glowAni;

			left = 0;
			top = 0;
            m_color = Color.White;
		}

		public Sprite(AnimationData aniData, AnimationData glowAniData)
		{
			this.ani = new Animation(aniData);
			this.glowAni = new Animation(glowAniData);

			left = 0;
			top = 0;
            m_color = Color.White;
		}

		public int Left
		{
			get { return left; }
			set { this.left = value; }
		}

		public int Top
		{
			get { return top; }
			set { this.top = value; }
		}

		public Animation Ani
		{
			get { return ani; }
		}

		public Animation GlowAni
		{
			get { return glowAni; }
		}

		public virtual void update(GameTime frameTime)
		{
			if (glowAni != null)
				glowAni.update(frameTime);
			ani.update(frameTime);
		}

		public virtual void draw(GameTime frameTime, SpriteBatch spriteBatch)
		{
			drawFrame(CurFrame, spriteBatch);
			if (glowAni != null)
			{
				BlendState oldState = spriteBatch.GraphicsDevice.BlendState;
				spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;
				drawFrame(CurGlowFrame, spriteBatch);
				spriteBatch.GraphicsDevice.BlendState = oldState;
			}
		}

		protected void drawFrame(Frame frame, SpriteBatch spriteBatch)
		{
			if (frame == null)
				return;

			int numComponents = frame.NumComponents;
			for (int i = 0; i < numComponents; ++i)
			{
				FrameComponent component = frame.getComponent(i);
				TileSet tileSet = Game1.Instance.gameData.tileSets[component.TileSetIndex];
				Rectangle tileDims = component.getTileRect(tileSet);

				Rectangle pos = new Rectangle(Left + component.Left, Top + component.Top, tileDims.Width, tileDims.Height);

				spriteBatch.Draw(tileSet.texture, pos, tileDims, getFilterColour());
			}
		}

        public virtual void input(KeyboardState keys){} //HACKJEFFGIFFEN ugh passthrough to PhysicsSprite

		public Frame CurFrame
		{
			get { return Ani.CurFrame; }
		}

		public Frame CurGlowFrame
		{
			get { return (GlowAni != null)? GlowAni.CurFrame : null; }
		}

		public virtual Color getFilterColour()
		{
            return m_color;
		}
	}
}
