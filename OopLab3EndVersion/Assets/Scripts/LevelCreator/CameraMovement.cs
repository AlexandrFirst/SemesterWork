using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    float x = 0;
    float y = 0;
    float z = -10;
    public float speed = 2f;

    

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Camera>().orthographic = true;
    }

    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && (Input.GetAxis("Mouse ScrollWheel") + GetComponent<Camera>().orthographicSize) > 5)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) GetComponent<Camera>().orthographicSize--;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && (Input.GetAxis("Mouse ScrollWheel") + GetComponent<Camera>().orthographicSize) < 10)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) GetComponent<Camera>().orthographicSize++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Transform>().position = new Vector3(x, y, z);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            y += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            y -= speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            x -= speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            x+=speed*Time.deltaTime;
        }

       
        Zoom();

    }
}
