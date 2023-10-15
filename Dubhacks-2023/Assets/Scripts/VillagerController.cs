using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VillagerState {
    Peaceful = 0,
    Suspicious = 1,
}

public class VillagerController : MonoBehaviour
{
    public string dialogue;
    public VillagerState currState;

    private Renderer rend;

    // Body sighting related fields
    private static float VIEW_RADIUS = 5.0f;

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
