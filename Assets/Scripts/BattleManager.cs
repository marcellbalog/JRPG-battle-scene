using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Video;

namespace ChristmasBattle
{
	public sealed class BattleManager : MonoBehaviour
	{
		private BattleManager() { }
		public static BattleManager S;

		[Header("Battle Data")]

		public Ally[] activeChars = new Ally[3];
		public Transform[] allyPositions = new Transform[3];
		public Transform[] enemyPositions = new Transform[3];
		public GameObject Enemy;

		List<Enemy> TestEnemies = new List<Enemy>();
		public List<Enemy> Enemies = new List<Enemy>();		

		public BattleEntity currentEntity;
		public Ally currentChar;
		public GameObject CurrentCharImage;
		public GameObject ActionButtons;
		public GameObject AbilityPanel;
		public GameObject AbilityRow;
		public GameObject ItemPanel;
		public GameObject ItemRow;
		public GameObject StatPanel;

		public GameObject TextHolder;
		public GameObject DamageText;
		public GameObject EndPanel;
		public GameObject StartMenuPanel;

		[SerializeReference]
		public List<BattleEntity> BattleEntities = new List<BattleEntity>();
		public List<BattleEntity> EntityQueue = new List<BattleEntity>();

		public GameObject QueuePanel;
		public GameObject QueueElement;
		public GameObject SwitchPanel;
		public GameObject SwitchElement;

		public bool isAttacking;
		public Enemy selectedEnemy;

		public AudioClip battleMusic;
		public AudioSource music;

		public enum TurnType
        {
			Ally_turn,
			Enemy_turn,
			Talking_turn
        }
		public TurnType turnType;

		private void Awake()
		{
			S = this;
		}

		private void Start()
		{
			StartMenuPanel.SetActive(true);
			TestEnemies.Add(DataHolder.S.CreateNewEnemy(DataHolder.EnemyName.Toy));
			TestEnemies.Add(DataHolder.S.CreateNewEnemy(DataHolder.EnemyName.Grinch));
			TestEnemies.Add(DataHolder.S.CreateNewEnemy(DataHolder.EnemyName.Monster));
		}

		public void StartBattle()
		{
			SetUpAllyCharacters();
			CreateEnemies(TestEnemies);
			SetUpQueue();
			StartCoroutine(RunQueue(false));
			music.clip = battleMusic;
			music.Play();
		}

		void CreateEnemies(List<Enemy> enemies)
		{			
			Enemies = enemies;			
			for (int i = 0; i < enemies.Count; i++)
			{
				var e = Instantiate(Enemy, enemyPositions[i]);
				if (enemies.Count == 1)
					e.transform.parent = enemyPositions[1];
				e.transform.position = e.transform.parent.position;
				enemies[i].enemyObject = e;
				enemies[i].EntityObject = e;
				enemies[i].Setup();
				enemies[i].enemyScript.SetImage(enemies[i]);
				enemies[i].ID = "e" + (i + 1);
				BattleEntities.Add(enemies[i]);
			}
		}

		void SetUpAllyCharacters()
		{
			for (int i = 0; i < activeChars.Length; i++)
			{

				activeChars[i] = TeamManager.S.characters[i];
				GameObject thisChar = TeamManager.S.characters[i].GetCharacter();
				thisChar.SetActive(true);
				thisChar.transform.position = allyPositions[i].position;
				thisChar.GetComponent<BattleEntityAnimator>().Switch(false, allyPositions[i].position);
				BattleEntities.Add(activeChars[i]);
			}
			currentChar = activeChars[0];
			UpdateStats();
		}

		void SetUpQueue()
		{
			foreach (var e in BattleEntities)
			{
				if (e.LastTick == 0)
				{
					e.LastTick = UnityEngine.Random.Range(0, 5);
				}
				else
				{
					//e.LastTick = e.CurrentTick;
				}
			}
		}

