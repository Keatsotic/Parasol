using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;


namespace Parasol
{
	public class SceneManager
	{
		//wallMap vars
		public WallMap wallMap = new WallMap();
		public TiledMap tiledMap;
		public TiledMapRenderer tiledRenderer;
		public TiledMapObjectLayer ObjectLayer { get; private set; } = null;

		//lists of objects / walls / doors to clean up after
		public List<GameObject> killObjects = new List<GameObject>();
		public List<Wall> killWalls = new List<Wall>();
		//public List<Door> killDoor = new List<Door>();
		public int wilKillPlayer;

		//music 
		public static SoundEffect bgMusic;
		public static SoundEffectInstance soundEffectInstance;

		//temp vars
		public string levelNameHolder;

		public SceneManager()
		{ }

		
		public void LoadObjects(ContentManager content, List<GameObject> objects)
		{
			//PauseMenu.Load(content);
			for (int i = 1; i < objects.Count; i++)
			{
				objects[i].Initialize();
				objects[i].Load(content);
			}
		}

	}
}
