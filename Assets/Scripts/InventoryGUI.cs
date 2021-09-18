using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGUI : MonoBehaviour
{
  public List<GameObject> items;

  // Start is called before the first frame update
  void Start()
  {
    InventoryManager.ItemCollected += IncrementItem; 
  }

  // Update is called once per frame
  void Update()
  {
        
  }

  void IncrementItem(ItemPickUp.VegetableType itemType)
  {
    CountGUI countGUI = items[(int)itemType].GetComponent<CountGUI>();
    countGUI.UpdateCount();
  }
}
