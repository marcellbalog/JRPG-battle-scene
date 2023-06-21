using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ChristmasBattle
{
	[System.Serializable]
	public class Enemy : BattleEntity
	{
		public GameObject enemyObject { get; set; }
		public Sprite Image { get; set; }
		public EnemyObjectScript enemyScript { get; set; }
		//public string ID { get; set; }
		public string NameText;
		public DataHolder.EnemyName Name;
		//public int Level;
		public int max_Health;
		public int cur_Health;
		public int Power;
		//public int Initiative;

		public bool isDead { get; set; }

		public Enemy(Enemy e) {			
			NameText = e.NameText;
			Level = e.Level;
			cur_Health = e.max_Health;
			max_Health = e.cur_Health;
			Power = e.Power;
			Initiative = e.Initiative;

			Enum.TryParse(NameText, out Name);
			Image = DataHolder.S.GetEnemyImage(Name);

			//base.Setup();
		}
        public override void Setup()
        {
			base.Setup();
			enemyScript = EntityObject.transform.GetComponent<EnemyObjectScript>();
			enemyScript.thisEntity = this;
		}

		public override void LoseHealth(int x) {
			cur_Health -= x;
			if (cur_Health < 0) {
				isDead = true;
				BattleManager.S.RemoveEnemy(this);
			}			
		}

        public override void Heal(float amount)
        {
			cur_Health += (int)amount;
			if (cur_Health > max_Health)
			{
				cur_Health = max_Health;
			}
		}

        public override void GainEnergy(float amount)
        {
            throw new NotImplementedException("Tried to restore energy for enemies...");
        }
    }


}
