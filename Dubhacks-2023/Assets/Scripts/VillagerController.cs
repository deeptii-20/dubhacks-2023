using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerController : MonoBehaviour
{
    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    void CheckForBodies()
    {
        if (rend.isVisible)
        {
            Debug.Log(name + "Object is visible from the camera.");
            // is there a body in my vicinity?
        }
    }
}
