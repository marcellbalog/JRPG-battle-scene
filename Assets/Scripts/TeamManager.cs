using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ChristmasBattle
{
	public class TeamManager : MonoBehaviour
	{
		public static TeamManager S;

		public List<Ability> rudolfAbilities;
		public List<Ability> snowmanAbilities;
		public List<Ability> santaAbilities;
		public List<Ability> elfAbilities;
		public List<Ability> gingerbreadAbilities;

		int teamSize = 5;
		public List<Ally> characters = new List<Ally>();
		public List<GameObject> EntityObjects = new List<GameObject>();		
		public enum Member
		{			
			Rudolf,
			Snowman,
			Santa,
			Elf,
			Gingerbread
		};
		Member mCounter = 0;
		
		
		private void Awake()
		{
			S = this;
			SetupChars();
		}

		void SetupChars()
		{
			for (int i = 0; i < teamSize; i++)
			{
				Ally c = new Ally(EntityObjects[i], mCounter);
				characters.Add(c);
				c.allyScript = EntityObjects[i].GetComponent<AllyObjectScript>();

				if (mCounter == Member.Rudolf)
				{
					c.ID = "c01";
					c.name = "Rudolf";
					c.Level = 1;
					c.max_health = 200;
					c.power = 30;
					c.max_energy = 20;
					c.luck = 0;
					c.Initiative = 4;

					
					c.abilities = rudolfAbilities;
				}
				else if (mCounter == Member.Snowman)
				{
					c.ID = "c02";
					c.name = "Snowan";
					c.Level = 1;
					c.max_health = 120;
					c.power = 2;
					c.max_energy = 10;
					c.luck = 1;
					c.Initiative = 4;

					c.abilities = snowmanAbilities;

				}
				else if (mCounter == Member.Santa)
				{
					c.ID = "c03";
					c.name = "Santa";
					c.Level = 1;
					c.max_health = 180;
					c.power = 1;
					c.max_energy = 20;
					c.luck = 0;
					c.Initiative = 6;

					c.abilities = santaAbilities;

				}
				else if (mCounter == Member.Elf)
				{
					c.ID = "c04";
					c.name = "Elf";
					c.Level = 1;
					c.max_health = 200;
					c.power = 1;
					c.max_energy = 20;
					c.luck = 2;
					c.Initiative = 1;

					c.abilities = gingerbreadAbilities;

				}
				else if (mCounter == Member.Gingerbread)
				{
					c.ID = "c05";
					c.name = "Gingerbread";
					c.Level = 1;
					c.max_health = 140;
					c.power = 1;
					c.max_energy = 20;
					c.luck = 2;
					c.Initiative = 5;

					c.abilities = gingerbreadAbilities;

				}

				c.cur_health = c.max_health;
				c.cur_energy = c.max_energy;

				mCounter++;
			}


		}


	}


	public static class EnumUtils
	{
		public static Nullable<T> Parse<T>(string input) where T : struct
		{
			//since we cant do a generic type constraint
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("Generic Type 'T' must be an Enum");
			}
			if (!string.IsNullOrEmpty(input))
			{
				if (Enum.GetNames(typeof(T)).Any(
					  e => e.Trim().ToUpperInvariant() == input.Trim().ToUpperInvariant()))
				{
					return (T)Enum.Parse(typeof(T), input, true);
				}
			}
			return null;
		}
	}

}