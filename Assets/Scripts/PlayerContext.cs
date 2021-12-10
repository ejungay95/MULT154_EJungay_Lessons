using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public abstract class PlayerState
{
  protected NetworkBehaviour thisObject;
  protected string stateName;
  protected GameObject player;
  protected PlayerState(NetworkBehaviour thisObj)
  {
    thisObject = thisObj;
    player = thisObject.gameObject;
  }

  public abstract void Start();
  public abstract void Update();
  public abstract void FixedUpdate();
  public abstract void OnTriggerExit(Collider other);
  public abstract void OnTriggerEnter(Collider other);
  public abstract void OnCollisionEnter(Collision collision);
}

public class RiverState : PlayerState
{
  private Rigidbody rb;
  private Vector3 direction = Vector3.zero;
  public GameObject[] spawnPoints = null;
  public float speed = 10.0f;
  public float zLimit = 40;

  public RiverState(NetworkBehaviour thisObj) : base(thisObj)
  {
    stateName = "RiverLevel";
    GameData.gamePlayStart = Time.time;
  }

  public override void Start()
  {
    rb = player.GetComponent<Rigidbody>();

    spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
  }

  public override void Update()
  {


    float horzMove = Input.GetAxis("Horizontal");
    float vertMove = Input.GetAxis("Vertical");

    direction = new Vector3(horzMove, 0, vertMove);
  }

  /*private void OnDrawGizmos()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawRay(player.transform.position, direction * 10);
    Gizmos.color = Color.magenta;
    Gizmos.DrawRay(player.transform.position, rb.velocity * 5);
    Gizmos.color = Color.blue;
    Gizmos.DrawCube(player.transform.position, new Vector3(10f, 10f, 10f));
  }*/

  // Update is called once per frame
  public override void FixedUpdate()
  {
    // Note to self -- I uncommented this line and commented line 50 to 59 to answer question 3 for the rigidbody
    // and for question 4 to see the magenta ray
    rb.AddForce(direction * speed, ForceMode.Force);

    //*rb.MovePosition(transform.position + direction * speed * Time.deltaTime);

    if (player.transform.position.z > zLimit)
    {
      player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, zLimit);
    } else if (player.transform.position.z < -zLimit)
    {
      player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -zLimit);
    }
  }

  private void Respawn()
  {
    int index = 0;
    while (Physics.CheckBox(spawnPoints[index].transform.position, new Vector3(1.5f, 1.5f, 1.5f)))
    {
      index++;
    }
    rb.MovePosition(spawnPoints[index].transform.position);
    rb.velocity = Vector3.zero;
  }

  public override void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Hazard"))
    {
      Respawn();
    }
  }

  public override void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Exit"))
    {
      NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
      networkManager.ServerChangeScene("ForestLevel");
    }
  }

  public override void OnCollisionEnter(Collision collision)
  {
    //throw new System.NotImplementedException();
  }
}

public class ForestState : PlayerState
{
  public delegate void DropHive(Vector3 pos);
  public static event DropHive DroppedHive;

  public float speed = 10.0f;
  public float rotationSpeed = 30.0f;
  Rigidbody rgBody = null;
  float trans = 0;
  float rotate = 0;
  private Animator anim;
  private Camera camera;
  private Transform lookTarget;
  CapsuleCollider capsule = null;

  public ForestState(NetworkBehaviour thisObj) : base(thisObj)
  {
    stateName = "ForestLevel";
  }

  
  public override void Start()
  {
    player.transform.position = new Vector3(-20f, 0.5f, -10f);

    Transform rabbit = player.transform.Find("Rabbit");
    rabbit.transform.localEulerAngles = Vector3.zero;
    rabbit.transform.localScale = Vector3.one;

    capsule = player.GetComponent<CapsuleCollider>();
    capsule.radius = .5f;
    capsule.center = new Vector3(0, .5f, .1f);

    rgBody = player.GetComponent<Rigidbody>();
    anim = player.GetComponentInChildren<Animator>();
    camera = player.GetComponentInChildren<Camera>();
    camera.enabled = true;
    lookTarget = GameObject.Find("HeadAimTarget").transform;
  }
  public override void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      DroppedHive?.Invoke(player.transform.position + (player.transform.forward * 10));
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

