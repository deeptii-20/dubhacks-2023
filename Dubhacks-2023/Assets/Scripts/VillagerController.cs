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

    // General villager fields
    private Vector2 facingDirection;

    // Body sighting related fields
    private static float VISION_RADIUS = 5.0f;
    private static float FIELD_OF_VIEW = 80.0f; // from center
    private static string CORPSE_TAG = "Corpse";

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        facingDirection = new Vector2(0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCorpses();
    }

    void CheckForCorpses()
    {
        // if (rend.isVisible) (if on camera)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, VISION_RADIUS);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag(CORPSE_TAG) 
                && Vector2.Angle((collider.transform.position - transform.position), facingDirection) < FIELD_OF_VIEW)
            {
                // check if we're facing 180 degrees
                Debug.Log(name + " Detected: " + collider.gameObject.name);
            }
            Debug.Log(Vector2.Angle((collider.transform.position - transform.position), facingDirection));

        }
    }
}