		//a játék elején, majd minden lépés után lefuttatni
		//ha karaktercsere van, akkor kezelni!!!
		public IEnumerator RunQueue(bool switchRun)
		{
			bool canRun = true;
			int roof = BattleEntities.Select(x => x).Max(x => x.Level);

			yield return new WaitForSeconds(0.5f);

			for (int i = 0; i < 102; i++)
			{     //roofot kicserélni											

				foreach (var e in BattleEntities)
				{
					e.LastTick += e.Initiative;
					if (e.LastTick >= roof)
					{
						EntityQueue.Add(e);
						e.LastTick -= roof;

						var block = Instantiate(QueueElement, QueuePanel.transform);
						block.GetComponent<Image>().sprite = DataHolder.S.GetAllyImage(e.EntityObject);
						block.name = e.ID;
						if (e is Enemy)
						{
							int index = Enemies.IndexOf((Enemy)e);
							block.transform.GetChild(0).GetComponent<Text>().text = "E" + (index + 1);
						}
					}
				}
				if (EntityQueue.Count > 10 || i > 1000)
				{
					canRun = false;
					DecideTurn();
				}


				yield return new WaitUntil(() => canRun);
			}
		}

		void DecideTurn()
		{
			print(EntityQueue[0].EntityObject.name);
			currentEntity = EntityQueue[0];
			if (EntityQueue[0] is Ally)
			{
				print("ally turn");
				StartAllyTurn((Ally)EntityQueue[0]);
			}
			else
			{
				StartEnemyTurn((Enemy)EntityQueue[0]);
			}
		}

		public void StartAllyTurn(Ally ally)
		{
			turnType = TurnType.Ally_turn;
			//currentChar = activeChars.Select(x => x).FirstOrDefault(x => x.EntityObject == Ally);
			currentChar = ally;
			if (currentChar.isCorrupted) //corrupted
			{
				currentChar.SelectAbility(0);
				var targetChars = new List<Ally>(activeChars);
				targetChars.Remove(currentChar);
				int target = UnityEngine.Random.Range(0, targetChars.Count);
				int dmg = (currentChar.selectedAbility.power + currentChar.power) / 2;
				dmg += Mathf.RoundToInt(dmg * UnityEngine.Random.Range(-0.1f, 0.1f));

				currentChar.EntityAnimator.Attack(AttackType.Punch, dmg, currentChar.selectedAbility.castTime);
				currentChar.EntityAnimator.GetAllyTarget(targetChars[target]);
				targetChars[target].LoseHealth(dmg);
			}
			else //nem corrupted
			{
				currentChar.allyScript.SetMarkerVisible(true);

				ActionButtons.gameObject.SetActive(true);
				CurrentCharImage.gameObject.SetActive(true);

				CurrentCharImage.GetComponent<Image>().sprite = DataHolder.S.GetAllyImage(currentChar.EntityObject);
				for (int i = 0; i < ActionButtons.transform.childCount; i++)
				{
					//AbilityButtons.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = currentChar.abilities[i].name + "";
				}
			}
		}

		public void StartEnemyTurn(Enemy e)
		{
			turnType = TurnType.Enemy_turn;

			ActionButtons.gameObject.SetActive(false);
			CurrentCharImage.gameObject.SetActive(false);

			//actions...
			//var e = Enemies.Select(x => x).FirstOrDefault(x => x.enemyObject == Enemy);		
			int target = UnityEngine.Random.Range(0, 3);
			int dmg = e.Power + Mathf.RoundToInt(e.Power * UnityEngine.Random.Range(-0.1f, 0.1f));
			activeChars[target].LoseHealth(dmg);
			e.enemyObject.GetComponent<BattleEntityAnimator>().Attack(AttackType.Punch, dmg, 1);
			e.enemyObject.GetComponent<BattleEntityAnimator>().GetAllyTarget(activeChars[target]);
		}

		public void EndEntityTurn()
		{
			if (turnType == TurnType.Ally_turn)
			{
				currentChar.EntityObject.GetComponent<Animator>().SetBool("isIdle", true);
				currentChar.allyScript.SetMarkerVisible(false);

				isAttacking = false;
				currentChar.SelectAbility(null);
				currentChar = null;
			}

			EntityQueue.RemoveAt(0);
			Destroy(QueuePanel.transform.GetChild(0).gameObject);
			UpdateStats();
			StartCoroutine(RunQueue(false));
		}

