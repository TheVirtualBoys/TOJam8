using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
	public class Sprite
	{
		protected Animation ani;
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

        public virtual void input(KeyboardState keys){} //HACKJEFFGIFFEN ugh passthrough to PhysicsSprite

		public Frame CurFrame
		{
			get { return Ani.CurFrame; }
		}

	}
}
