using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class player2 : MonoBehaviour
{
    public Rigidbody rb;
    public float movementX;
    public float movementY;
    public float speed = 10;
    public int count; //Quiero probar cosas entonces lo puse public.
    public TextMeshProUGUI countText;
    public GameObject camara;
    public float jumpSpeed;
    public bool doubleJ = true;
    public float drop;
    public float lastGroundedAtTime;
    public float jumpHoldDuration;
    public bool wallJ = true;
    public float WJduration;
    public Vector3 BVelocity;
    public Vector3 wallSpeed;
    public Vector3 Startingpos;
    public float deadLayer;
    public string nextLevel;
    public float escTimer;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        rb = GetComponent<Rigidbody>();
        //lastGroundedAtTime = Time.time;
        lastGroundedAtTime = Time.time;
        Startingpos = rb.position;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        Vector3 moveX = camara.transform.right * movementVector.x;
        Vector3 moveZ = camara.transform.forward * movementVector.y ;
        Vector3 moveDirection = Vector3.Normalize(moveX + moveZ) * speed;
        movementX = moveDirection.x;
        movementY = moveDirection.z;
    }

    public void FixedUpdate()
    {
        //Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.velocity = new Vector3(movementX+rb.velocity.x, rb.velocity.y, movementY+rb.velocity.z);
        //rb.AddForce(movement * speed);
        if (Physics.Raycast(transform.position, -Vector3.up, 1f)) //Normal jump.
        {
            lastGroundedAtTime = Time.time;

            //rb.AddForce(new Vector3(0.0f, jumpSpeed, 0.0f));
            doubleJ = true;
        }
        else if (Input.GetButtonDown("Jump") && wallJ) //Wall jump.
        {
            lastGroundedAtTime = Time.time + WJduration;
            wallJ = false;
            //rb.velocity = new Vector3(20 * wallSpeed.x, rb.velocity.y * 1.1f, 20 * wallSpeed.z);
            rb.velocity = new Vector3(20 * wallSpeed.x, 0.0f, 20 * wallSpeed.z);
        }
        else if (Input.GetButtonDown("Jump") && doubleJ) //Double jump (after wall jump in the if chain so you can still wall jump and then double jump)
        {
            lastGroundedAtTime = Time.time;
            rb.velocity = new Vector3(rb.velocity.x, (rb.velocity.y >= 0 ? rb.velocity.y : 0), rb.velocity.z);
            //rb.AddForce(new Vector3(0.0f, jumpSpeed, 0.0f));
            doubleJ = false;
        }
        //}

        //if (Keyboard.current.spaceKey.isPressed && Time.time < lastGroundedAtTime + jumpHoldDuration)
        //{

        //Lo que nos dijiste de hacer que el salto dure más entre más se unda el boton
        //rb.AddForce(new Vector3(0.0f, jumpSpeed * (Keyboard.current.spaceKey.isPressed && Time.time < lastGroundedAtTime + jumpHoldDuration ? 1 : 0), 0.0f));
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + jumpSpeed * (Keyboard.current.spaceKey.isPressed && Time.time < lastGroundedAtTime + jumpHoldDuration ? 1 : 0), rb.velocity.z);

    }

    void OnTriggerEnter(Collider other)
    {
        //grounded = other.gameObject.CompareTag("Ground");

        count = count + (other.gameObject.CompareTag("PickUp") ? 1 : 0);
        other.gameObject.SetActive(!other.gameObject.CompareTag("PickUp"));
        if (other.gameObject.CompareTag("Level"))
        {
            string destination = Application.persistentDataPath + nextLevel+".ball";
            FileStream file;
            float Rtime = Time.timeSinceLevelLoad;
            float Btime = Rtime;
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists(destination))
            {
                file = File.OpenRead(destination);

                levelRecords Records = (levelRecords)bf.Deserialize(file);
                bool change = Btime > Records.BestTime;
                Btime = Records.BestTime * (change ? 1 : 0) + Btime * (change ? 0 : 1);
                change = count <= Records.BestScore;             
                Rtime = Records.BestTimeScore * (change ? 1 : 0) + Rtime * (change ? 0 : 1);
                if (count == Records.BestScore)
                {
                    change = Rtime > Records.BestTimeScore;
                    Rtime = Records.BestTimeScore * (change ? 1 : 0) + Rtime * (change ? 0 : 1);
                }
                count = Records.BestScore * (change ? 1 : 0) + count * (change ? 0 : 1);

                file.Close();
            }
            else {
                file = File.Create(destination);
                file.Close();
            }
            file = File.OpenWrite(destination);
            levelRecords Records_ = new levelRecords(Btime,Rtime,count);
            bf.Serialize(file, Records_);
            file.Close();


            //end of record
            SceneManager.LoadScene(nextLevel);

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        wallJ = !Physics.Raycast(transform.position, -Vector3.up, 1f);
        wallSpeed = collision.contacts[0].normal;
        //if (Keyboard.current.rightShiftKey.isPressed) {
        //rb.velocity = BVelocity.magnitude * Vector3.Reflect(BVelocity.normalized, collision.contacts[0].normal);
        bool bounce = Keyboard.current.rightShiftKey.isPressed;
        //
        rb.velocity = (BVelocity.magnitude * Vector3.Reflect(BVelocity.normalized, collision.contacts[0].normal)) * (bounce ? 1 : 0) + rb.velocity * (bounce ? 0 : 1);
        //}

    }
    void OnCollisionExit(Collision other)
    {
        wallJ = false;
    }

    // Update is called once per frame
    void Update()
    {
        BVelocity = rb.velocity;
        //if (Input.GetButtonDown("Jump"))
        //{

        //}
        escTimer = escTimer + (Keyboard.current.escapeKey.isPressed ? 1 : 0);
        if (Input.GetButtonDown("Fire3"))
        { //rb position y velocity son raros, entonces no los puse branchless.
            rb.velocity = new Vector3(0.0f, -drop, 0.0f); //For making it branchless we would need to call the comparison multipletimes, which would make it inefficient.
        }
        else if (rb.position.y < deadLayer)
        { //misma cosa para esto.
            rb.position = Startingpos;
            rb.velocity = new Vector3(0, 0, 0);
        }
        
        else if (escTimer>120) {
            SceneManager.LoadScene("mainmenu");
        }
        countText.text = "Score: " + count.ToString() + "\nTime: " + Time.timeSinceLevelLoad;
    }
}

