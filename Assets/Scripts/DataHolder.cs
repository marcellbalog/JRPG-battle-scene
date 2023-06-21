using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;

namespace ChristmasBattle
{
	public class DataHolder : MonoBehaviour
	{
		public static DataHolder S;

		int[] AllyUsedImages = new int[10];

		public List<Sprite> RudolfImages = new List<Sprite>();
		public List<Sprite> ElfImages = new List<Sprite>();
		public List<Sprite> SnowmanImages = new List<Sprite>();
		public List<Sprite> SantaImages = new List<Sprite>();
		public List<Sprite> GingerbreadImages = new List<Sprite>();

		public Sprite TestEnemy;

		public SoundAudioClip[] SoundAudioClips;
		[System.Serializable]
		public class SoundAudioClip
		{
			public MediaManager.Sound sound;
			public AudioClip audioClip;
		}

		public Effect[] Effects;
		[System.Serializable]
		public class Effect
		{
			public MediaManager.Effect effect;
			public GameObject EffectObject;
		}

		public EnemyAsset[] Enemies;
		[System.Serializable]
		public class EnemyAsset
		{
			public EnemyName name;
			public Sprite EnemyImage;
		}
		public enum EnemyName
		{
			Grinch,
			Toy,
			Monster
		}

		[System.Serializable]
		public class EnemyList
		{
			public Enemy[] Enemies;
		}
		public EnemyList enemyCollection = new EnemyList();
		public TextAsset enemyData;
		public List<Enemy> baseEnemies = new List<Enemy>();

		public List<Item> Items;

		private void Awake()
		{
			S = this;

			enemyCollection = JsonUtility.FromJson<EnemyList>(enemyData.text);
			baseEnemies = enemyCollection.Enemies.ToList();
			print(baseEnemies.Count);
		}

		public Sprite GetAllyImage(GameObject o)
		{
			int index = 0;
			if (TeamManager.S.EntityObjects.Contains(o))
			{
				index = TeamManager.S.EntityObjects.IndexOf(o);
				switch (index)
				{
					case 0:
						return RudolfImages[AllyUsedImages[index]];
					case 1:
						return SnowmanImages[AllyUsedImages[index]];
					case 2:
						return SantaImages[AllyUsedImages[index]];
					case 3:
						return ElfImages[AllyUsedImages[index]];
					case 4:
						return GingerbreadImages[AllyUsedImages[index]];
				}
			}
			else
				return TestEnemy;

			return null;
		}

		public Sprite GetEnemyImage(EnemyName name)
		{
			foreach (var e in Enemies)
			{
				if (e.name == name)
					return e.EnemyImage;
			}

			Debug.LogError("Enemy " + name + " not found!");
			return null;
		}

		public Enemy CreateNewEnemy(EnemyName name)
		{
			foreach (var e in enemyCollection.Enemies)
			{
				if (e.NameText == name.ToString())
				{
					Enemy ne = new Enemy(e);
					return ne;
				}
			}

			Debug.LogError("Enemy " + name + " creation error!");
			return null;
		}

		public Item CreateNewItem(string name)
		{
			/*return Items.FirstOrDefault(item =>
			{
				var attribute = Items.Find(attribute => attribute.name == name);

				if (attribute != null)
				{
					print("Item created " + attribute.name);
					return attribute;
				}

				return false;
			});		
			*/

			return Items.FirstOrDefault(item => item.name == name);

		}
	}
}