using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ChristmasBattle
{
	[CreateAssetMenu]
	public class Enemy : BattleEntity
	{
		public GameObject EnemyObject {		
			get { return EntityObject; }
			set { EntityObject = value; }
		}
		public Sprite Image { get; set; }
		public string nameText;
		private EnemyObjectScript enemyScript { get; set; }		
		private DataHolder.EnemyName Name; 
		private int max_Health;
		private int cur_Health;
		public int power;

		public bool isDead { get; set; }

		public Enemy(Enemy e) {			
			nameText = e.nameText;
			Level = e.Level;
			cur_Health = e.max_Health;
			max_Health = e.cur_Health;
			power = e.power;
			Initiative = e.Initiative;

			Enum.TryParse(nameText, out Name);
			Image = DataHolder.instance.GetEnemyImage(Name);

			//base.Setup();
		}

        public override void Setup()
        {
			base.Setup();
			enemyScript = EntityObject.transform.GetComponent<EnemyObjectScript>();
			enemyScript.thisEntity = this;
			enemyScript.SetImage(this);			
		}

		public override void LoseHealth(int x) {
			cur_Health -= x;
			if (cur_Health < 0) {
				isDead = true;
				BattleManager.instance.RemoveEnemy(this);
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
