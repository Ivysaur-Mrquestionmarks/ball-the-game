using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
public class Playercontroller : MonoBehaviour
{
    public Rigidbody rb;
    public float movementX;
    public float movementY;
    public float speed = 10;
    public int count; //Quiero probar cosas entonces lo puse public.
    public TextMeshProUGUI countText;
    public TextMeshProUGUI WinText;
    public GameObject winTextObject;
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

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        rb = GetComponent<Rigidbody>();
        winTextObject.SetActive(false);
        //lastGroundedAtTime = Time.time;
        lastGroundedAtTime = Time.time;
        Startingpos = rb.position;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        Vector3 moveX = camara.transform.right * movementVector.x * speed;
        Vector3 moveZ = camara.transform.forward * movementVector.y * speed;
        Vector3 moveDirection = moveX +  moveZ;
        movementX = moveDirection.x;
        movementY = moveDirection.z;
    }

    public void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement*speed);
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
            rb.velocity = new Vector3(rb.velocity.x, (rb.velocity.y >= 0 ? rb.velocity.y:0), rb.velocity.z);
            //rb.AddForce(new Vector3(0.0f, jumpSpeed, 0.0f));
            doubleJ = false;
        }
        //}

        //if (Keyboard.current.spaceKey.isPressed && Time.time < lastGroundedAtTime + jumpHoldDuration)
        //{

        //Lo que nos dijiste de hacer que el salto dure más entre más se unda el boton
        //rb.AddForce(new Vector3(0.0f, jumpSpeed * (Keyboard.current.spaceKey.isPressed && Time.time < lastGroundedAtTime + jumpHoldDuration ? 1 : 0), 0.0f));
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y+ jumpSpeed * (Keyboard.current.spaceKey.isPressed && Time.time < lastGroundedAtTime + jumpHoldDuration ? 1 : 0), rb.velocity.z);
        
    }

    void OnTriggerEnter(Collider other)
    {
        //grounded = other.gameObject.CompareTag("Ground");

        count = count + (other.gameObject.CompareTag("PickUp")?1:0);
        other.gameObject.SetActive(!other.gameObject.CompareTag("PickUp"));  
        winTextObject.SetActive(other.gameObject.CompareTag("Goal"));
        WinText.text = "You won!!!\nYour score was " + count.ToString() + "\nYour time was: "+ Time.time +"\nSong used Game 3 loop thing, by Rig\nLink in the read me file.";
        if (other.gameObject.CompareTag("Level")) {
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
        if (Input.GetButtonDown("Fire3")) { //rb position y velocity son raros, entonces no los puse branchless.
            rb.velocity = new Vector3(0.0f, -drop, 0.0f); //For making it branchless we would need to call the comparison multipletimes, which would make it inefficient.
        }
        else if (rb.position.y < deadLayer) { //misma cosa para esto.
            rb.position = Startingpos;  
            rb.velocity = new Vector3(0, 0, 0); 
        }
        countText.text = "Score: " + count.ToString()+"\nTime: " + Time.time;
    }
}
