using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
	[System.Serializable]
	public abstract class BattleEntity
	{
		public int Level;
		public int Initiative;
		public int CurrentTick;
		public int LastTick;
		public GameObject EntityObject;
		public BattleEntityObjectScript entityScript;
		public BattleEntityAnimator EntityAnimator;
		public string ID;

		public enum AbilityOwner
		{
			Rudolf,
			Snowan,
			Santa,
			Elf,
			Gingerbread
		};

		public virtual void Setup()
        {
			if (!EntityObject)
			{
				Debug.LogError("Missing entity object...");
				return;
			}
			EntityAnimator = EntityObject.transform.GetComponent<BattleEntityAnimator>();
			entityScript = EntityObject.transform.GetComponent<BattleEntityObjectScript>();
			entityScript.thisEntity = this;
		}

		public abstract void LoseHealth(int amount);

		public abstract void Heal(float amount);

		public virtual void GainEnergy(float amount) { }
	}
}
