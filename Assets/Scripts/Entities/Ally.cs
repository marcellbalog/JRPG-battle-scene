﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static ChristmasBattle.TeamManager;

namespace ChristmasBattle
{
	[System.Serializable]
	public class Ally : BattleEntity
	{
		public AllyObjectScript allyScript;
		//public GameObject Marker;
		public Image Image;
		public Transform Attackpoint;
		Member member;
		public string name;

		//public int Level;		
		public int cur_xp;
		public int new_xp;

		public int max_health;
		public int max_energy;
		public int power;
		public int luck;
		//public int initiative;

		public int cur_health;
		public int cur_energy;
		public bool isCorrupted;
		public int synkmeter;

		//public List<cAbility> cabilities = new List<cAbility>();
		//public cAbility cSelectedAbility;
		public List<Ability> abilities = new List<Ability>();
		public Ability selectedAbility;
		public Action selectedAction;


		public Ally(GameObject thisChar, Member m)
		{
			EntityObject = thisChar;			
			member = m;			
			Attackpoint = thisChar.transform.Find("attackpoint");

			for (int i = 0; i < thisChar.transform.childCount; i++)
			{
				Attackpoint = thisChar.transform.GetChild(i).Find("attackpoint");
				if (Attackpoint != null)
					break;
			}

			Setup();
		}

        public override void Setup()
        {
            base.Setup();
			allyScript = EntityObject.transform.GetComponent<AllyObjectScript>();
			allyScript.thisEntity = this;
		}

        public GameObject GetCharacter()
		{
			return EntityObject;
		}

		public void AddAbility(Ability ability)
		{
			if (!abilities.Contains(ability))
				abilities.Add(ability);
		}

		public bool CheckAbility(int num)
		{
			bool isUsable = true;
			if (cur_energy < abilities[num].energy)
				isUsable = false;

			return isUsable;
		}

		public void SelectAbility(int? num)
        {
			if (num != null)
				selectedAbility = abilities[(int)num];
			else
				selectedAbility = null;

			selectedAction = selectedAbility;
        }

		public void GetBattlePoints(int p)
		{
			new_xp += p;
		}

		public int UpdateLevel()
		{
			int levelUp = 0;

			int xpForNext = Mathf.RoundToInt(0.04f * (Level ^ 3) + 0.8f * (Level ^ 2) + 2 * Level);

			int d = new_xp + cur_xp;
			while (d >= xpForNext)
			{
				d -= xpForNext;
				levelUp++;
				cur_xp = d;
			}

			new_xp = 0;

			if (levelUp > 0)
				Debug.Log(name + " leveled up! (+" + levelUp + ")");
			return levelUp;
		}

		public Vector3 FindAttackPoint()
		{
			return (Attackpoint ??= EntityObject.transform.Find("attackpoint")).position;
		}
		 
		public override void LoseHealth(int damage)
		{
			cur_health -= damage;
			if (cur_health < 0 && !isCorrupted)
			{
				cur_health = 0;
				isCorrupted = true;
				allyScript.GetCorrupted(ID);
			}
		}

		public override void Heal(float amount)
		{
			cur_health += Mathf.RoundToInt(max_health * amount);
			Debug.Log(max_health * amount + "  " + amount);
			if (cur_health > max_health)
			{
				cur_health = max_health;
			}
		}

		public void Cure(int amount = 0)
		{
			isCorrupted = false;
			if (amount > 0)
				cur_health = amount;
			else
				cur_health = max_health / 50;

			allyScript.CureCorruption(ID);
			
		}

		public void ConsumeEnergy(int amount)
        {
			cur_energy -= amount;
			if (cur_energy < 0)
				cur_energy = 0;
		}

		public override void GainEnergy(float amount)
        {
			cur_energy += Mathf.RoundToInt(max_energy * amount);
			if (cur_energy > max_energy)
				cur_energy = max_energy;
		}

		public int CalculateDamage()
        {
			int dmg = selectedAbility.power + power;
			dmg += Mathf.RoundToInt(dmg * UnityEngine.Random.Range(-0.1f, 0.1f));
			return dmg;
		}
	}

}