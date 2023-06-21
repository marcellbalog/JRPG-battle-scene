using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChristmasBattle
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory S;

        public Dictionary<Item, int> itemDict = new Dictionary<Item, int>();

        private void Start()
        {
            S = this;
            //testing
            AddItem(DataHolder.S.CreateNewItem("Heal"), 3);
            AddItem(DataHolder.S.CreateNewItem("Heal Group"), 3);
            AddItem(DataHolder.S.CreateNewItem("Energy Plus"), 3);
            print(DataHolder.S.CreateNewItem("Energy Plus").name);
            foreach (var item in itemDict)
            {
                print(item.Key +" "+ item.Value);
            }
        }

        public void AddItem(Item item, int amount)
        {
            print(item.name);
            if (itemDict.ContainsKey(item))
                itemDict[item] += amount;
            else
                itemDict.Add(item, amount);
        }

        public void SelectItem(Item item)
        {
            
        }

        public void DepleteItem(Item item)
        {
            itemDict[item]--;
            if (itemDict[item] <= 0) itemDict.Remove(item);
        }
    }
}
