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
		private Animation ani;
		private int left;
		private int top;

		public Sprite(Animation ani)
		{
			this.ani = ani;

			left = 0;
			top = 0;
		}

		public Sprite(AnimationData aniData)
		{
			this.ani = new Animation(aniData);

			left = 0;
			top = 0;
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

		public virtual void update(GameTime frameTime)
		{
			ani.update(frameTime);
		}

		public virtual void draw(GameTime frameTime, SpriteBatch spriteBatch)
		{
			Frame frame = CurFrame;
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

		public virtual Color getFilterColour()
		{
			return Color.White;
		}
	}
}
