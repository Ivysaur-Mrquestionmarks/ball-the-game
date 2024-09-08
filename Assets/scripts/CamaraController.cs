using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CamaraController : MonoBehaviour
{


    public GameObject player;
    public Vector3 offset;
    public int sensitivity;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
        Vector3 offsetR =  player.transform.position + transform.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        transform.position = player.transform.position + offset;

        offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity, Vector3.up) * Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * sensitivity, -Vector3.right) * offset * (Input.GetMouseButton(0) ? 1 : 0) + offset * (Input.GetMouseButton(0) ? 0 : 1);
        transform.LookAt(player.transform.position);
    }
    private void Update()
    {

    }
}
