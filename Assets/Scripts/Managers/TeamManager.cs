using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ChristmasBattle
{
	public class TeamManager : MonoBehaviour
	{
		public static TeamManager instance;

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
		
		
		private void Awake()
		{
			instance = this;
			SetupChars();
		}

		void SetupChars()
		{
			foreach (var ally in characters)
			{
				ally.EntityObject = EntityObjects.First(x => x.GetComponent<AllyObjectScript>().member == ally.member);
				ally.Setup();
			}			
		}
	}

}