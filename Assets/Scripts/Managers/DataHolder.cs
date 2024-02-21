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
		public static DataHolder instance;

		private int[] _allyUsedImages = new int[10];

		[SerializeField] private List<Sprite> RudolfImages = new List<Sprite>();
		[SerializeField] private List<Sprite> ElfImages = new List<Sprite>();
		[SerializeField] private List<Sprite> SnowmanImages = new List<Sprite>();
		[SerializeField] private List<Sprite> SantaImages = new List<Sprite>();
		[SerializeField] private List<Sprite> GingerbreadImages = new List<Sprite>();

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
			instance = this;

			enemyCollection = JsonUtility.FromJson<EnemyList>(enemyData.text);
			baseEnemies = enemyCollection.Enemies.ToList();
			print(baseEnemies.Count);
		}

		public Sprite GetAllyImage(GameObject o)
		{
			int index = 0;
			if (TeamManager.instance.EntityObjects.Contains(o))
			{
				index = TeamManager.instance.EntityObjects.IndexOf(o);
				switch (index)
				{
					case 0:
						return RudolfImages[_allyUsedImages[index]];
					case 1:
						return SnowmanImages[_allyUsedImages[index]];
					case 2:
						return SantaImages[_allyUsedImages[index]];
					case 3:
						return ElfImages[_allyUsedImages[index]];
					case 4:
						return GingerbreadImages[_allyUsedImages[index]];
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
				if (e.nameText == name.ToString())
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
			return Items.FirstOrDefault(item => item.name == name);
		}

	}
}