		public void UpdateStats()
		{
			for (int i = 0; i < activeChars.Length; i++)
			{
				StatPanel.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = activeChars[i].name;
				StatPanel.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = (float)activeChars[i].cur_health / activeChars[i].max_health;
				StatPanel.transform.GetChild(i).GetChild(1).GetChild(1).GetComponent<Image>().fillAmount = (float)activeChars[i].cur_energy / activeChars[i].max_energy;
				StatPanel.transform.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text = activeChars[i].cur_health + "/" + activeChars[i].max_health;
				StatPanel.transform.GetChild(i).GetChild(3).GetComponent<TextMeshProUGUI>().text = activeChars[i].cur_energy + "/" + activeChars[i].max_energy;
				StatPanel.transform.GetChild(i).GetChild(4).transform.gameObject.SetActive(activeChars[i].isCorrupted);

				if ((float)activeChars[i].cur_health > (float)activeChars[i].max_health * 0.4)
				{
					StatPanel.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(83, 183, 0, 255);
				}
				else if ((float)activeChars[i].cur_health > (float)activeChars[i].max_health * 0.2)
				{
					StatPanel.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(183, 169, 0, 255);
				}
				else
				{
					StatPanel.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(183, 67, 0, 255);
				}
			}
		}

		public void OpenSwitchPanel()
		{
			SwitchPanel.SetActive(!SwitchPanel.activeSelf);
			MediaManager.PlaySound(MediaManager.Sound.ButtonPress);
			//...listázza minden olyan char-t aki nincs benne az activeChars-ban		
		}

		public void SwitchAllyTo(string charName)
		{
			SwitchPanel.SetActive(false);
			MediaManager.PlaySound(MediaManager.Sound.ButtonPress);

			//set vars & elements
			Ally selectedChar = TeamManager.S.characters.Select(x => x).FirstOrDefault(x => x.name == charName);
			int index = Array.IndexOf(activeChars, currentChar);
			activeChars[index] = selectedChar;

			//switch model	
			currentChar.EntityAnimator.Switch(true, allyPositions[index].position);
			selectedChar.EntityAnimator.Switch(false, allyPositions[index].position);

			//update stats
			StatPanel.transform.GetChild(index).GetChild(0).GetComponent<Text>().text = selectedChar.name;
			UpdateStats();

			//update ability panel
			CurrentCharImage.GetComponent<Image>().sprite = DataHolder.S.GetAllyImage(selectedChar.EntityObject);
			selectedChar.allyScript.SetMarkerVisible(true);
			for (int i = 0; i < ActionButtons.transform.childCount; i++)
			{
				//AbilityButtons.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = selectedChar.abilities[i].name + "";
			}



			//update queue
			for (int i = 0; i < EntityQueue.Count; i++)
			{
				if (EntityQueue[i] == currentChar)
				{
					EntityQueue[i] = selectedChar;
					QueuePanel.transform.GetChild(i).GetComponent<Image>().sprite = DataHolder.S.GetAllyImage(selectedChar.EntityObject);
				}
			}


			currentChar = selectedChar;
			//StartCoroutine(RunQueue(true));



			//update switch panel
			for (int i = 0; i < SwitchPanel.transform.childCount; i++)
			{
				Destroy(SwitchPanel.transform.GetChild(i).gameObject);
			}
			var inactives = TeamManager.S.characters.Select(x => x).Where(x => !activeChars.Contains(x)).ToList();
			for (int i = 0; i < inactives.Count(); i++)
			{
				var item = Instantiate(SwitchElement, SwitchPanel.transform);
				item.transform.GetComponent<Image>().sprite = DataHolder.S.GetAllyImage(inactives[i].EntityObject);
				string name = inactives[i].name;
				item.transform.GetComponent<Button>().onClick.AddListener(() => SwitchAllyTo(name));
			}
		}


		public void ClickOnAction(int actionNum)
		{
			MediaManager.PlaySound(MediaManager.Sound.ButtonPress);

			if (currentChar == null)
				return;

			switch (actionNum)
			{
				case 0:
					UseAbility(0);
					AbilityPanel.SetActive(false);
					ItemPanel.SetActive(false);
					break;
				case 1:
					OpenAbilityPanel();
					break;
				case 2:
					OpenItemPanel();
					break;
			}
		}

		public void OpenAbilityPanel()
		{
			AbilityPanel.SetActive(!AbilityPanel.activeSelf);
			ItemPanel.SetActive(false);
			ResetMarkers();

			foreach (Transform item in AbilityPanel.transform.GetChild(0).GetChild(0))
			{
				Destroy(item.gameObject);
			}
			for (int i = 1; i < currentChar.abilities.Count; i++)
			{
				var r = Instantiate(AbilityRow, AbilityPanel.transform.GetChild(0).GetChild(0));
				r.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentChar.abilities[i].name + "";
				r.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "-" + currentChar.abilities[i].energy;
				int num = 0 + i;
				r.GetComponent<Button>().onClick.AddListener(delegate { UseAbility(num); });
			}
		}

