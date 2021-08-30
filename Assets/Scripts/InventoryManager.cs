using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
  private Dictionary<ItemPickUp.VegetableType, int> Inventory = new Dictionary<ItemPickUp.VegetableType, int>();

  // Start is called before the first frame update
  void Start()
  {
    foreach (ItemPickUp.VegetableType item in System.Enum.GetValues(typeof(ItemPickUp.VegetableType))) {
      Inventory.Add(item, 0);
    }
  }

  public void AddToInventory(ItemPickUp item)
  {
    Inventory[item.typeOfVegetable]++;
  }

  public void PrintInventory() {
    string output = "";

    foreach (KeyValuePair<ItemPickUp.VegetableType, int> kvp in Inventory) {
      output += string.Format("{0}: {1}, ", kvp.Key, kvp.Value);
    }

    Debug.Log(output);
  }
}
