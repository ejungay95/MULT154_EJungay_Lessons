using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountGUI : MonoBehaviour
{
  private TextMeshProUGUI tmPro;
  private int count = 0;
  public string itemName;

  // Start is called before the first frame update
  void Start()
  {
    tmPro = GetComponent<TextMeshProUGUI>(); 
  }


  public void UpdateCount()
  {
    count++;
    tmPro.text = itemName + ": " + count;
  }
}
