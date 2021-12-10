using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryManager : NetworkBehaviour
{
  private Dictionary<ItemPickUp.VegetableType, int> Inventory = new Dictionary<ItemPickUp.VegetableType, int>();

  public delegate void CollectItem(ItemPickUp.VegetableType item);
  public static event CollectItem ItemCollected;

  Collider itemCollider = null;

  // Start is called before the first frame update
  void Start()
  {
    foreach (ItemPickUp.VegetableType item in System.Enum.GetValues(typeof(ItemPickUp.VegetableType))) {
      Inventory.Add(item, 0);
    }
  }

  private void Update()
  {
    if (!isLocalPlayer)
    {
      return;
    }

    if (itemCollider && Input.GetKeyDown(KeyCode.Space))
    {
      ItemPickUp item = itemCollider.gameObject.GetComponent<ItemPickUp>();
      AddToInventory(item);
      PrintInventory();
      CmdItemCollected(item.typeOfVegetable);
    }
  }

  [Command]
  void CmdItemCollected(ItemPickUp.VegetableType itemType)
  {
    RpcItemCollected(itemType);
  }

  [ClientRpc]
  void RpcItemCollected(ItemPickUp.VegetableType itemType)
  {
    ItemCollected?.Invoke(itemType);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!isLocalPlayer)
    {
      return;
    }

    if (other.CompareTag("Item"))
    {
      itemCollider = other;
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (!isLocalPlayer)
    {
      return;
    }

    if (other.CompareTag("Item"))
    {
      itemCollider = null;
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
