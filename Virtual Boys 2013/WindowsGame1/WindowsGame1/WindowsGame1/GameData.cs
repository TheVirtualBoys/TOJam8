using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
	public class TileSet
	{
		public string name;
		public Texture2D texture;
		public int width;
		public int height;
		public int count;
		public List<Rectangle> coords = new List<Rectangle>();
	};

	public class Map
	{
		public string name;
		public string tileset;
		public int width;
		public int height;
		public int[][] data;
	};

	public class GameData// : Microsoft.Xna.Framework.Game
	{
		public List<TileSet>	tileSets	= new List<TileSet>();
		public List<Map>		maps		= new List<Map>();

		public GameData()
		{
		}

		~GameData()
		{
			tileSets.Clear();
			maps.Clear();
		}

		public void init(ContentManager content)
		{
			XElement gameData = XElement.Load("steve.xml", LoadOptions.None);
			foreach (XElement set in gameData.Element("tilesets").Elements("tileset")) {
				TileSet s	= new TileSet();
				s.name		= (string)set.Attribute("name");
				//s.texture	= content.Load<Texture2D>((string)set.Attribute("fileName"));
				s.width		= (int)set.Attribute("TileWidth");
				s.height	= (int)set.Attribute("TileHeight");
				int hCount	= (int)set.Attribute("HorizontalTileCount");
				int vCount	= (int)set.Attribute("VerticalTileCount");
				s.count		= (int)set.Attribute("TileCount");
				int k = 0;
				for (int i = 0; i < hCount; i++) {
					for (int j = 0; j < vCount; j++) {
						if (k++ > s.count) break;
						s.coords.Add(new Rectangle(i * s.width, j * s.height, s.width, s.height));
					}
				}
				tileSets.Add(s);
				Debug.Print("Added Tileset: " + s.name);
			}
			foreach (XElement map in gameData.Element("maps").Elements("map")) {
				Map m			= new Map();
				m.name			= (string)map.Attribute("name");
				m.tileset		= (string)map.Attribute("tileset");
				m.width			= (int)map.Attribute("width");
				m.height		= (int)map.Attribute("height");
				m.data			= new int[m.height][];
				for (int i = 0; i < m.data.Length; i++)
				{
					m.data[i] = new int[m.width];
				}
				char[] split	= new char[] {',', '\r', '\n'};
				string[] values	= map.Value.Split(split, StringSplitOptions.RemoveEmptyEntries);
				int k = 0;
				for (int i = 0; i < m.height; i++) {
					//String dbg = "";
					for (int j = 0; j < m.width; j++) {
						int val = Convert.ToInt32(values[k]);
						m.data[i][j] = val;
						//dbg += val.ToString() + ", ";
						k++;
					}
					//Debug.Print(dbg);
				}
				maps.Add(m);
			}
		} // init()
	}; // GameData
} // namespace
