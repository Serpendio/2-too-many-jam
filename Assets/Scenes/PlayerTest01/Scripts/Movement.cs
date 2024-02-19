using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float maxSpeed = 3.4f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello world!"); // I just wanted to see how scripts are loaded :)
    }

    // Update is called once per frame
    void Update()
    {
        // Movement controls
        if (Input.GetKey(KeyCode.A))
        {
            // Move Left
            
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Move Right

        }
        if (Input.GetKey(KeyCode.W))
        {
            // Move Up

        }
        else if (Input.GetKey(KeyCode.S))
        {
            // Move Down

        }
    }
}
