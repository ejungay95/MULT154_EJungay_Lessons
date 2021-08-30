using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  private Rigidbody rb;
  private Vector3 direction = Vector3.zero;
  private InventoryManager inventory;
  
  public GameObject spawnPoint = null;
  public float speed = 10.0f;
  public float zLimit = 40;
  // Start is called before the first frame update
  void Start()
  {
    rb = GetComponent<Rigidbody>();
    inventory = GetComponent<InventoryManager>();
  }

  private void Update() {
    float horzMove = Input.GetAxis("Horizontal");
    float vertMove = Input.GetAxis("Vertical");

    direction = new Vector3(horzMove, 0, vertMove);
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    //rb.AddForce(direction * speed, ForceMode.Force);
    rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    
    if(transform.position.z > zLimit)
    {
      transform.position = new Vector3(transform.position.x, transform.position.y, zLimit);
    }
    else if(transform.position.z < -zLimit)
    {
      transform.position = new Vector3(transform.position.x, transform.position.y, -zLimit);
    }
  }

  private void Respawn()
  {
    rb.MovePosition(spawnPoint.transform.position);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Item"))
    {
      ItemPickUp item = other.gameObject.GetComponent<ItemPickUp>();
      inventory.AddToInventory(item);
      inventory.PrintInventory();
      Destroy(other.gameObject);
    }
    
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Hazard")) {
      Respawn();
    }
  }
}