  public override void FixedUpdate()
  {
    Vector3 rot = player.transform.rotation.eulerAngles;
    rot.y += rotate * rotationSpeed * Time.deltaTime;
    rgBody.MoveRotation(Quaternion.Euler(rot));
    rotate = 0;

    Vector3 move = player.transform.forward * trans * speed;
    move.y = rgBody.velocity.y;
    rgBody.velocity = move;// * Time.deltaTime;

    trans = 0;
  }

  public override void OnCollisionEnter(Collision collision)
  {
    if (collision.collider.CompareTag("Hazard"))
    {
      anim.SetTrigger("died");
      thisObject.StartCoroutine(ZoomOut());
    } else
    {
      anim.SetTrigger("twitchLeftEar");
    }
    
  }

  IEnumerator ZoomOut()
  {
    const int ITERATIONS = 24;
    for (int i = 0; i < ITERATIONS; i++)
    {
      camera.transform.Translate(camera.transform.forward * -1 * 15.0f / ITERATIONS);
      yield return new WaitForSeconds(1.0f / ITERATIONS);
    }
  }

  public override void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Hazard"))
    {
      //lookTarget.position = other.transform.position;
      thisObject.StartCoroutine(LookAndLookAway(lookTarget.position, other.transform.position));
    }
    if (other.CompareTag("Exit"))
    {
      NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
      networkManager.ServerChangeScene("EndScene");
    }
  }

  IEnumerator LookAndLookAway(Vector3 targetPos, Vector3 hazardPos)
  {
    Vector3 targetDir = targetPos - player.transform.position;
    Vector3 hazardDir = hazardPos - player.transform.position;

    //float angle = Vector2.SignedAngle(new Vector2(targetPos.x, targetPos.z), new Vector2(hazardPos.x, hazardPos.z));
    float angle = Vector2.SignedAngle(new Vector2(targetDir.x, targetDir.z), new Vector2(hazardDir.x, hazardDir.z));

    const int INTERVALS = 20;
    const float INTERVAL = 0.5f / INTERVALS;
    float angleInterval = angle / INTERVALS;

    for (int i = 0; i < INTERVALS; i++)
    {
      lookTarget.RotateAround(player.transform.position, Vector3.up, -angleInterval);
      yield return new WaitForSeconds(INTERVAL);
    }
    for (int i = 0; i < INTERVALS; i++)
    {
      lookTarget.RotateAround(player.transform.position, Vector3.up, angleInterval);
      yield return new WaitForSeconds(INTERVAL);
    }
  }

  public override void OnTriggerExit(Collider other)
  {
    //throw new System.NotImplementedException();
  }
}

public class PlayerContext : NetworkBehaviour
{
  PlayerState currentState;

  // Start is called before the first frame update
  void Start()
  {
    if (!isLocalPlayer) return;

    if(SceneManager.GetActiveScene().name == "RiverLevel")
    {
      currentState = new RiverState(this);
    }
    else if (SceneManager.GetActiveScene().name == "ForestLevel")
    {
      currentState = new ForestState(this);
    } else
    {
      this.gameObject.SetActive(false);
    }

    if(currentState != null)
    {
      currentState.Start();
    } 
  }

  // Update is called once per frame
  void Update()
  {
    if (!isLocalPlayer) return;

    currentState.Update();
  }

  private void FixedUpdate()
  {
    if (!isLocalPlayer) return;

    currentState.FixedUpdate();
  }

  private void OnTriggerExit(Collider other)
  {
    if (!isLocalPlayer) return;

    currentState.OnTriggerExit(other);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!isLocalPlayer) return;

    currentState.OnTriggerEnter(other);
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!isLocalPlayer) return;

    currentState.OnCollisionEnter(collision);
  }
}
