using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavPlayerMovement : MonoBehaviour
{
  /*public float speed = 10.0f;
  public float rotationSpeed = 100.0f;
  void Update()
  {
      // Get the horizontal and vertical axis.
      // By default they are mapped to the arrow keys.
      // The value is in the range -1 to 1
      float translation = Input.GetAxis("Vertical") * speed;
      float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

      // Make it move 10 meters per second instead of 10 meters per frame...
      translation *= Time.deltaTime;
      rotation *= Time.deltaTime;

      // Move translation along the object's z-axis
      transform.Translate(0, 0, translation);

      // Rotate around our y-axis
      transform.Rotate(0, rotation, 0);
  }*/

  public delegate void DropHive(Vector3 pos);
  public static event DropHive DroppedHive;

  public float speed = 80.0f;
  public float rotationSpeed = 30.0f;
  Rigidbody rgBody = null;
  float trans = 0;
  float rotate = 0;
  private Animator anim;
  private Camera camera;
  private Transform lookTarget;

  private void Start()
  {
    rgBody = GetComponent<Rigidbody>();
    anim = GetComponentInChildren<Animator>();
    camera = GetComponentInChildren<Camera>();
    lookTarget = GameObject.Find("HeadAimTarget").transform;
  }
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Space))
    {
      DroppedHive?.Invoke(transform.position + (transform.forward * 10));
    }
    // Get the horizontal and vertical axis.
    // By default they are mapped to the arrow keys.
    // The value is in the range -1 to 1
    float translation = Input.GetAxis("Vertical");
    float rotation = Input.GetAxis("Horizontal");

    anim.SetFloat("speed", translation);

    trans += translation;
    rotate += rotation;
  }

  private void FixedUpdate()
  {
    Vector3 rot = transform.rotation.eulerAngles;
    rot.y += rotate * rotationSpeed * Time.deltaTime;
    rgBody.MoveRotation(Quaternion.Euler(rot));
    rotate = 0;

    Vector3 move = transform.forward * trans;
    rgBody.velocity = move * speed * Time.deltaTime;
    trans = 0;
  }

  private void OnCollisionEnter(Collision collision)
  {
    if(collision.collider.CompareTag("Hazard"))
    {
      anim.SetTrigger("died");
      StartCoroutine(ZoomOut());
    }
    else
    {
      anim.SetTrigger("twitchLeftEar");
    }
  }

  IEnumerator ZoomOut()
  {
    const int ITERATIONS = 24;
    for(int i = 0; i < ITERATIONS; i++)
    {
      camera.transform.Translate(camera.transform.forward * -1 * 15.0f/ ITERATIONS);
      yield return new WaitForSeconds(1.0f/ITERATIONS);
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if(other.CompareTag("Hazard"))
    {
      //lookTarget.position = other.transform.position;
      StartCoroutine(LookAndLookAway(lookTarget.position, other.transform.position));
    }
  }

  IEnumerator LookAndLookAway(Vector3 targetPos, Vector3 hazardPos)
  {
    Vector3 targetDir = targetPos - transform.position;
    Vector3 hazardDir = hazardPos - transform.position;

    //float angle = Vector2.SignedAngle(new Vector2(targetPos.x, targetPos.z), new Vector2(hazardPos.x, hazardPos.z));
    float angle = Vector2.SignedAngle(new Vector2(targetDir.x, targetDir.z), new Vector2(hazardDir.x, hazardDir.z));

    const int INTERVALS = 20;
    const float INTERVAL = 0.5f / INTERVALS;
    float angleInterval = angle / INTERVALS;

    for (int i = 0; i < INTERVALS; i++)
    {
      lookTarget.RotateAround(transform.position, Vector3.up, -angleInterval);
      yield return new WaitForSeconds(INTERVAL);
    }
    for (int i = 0; i < INTERVALS; i++)
    {
      lookTarget.RotateAround(transform.position, Vector3.up, angleInterval);
      yield return new WaitForSeconds(INTERVAL);
    }
  }
}
