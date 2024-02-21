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
		public static BattleManager instance;

		[Header("Battle Data")]

		public Ally[] activeChars = new Ally[3];
		[SerializeField] private Transform[] allyPositions = new Transform[3];
		[SerializeField] private Transform[] enemyPositions = new Transform[3];
		[SerializeField] private GameObject Enemy;

		private List<Enemy> _testEnemies = new List<Enemy>();
		public List<Enemy> Enemies = new List<Enemy>();		

		public BattleEntity currentEntity;
		public Ally currentChar;

		[SerializeReference]
		public List<BattleEntity> BattleEntities = new List<BattleEntity>();
		private List<BattleEntity> EntityQueue = new List<BattleEntity>();

		private bool isAttacking;
		private Enemy selectedEnemy;

		[Header("UI")]
		[SerializeField] private GameObject CurrentCharImage;
		[SerializeField] private GameObject ActionButtons;
		[SerializeField] private GameObject AbilityPanel;
		[SerializeField] private GameObject AbilityRow;
		[SerializeField] private GameObject ItemPanel;
		[SerializeField] private GameObject ItemRow;
		[SerializeField] private GameObject StatPanel;

		public GameObject TextHolder;
		public GameObject DamageText;
		[SerializeField] private GameObject EndPanel;
		[SerializeField] private GameObject StartMenuPanel;

		public GameObject QueuePanel;
		public GameObject QueueElement;
		public GameObject SwitchPanel;
		public GameObject SwitchElement;

		[SerializeField] private AudioClip battleMusic;
		[SerializeField] private AudioSource music;

		public enum TurnType
        {
			Ally_turn,
			Enemy_turn,
			Talking_turn
        }
		public TurnType turnType;

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			StartMenuPanel.SetActive(true);
			_testEnemies.Add(DataHolder.instance.CreateNewEnemy(DataHolder.EnemyName.Toy));
			_testEnemies.Add(DataHolder.instance.CreateNewEnemy(DataHolder.EnemyName.Grinch));
			_testEnemies.Add(DataHolder.instance.CreateNewEnemy(DataHolder.EnemyName.Monster));
		}

		public void StartBattle()
		{
			SetUpAllyCharacters();
			CreateEnemies(_testEnemies);
			SetUpQueue();
			StartCoroutine(RunQueue(false));
			music.clip = battleMusic;
			music.Play();
		}

		void SetUpAllyCharacters()
		{
			for (int i = 0; i < activeChars.Length; i++)
			{
				activeChars[i] = TeamManager.instance.characters[i];
				GameObject thisChar = TeamManager.instance.characters[i].GetCharacterObject();
				thisChar.SetActive(true);
				thisChar.transform.position = allyPositions[i].position;
				thisChar.GetComponent<BattleEntityAnimator>().Switch(false, allyPositions[i].position);
				BattleEntities.Add(activeChars[i]);
			}
			currentChar = activeChars[0];
			UpdateStats();
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
				enemies[i].EnemyObject = e;				
				enemies[i].Setup();				
				enemies[i].ID = "e" + (i + 1);
				BattleEntities.Add(enemies[i]);
			}
		}

		//for randomizing?
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

		// Implementation of the queue mechanic
		// Runs at the beginning of the battle and after every turn		
		//ha karaktercsere van, akkor kezelni!!!
		public IEnumerator RunQueue(bool switchRun)
		{
			int roof = BattleEntities.Select(x => x).Max(x => x.Level);	 

			yield return new WaitForSeconds(0.5f);

			for (int i = 0; i < 1000; i++)
			{									

				foreach (var e in BattleEntities)
				{
					e.LastTick += e.Initiative;
					if (e.LastTick >= roof)
					{
						EntityQueue.Add(e);
						e.LastTick -= roof;

						var block = Instantiate(QueueElement, QueuePanel.transform);
						block.GetComponent<Image>().sprite = DataHolder.instance.GetAllyImage(e.EntityObject);
						block.name = e.ID;
						if (e is Enemy)
						{
							int index = Enemies.IndexOf((Enemy)e);
							block.transform.GetChild(0).GetComponent<Text>().text = "E" + (index + 1);
						}
					}
				}
				if (EntityQueue.Count > 10)
				{					
					break;
				}						
			}

			DecideTurn();
		}

		void DecideTurn()
		{			
			currentEntity = EntityQueue[0];
			if (currentEntity is Ally)
			{				
				StartAllyTurn((Ally)currentEntity);
			}
			else
			{
				PlayEnemyTurn((Enemy)currentEntity);
			}
		}

		public void StartAllyTurn(Ally ally)
		{
			turnType = TurnType.Ally_turn;
			
			currentChar = ally;
			if (currentChar.isCorrupted)
			{
				currentChar.SelectAbility(0);

				var targetChars = new List<Ally>(activeChars);
				targetChars.Remove(currentChar);

				int target = UnityEngine.Random.Range(0, targetChars.Count);
				int baseDamage = (currentChar.selectedAbility.power + currentChar.power) / 2;
				int damage = baseDamage + Mathf.RoundToInt(baseDamage * UnityEngine.Random.Range(-0.1f, 0.1f));

				currentChar.EntityAnimator.Attack(AttackType.Punch, damage, currentChar.selectedAbility.castTime);
				currentChar.EntityAnimator.GetAllyTarget(targetChars[target]);
				targetChars[target].LoseHealth(damage);
			}
			else if (!currentChar.isCorrupted)
			{
				currentChar.allyScript.SetMarkerVisible(true);

				ActionButtons.gameObject.SetActive(true);
				CurrentCharImage.gameObject.SetActive(true);

				CurrentCharImage.GetComponent<Image>().sprite = DataHolder.instance.GetAllyImage(currentChar.EntityObject);
			}
		}

		public void PlayEnemyTurn(Enemy enemy)
		{
			turnType = TurnType.Enemy_turn;

			ActionButtons.gameObject.SetActive(false);
			CurrentCharImage.gameObject.SetActive(false);

			//actions...					
			int target = UnityEngine.Random.Range(0, 3);
			int dmg = enemy.power + Mathf.RoundToInt(enemy.power * UnityEngine.Random.Range(-0.1f, 0.1f));
			activeChars[target].LoseHealth(dmg);
			enemy.EnemyObject.GetComponent<BattleEntityAnimator>().GetAllyTarget(activeChars[target]);
			enemy.EnemyObject.GetComponent<BattleEntityAnimator>().Attack(AttackType.Punch, dmg, 1);
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

				SetHealthColor(i, activeChars[i].cur_health, activeChars[i].max_health);
			}
		}

		void SetHealthColor(int index, float currentHealth, float maxHealth)
		{
			Image healthImage = StatPanel.transform.GetChild(index).GetChild(1).GetChild(0).GetComponent<Image>();

			Color32 healthyColor = new Color32(83, 183, 0, 255);
			Color32 moderateColor = new Color32(183, 169, 0, 255);
			Color32 criticalColor = new Color32(183, 67, 0, 255);

			if (currentHealth > maxHealth * 0.4)
			{
				healthImage.color = healthyColor;
			}
			else if (currentHealth > maxHealth * 0.2)
			{
				healthImage.color = moderateColor;
			}
			else
			{
				healthImage.color = criticalColor;
			}
		}


		public void OpenSwitchPanel()
		{
			SwitchPanel.SetActive(!SwitchPanel.activeSelf);
			MediaManager.PlaySound(MediaManager.Sound.ButtonPress);					
		}

		public void SwitchAllyTo(string charName)
		{
			SwitchPanel.SetActive(false);
			MediaManager.PlaySound(MediaManager.Sound.ButtonPress);

			//set vars & elements
			Ally selectedChar = TeamManager.instance.characters.Select(x => x).FirstOrDefault(x => x.name == charName);
			int index = Array.IndexOf(activeChars, currentChar);
			activeChars[index] = selectedChar;

			//switch model	
			currentChar.EntityAnimator.Switch(true, allyPositions[index].position);
			selectedChar.EntityAnimator.Switch(false, allyPositions[index].position);

			//update UI
			StatPanel.transform.GetChild(index).GetChild(0).GetComponent<Text>().text = selectedChar.name;
			UpdateStats();			
			CurrentCharImage.GetComponent<Image>().sprite = DataHolder.instance.GetAllyImage(selectedChar.EntityObject);


			selectedChar.allyScript.SetMarkerVisible(true);			


			//update queue
			for (int i = 0; i < EntityQueue.Count; i++)
			{
				if (EntityQueue[i] == currentChar)
				{
					EntityQueue[i] = selectedChar;
					QueuePanel.transform.GetChild(i).GetComponent<Image>().sprite = DataHolder.instance.GetAllyImage(selectedChar.EntityObject);
				}
			}


			currentChar = selectedChar;


			//update switch panel
			for (int i = 0; i < SwitchPanel.transform.childCount; i++)
			{
				Destroy(SwitchPanel.transform.GetChild(i).gameObject);
			}
			var inactives = TeamManager.instance.characters.Select(x => x).Where(x => !activeChars.Contains(x)).ToList();
			for (int i = 0; i < inactives.Count(); i++)
			{
				var item = Instantiate(SwitchElement, SwitchPanel.transform);
				item.transform.GetComponent<Image>().sprite = DataHolder.instance.GetAllyImage(inactives[i].EntityObject);
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
			if (num != 0)
				MediaManager.PlaySound(MediaManager.Sound.ButtonPress);

			if (currentChar.CheckAbility(num))
			{
				currentChar.SelectAbility(num);				
				currentChar.selectedAbility.SetTargetMarkers();
				AbilityPanel.SetActive(false);
			}
			else
			{
				Debug.LogError("ability can't be selected");				
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
			for (int i = 0; i < Inventory.instance.itemDict.Count; i++)
			{
				var r = Instantiate(ItemRow, ItemPanel.transform.GetChild(0).GetChild(0));
				var item = Inventory.instance.itemDict.ElementAt(i);
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

		// When clicking on an enemy to attack
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
				.FirstOrDefault(x => x.EnemyObject == enemy);
			}

			AttackEnemy();
		}
	
		public void AttackEnemy()
		{
			// Calculate & update values
			currentChar.ConsumeEnergy(currentChar.selectedAbility.energy);
			int dmg = currentChar.CalculateDamage();
			currentChar.GetBattlePoints(5);


			// Set target(s)	
			if (!currentChar.selectedAbility.multitarget)
				currentChar.EntityAnimator.GetEnemyTarget(selectedEnemy);
			else
				currentChar.EntityAnimator.GetEnemyTarget(Enemies);

			// Execute attack
			currentChar.EntityAnimator.Attack(currentChar.selectedAbility.attackType, dmg, currentChar.selectedAbility.castTime);

			//Set sound & animation
			MediaManager.PlaySound(MediaManager.Sound.ButtonPress);
			currentChar.EntityObject.GetComponent<Animator>().SetBool("isIdle", false);
		}

		public void RemoveEnemy(Enemy e)
        {
			BattleEntities.Remove(e);
			EntityQueue.RemoveAll(item => item == e);
			e.EnemyObject.SetActive(false);

			//Remove instances from QUEUE panel
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
			if (Enemies.Select(x => x).Count(x => !x.isDead) == 0)
			{
				EndPanel.SetActive(true);
			}

			if (activeChars.Select(x => x).Count(x => !x.isCorrupted) == 0)
			{
				EndPanel.SetActive(true);
			}

			for (int i = 0; i < TeamManager.instance.characters.Count; i++)
			{
				TeamManager.instance.characters[i].UpdateLevel();
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