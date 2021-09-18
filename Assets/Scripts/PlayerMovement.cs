using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
  private Rigidbody rb;
  private Vector3 direction = Vector3.zero;
  public GameObject[] spawnPoints = null;
  public float speed = 10.0f;
  public float zLimit = 40;
  // Start is called before the first frame update
  void Start()
  {
    if(!isLocalPlayer)
    {
      return;
    }

    rb = GetComponent<Rigidbody>();

    spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
  }

  private void Update()
  {
    if (!isLocalPlayer)
    {
      return;
    }

    float horzMove = Input.GetAxis("Horizontal");
    float vertMove = Input.GetAxis("Vertical");

    direction = new Vector3(horzMove, 0, vertMove);
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    if (!isLocalPlayer)
    {
      return;
    }

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
    int index = 0;
    while(Physics.CheckBox(spawnPoints[index].transform.position, new Vector3(1.5f, 1.5f, 1.5f)))
    {
      index++;
    }
    rb.MovePosition(spawnPoints[index].transform.position);
  }

  private void OnTriggerExit(Collider other)
  {
    if (!isLocalPlayer)
    {
      return;
    }

    if (other.CompareTag("Hazard")) {
      Respawn();
    }
  }
}
