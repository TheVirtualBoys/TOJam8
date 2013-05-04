using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
	class FrameComponent
	{
		int tileSetIndex;

		/**
		 * The left offset for this component
		 */
		int posLeft;

		/**
		 * The top offset for this component
		 */
		int posTop;

		/**
		 * The index of the rectangle within the tileset
		 */
		int rectIndex;

		public FrameComponent(int tileSetIndex, int rectIndex, int left, int top)
		{
			this.tileSetIndex = tileSetIndex;
			this.rectIndex = rectIndex;
			this.posLeft = left;
			this.posTop = top;
		}

		public int Left
		{
			get { return posLeft; }
		}

		public int Top
		{
			get { return posTop; }
		}

		public int TileSetIndex
		{
			get { return tileSetIndex; }
		}

		public int RectIndex
		{
			get { return rectIndex; }
		}

		public Rectangle getTileRect(TileSet tileSet)
		{
			return tileSet.coords[RectIndex];
		}
	}
}