		public void UseAbility(int num)
		{
			print(num);
			if (num != 0)
				MediaManager.PlaySound(MediaManager.Sound.ButtonPress);

			if (currentChar.CheckAbility(num))
			{
				currentChar.SelectAbility(num);
				/*currentChar.allyScript.SetMarkerVisible(false);
				for (int i = 0; i < Enemies.Count; i++)
				{
					Enemies[i].enemyScript.SetTargetable(true);
				}*/
				//¡¡¡ez lesz a jó, kicserélni a fenti sorokat erre, ha különszedtük már az abilityt
				currentChar.selectedAbility.SetTargetMarkers();
				AbilityPanel.SetActive(false);
			}
			else
			{
				Debug.LogError("ability can't be selected");
				//jelzés...
			}
		}

		public void OpenItemPanel()
		{
			ItemPanel.SetActive(!ItemPanel.activeSelf);
			AbilityPanel.SetActive(false);
			ResetMarkers();

			foreach (Transform item in ItemPanel.transform.GetChild(0).GetChild(0))
			{
				Destroy(item.gameObject);
			}
			for (int i = 0; i < Inventory.S.itemDict.Count; i++)
			{
				var r = Instantiate(ItemRow, ItemPanel.transform.GetChild(0).GetChild(0));
				var item = Inventory.S.itemDict.ElementAt(i);
				r.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Key.name + "";
				r.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Value + "";
				int num = 0 + i;
				r.GetComponent<Button>().onClick.AddListener(delegate { UseItem(item); });
			}
		}
		
		public void UseItem(KeyValuePair<Item, int> item)
        {
			currentChar.selectedAction = item.Key;
			MediaManager.PlaySound(MediaManager.Sound.ButtonPress);
			item.Key.SetTargetMarkers();
			ItemPanel.SetActive(false);			
		}

		//amikor az enemyre nyomunk
		public void PrepareAttackEnemy(GameObject enemy = null)
		{
			if (currentChar.selectedAbility == null || isAttacking)
				return;
			isAttacking = true;
			ActionButtons.gameObject.SetActive(false);

			ResetMarkers();

			if (enemy != null)
			{
				selectedEnemy = Enemies
				.Select(x => x)
				.FirstOrDefault(x => x.enemyObject == enemy);
			}

			AttackEnemy();
		}

		//ebben kéne kiválasztani az attack típusát, és annyiszor meghívni a DamageEnemy()-t, ahányszor kell...
		public void AttackEnemy()
		{
			//values calculate + update
			currentChar.ConsumeEnergy(currentChar.selectedAbility.energy);
			int dmg = currentChar.CalculateDamage();
			currentChar.GetBattlePoints(5);


			//attack animation ..	
			currentChar.EntityObject.GetComponent<Animator>().SetBool("isIdle", false);			
			currentChar.EntityAnimator.Attack(currentChar.selectedAbility.attackType, dmg, currentChar.selectedAbility.castTime);
			if (!currentChar.selectedAbility.multitarget)
				currentChar.EntityAnimator.GetEnemyTarget(selectedEnemy);
			else
				currentChar.EntityAnimator.GetEnemyTarget(Enemies);
			MediaManager.PlaySound(MediaManager.Sound.ButtonPress);
		}

		public void RemoveEnemy(Enemy e)
        {
			BattleEntities.Remove(e);
			EntityQueue.RemoveAll(item => item == e);
			e.enemyObject.SetActive(false);

			//remove instances from QUEUE panel
			foreach (Transform child in QueuePanel.transform)
			{
				if (child.name == e.ID)
					Destroy(child.gameObject);
			}

			//...score, animation, sound, etc
			CheckEndOfBattle();
		}

		public void CheckEndOfBattle()
		{
			print(Enemies.Select(x => x).Count(x => !x.isDead));
			if (Enemies.Select(x => x).Count(x => !x.isDead) == 0)
			{
				EndPanel.SetActive(true);
			}

			if (activeChars.Select(x => x).Count(x => !x.isCorrupted) == 0)
			{
				EndPanel.SetActive(true);
			}

			for (int i = 0; i < TeamManager.S.characters.Count; i++)
			{
				TeamManager.S.characters[i].UpdateLevel();
			}
		}


		public void ResetMarkers()
		{
			for (int i = 0; i < BattleEntities.Count; i++)
			{
				print(BattleEntities[i].entityScript.name);
				BattleEntities[i].entityScript.SetMarkerVisible(false);
			}
		}
	}
}