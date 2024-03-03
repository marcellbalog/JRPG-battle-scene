using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ChristmasBattle
{

    public class AllyObjectScript : BattleEntityObjectScript
    {
		public TeamManager.Member member;

		public override void PlayDamageAnimAndSound()
        {
            GetComponent<Animator>().Play("AllyDamage");
        }

        public void GetCorrupted(string ID)
        {
            MediaManager.PlayEffect(MediaManager.Effect.Corrupted, transform.position);

            foreach (Transform child in BattleManager.instance.QueuePanel.transform)
            {
                if (child.name == ID)
                    child.transform.GetComponent<Image>().color = new Color32(188, 70, 70, 255);
            }
        }

        public void CureCorruption(string ID)
        {
            foreach (Transform child in BattleManager.instance.QueuePanel.transform)
            {
                if (child.name == ID)
                    child.transform.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }

        public override void ReturnToIdle()
        {
            transform.GetChild(3).gameObject.SetActive(false);
        }
    }
}
