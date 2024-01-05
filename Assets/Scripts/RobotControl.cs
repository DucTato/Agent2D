using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotControl : MonoBehaviour
{
    public LineRenderer line;
    public GameObject gantry, magnet, currentHold;
    public Collider2D bar;
    public float timer, magnetSpeed, gantrySpeed;
    public bool magnetDown, magnetUp, moveGantry;
    public Vector3 target;
    private float timerCount;
    public HashSet<GameObject> redbox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, gantry.transform.position);
        line.SetPosition(1, magnet.transform.position);
        if (timerCount < 0)
        {
            timerCount = timer;
            scanEnvironment();
        }
        else
        {
            timerCount -= Time.deltaTime;
        }
        if (magnetDown)
        {
            magnet.transform.localPosition += Vector3.down * Time.deltaTime * magnetSpeed;
        }
        if (magnetUp && magnet.transform.localPosition.y <= -1.2f)
        {
            magnet.transform.localPosition += Vector3.up * Time.deltaTime * magnetSpeed;
        }
        else
        {
            magnetUp = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            magnetOFF(currentHold);
        }
        if (moveGantry)
        {
            Vector3 direction = gantry.transform.position;
            direction.x = Mathf.MoveTowards(direction.x, target.x, Time.deltaTime*gantrySpeed);
            gantry.transform.position = direction;
            if (gantry.transform.position.x == target.x)
            {
                moveGantry = false;
            }
        }
        if (gantry.transform.position.x > bar.bounds.max.x)
        {
            Vector3 position = gantry.transform.position;
            position.x = bar.bounds.max.x;
            gantry.transform.position = position;
            moveGantry= false;
        }
        if (gantry.transform.position.x < bar.bounds.min.x)
        {
            Vector3 position = gantry.transform.position;
            position.x = bar.bounds.min.x;
            gantry.transform.position = position;
            moveGantry= false;
        }
    }
    private void magnetON(GameObject obj)
    {
        obj.transform.parent = magnet.transform;
        obj.GetComponent<Rigidbody2D>().isKinematic = true;
        currentHold = obj;
    }
    private void magnetOFF(GameObject obj)
    {
        obj.transform.parent = null;
        obj.GetComponent<Rigidbody2D>().isKinematic = false;
        currentHold = null;
    }
    private void scanEnvironment()
    {
        redbox = GameObject.FindGameObjectsWithTag("RedBox").ToHashSet();
        Debug.Log("RedBox: " + redbox.Count);
    }
    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("RedBox"))
        {
            Debug.Log("Collision");
            magnetDown = false;
            magnetON(other.gameObject);
            magnetUp = true;
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            magnetDown = false;
            magnetUp = true;
        }
    }
}
