using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drift : MonoBehaviour
{
  public float speed = 5.0f;
  public enum DriftDirection
  {
    LEFT = -1,
    RIGHT = 1
  }

  public DriftDirection direction = DriftDirection.LEFT;

  private float edgeLimit = -80;
  // Start is called before the first frame update
  void Start()
  {
        
  }

  // Update is called once per frame
  void Update()
  {
    switch(direction) {
      case DriftDirection.LEFT:
        transform.Translate(Vector3.left * Time.deltaTime * speed);
        break;
      case DriftDirection.RIGHT:
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        break;
    } 

    if(transform.position.x < edgeLimit || transform.position.x > -edgeLimit) {
      Destroy(gameObject);
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.CompareTag("Player"))
    {
      GameObject child = collision.gameObject;
      child.transform.SetParent(gameObject.transform);
    }
  }


  // Added this because there was a weird glitch when moving from lilypads. The player would unparent to
  // the current lilypad but then wont parent to the lilypad the player wants to get to.
/*
  private void OnCollisionStay(Collision collision) {
    if (collision.gameObject.CompareTag("Player")) {
      GameObject child = collision.gameObject;
      child.transform.SetParent(gameObject.transform);
    }
  }

  private void OnCollisionExit(Collision collision)
  {
    if (collision.gameObject.CompareTag("Player"))
    {
      GameObject child = collision.gameObject;
      child.transform.SetParent(null);
    }
  }*/
}
