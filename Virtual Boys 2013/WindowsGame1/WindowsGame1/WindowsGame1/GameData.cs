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
		public enum Bounds
		{
			BOUNDS_NONE		= 0,
			BOUNDS_TOP		= 1 << 0,
			BOUNDS_LEFT		= 1 << 1,
			BOUNDS_BOTTOM	= 1 << 2,
			BOUNDS_RIGHT	= 1 << 3,
			BOUNDS_SLASH	= -128,
			BOUNDS_BSLASH	= -128 | 1 << 0
		}
		public string name;
		public Texture2D texture;
		public int width;
		public int height;
		public int count;
		public Dictionary<int, Bounds> bounds = new Dictionary<int, Bounds>();	// tile index, bounds value
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

	public class GameData
	{
		public List<TileSet>	tileSets	= new List<TileSet>();
		public List<Map>		maps		= new List<Map>();
		public Dictionary<string, int> tileSetNameIdMap = new Dictionary<string, int>();
		public List<AnimationData> animations = new List<AnimationData>();

		public List<Sprite> sprites = new List<Sprite>();

		public List<Layer> layers = new List<Layer>();

		/**
		 * A map of all the loaded textures. asset name -> texture
		 */
		public Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();

		int screenWidth;
		int screenHeight;

		public Random randGenerator = new Random();

		public GameData()
		{
		}

		~GameData()
		{
			tileSets.Clear();
			maps.Clear();
			animations.Clear();

			sprites.Clear();
		}

		public int ScreenHeight
		{
			get { return screenHeight; }
			set { screenHeight = value; }
		}

		public int ScreenWidth
		{
			get { return screenWidth; }
			set { screenWidth = value; }
		}

		private Texture2D loadTexture(ContentManager content, string assetName)
		{
			Texture2D tex = null;

			if (!loadedTextures.TryGetValue(assetName, out tex))
			{
				//didn't find the texture, so load it
				tex = content.Load<Texture2D>(assetName);
				loadedTextures.Add(assetName, tex);
			}

			return tex;
		}

		public void init(ContentManager content)
		{
			XElement gameData = XElement.Load("Content/steve.xml", LoadOptions.None);
			/**
			 * Load Tileset data
			 */
			addTileSets(content, gameData.Element("tilesets"));

			/**
			 * Load Map and tile bounds data
			 */
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
				string[] values = map.Element("mapdata").Value.Split(split, StringSplitOptions.RemoveEmptyEntries);
				string[] bounds = map.Element("bounds").Value.Split(split, StringSplitOptions.RemoveEmptyEntries);
				TileSet tileset = getTileSet(m.tileset);

				int k = 0;
				for (int i = 0; i < m.height; i++) {
					//String dbg = "";
					for (int j = 0; j < m.width; j++) {
						int val = Convert.ToInt32(values[k]);
						int bound = Convert.ToInt16(bounds[k]);
						tileset.bounds[val] = (TileSet.Bounds)bound;
						m.data[i][j] = val;
						k++;
						//dbg += val.ToString() + ", ";
					}
					//Debug.Print(dbg);
				}
				maps.Add(m);
			}

			XElement aniDataElement = XElement.Load("Content/sprites.xml", LoadOptions.None);
			addTileSets(content, aniDataElement.Element("tilesets"));
			addAnimations(content, aniDataElement.Element("animations"));

		} // init()

		private void addTileSets(ContentManager content, XElement tileSetsElement)
		{
			if (tileSetsElement == null)
				return;

			//get the new tilesets from the xml element
			List<TileSet> newTileSets = getTileSets(content, tileSetsElement);

			//add all of the new tiles to the list
			this.tileSets.AddRange(newTileSets);

			//add all of the new tiles to the name->index map
			foreach (TileSet tileSet in newTileSets)
			{
				//add the tileset to the name -> index map with its index within the tileSets list
				int tileSetIndex = tileSets.IndexOf(tileSet);
				tileSetNameIdMap.Add(tileSet.name, tileSetIndex);
			}
		}

		private List<TileSet> getTileSets(ContentManager content, XElement tileSetsElement)
		{
			List<TileSet> tileSets = new List<TileSet>();
			foreach (XElement set in tileSetsElement.Elements("tileset"))
			{
				TileSet s = new TileSet();
				s.name = (string)set.Attribute("name");
				string filename = (string)set.Attribute("fileName");
				s.texture = loadTexture(content, filename);
				s.width = (int)set.Attribute("TileWidth");
				s.height = (int)set.Attribute("TileHeight");
				int hCount = (int)set.Attribute("HorizontalTileCount");
				int vCount = (int)set.Attribute("VerticalTileCount");
				s.count = (int)set.Attribute("TileCount");
				int k = 0;
				for (int i = 0; i < vCount; i++)
				{
					for (int j = 0; j < hCount; j++)
					{
						if (k++ > s.count) break;
						s.coords.Add(new Rectangle(j * s.width, i * s.height, s.width, s.height));
					}
				}
				tileSets.Add(s);

				Debug.Print("Added Tileset: " + s.name);
			}
			return tileSets;
		}

		private void addAnimations(ContentManager content, XElement animationsElement)
		{
			if (animationsElement == null)
				return;

			List<AnimationData> anis = getAnimations(animationsElement);
			animations.AddRange(anis);
		}

		private List<AnimationData> getAnimations(XElement animationsData)
		{
			List<AnimationData> animations = new List<AnimationData>();
			foreach (XElement animationData in animationsData.Elements("animation"))
			{
				int loopCount = (int)animationData.Attribute("loopCount");
				List<Frame> frames = getFrames(animationData);

				AnimationData animation = new AnimationData(loopCount, frames);
				animations.Add(animation);
			}
			return animations;
		}

		private List<Frame> getFrames(XElement animationData)
		{
			List<Frame> frames = new List<Frame>();
			foreach (XElement frameData in animationData.Elements("frame"))
			{
				int frameTime = (int)frameData.Attribute("frameTime");

				//go through all the component xml data in the frame and create a list of of the components
				List<FrameComponent> components = getFrameComponents(frameData);

				Frame frame = new Frame(frameTime, components);
				frames.Add(frame);
			}
			return frames;
		}

		private List<FrameComponent> getFrameComponents(XElement frameData)
		{
			List<FrameComponent> frameComponents = new List<FrameComponent>();
			foreach (XElement componentData in frameData.Elements("component"))
			{
				string tileSetName = (string)componentData.Attribute("tileset");

				int leftOffset = (int)componentData.Attribute("left");
				int topOffset = (int)componentData.Attribute("top");
				int rectIndex = Convert.ToInt32(componentData.Value);

				int tileSetIndex;
				if (!tileSetNameIdMap.TryGetValue(tileSetName, out tileSetIndex))
				{
					System.Console.Error.WriteLine("Couldn't find the tileset index for: " + tileSetName);
				}

				FrameComponent frameComponent = new FrameComponent(tileSetIndex, rectIndex, leftOffset, topOffset);
				frameComponents.Add(frameComponent);
			}
			return frameComponents;
		}

		public TileSet getTileSet(string tileSetName)
		{
			int index = getTileSetIndex(tileSetName);
			return (index >= 0) ? tileSets[index] : null;
		}

		/**
		 * Gets the tileset index from the tilename.
		 * Returns -1 if nothing was found.
		 */
		public int getTileSetIndex(string tileSetName)
		{
			int outIndex = -1;
			if (!tileSetNameIdMap.TryGetValue(tileSetName, out outIndex))
			{
				System.Console.Error.WriteLine("Couldn't find the tileset: " + tileSetName);
				outIndex = -1;
			}

			return outIndex;
		}

		
	}; // GameData
} // namespace
