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
        public static Inventory instance;

        public Dictionary<Item, int> itemDict = new Dictionary<Item, int>();

        private void Start()
        {
            instance = this;
            //testing
            AddItem(DataHolder.instance.CreateNewItem("Heal"), 3);
            AddItem(DataHolder.instance.CreateNewItem("Heal Group"), 3);
            AddItem(DataHolder.instance.CreateNewItem("Energy Plus"), 3);
            print(DataHolder.instance.CreateNewItem("Energy Plus").name);
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
