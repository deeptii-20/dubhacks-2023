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

    // General villager fields
    private Vector2 facingDirection;
    protected VillagerState vstate;

    // Body sighting related fields
    private static float VISION_RADIUS = 5.0f;
    private static float FIELD_OF_VIEW = 80.0f; // from center
    private static LayerMask VISION_OCCLUDE_MASK;
    private static string CORPSE_TAG = "Corpse";
    private static string VILLAGER_TAG = "Villager";

    // Start is called before the first frame update
    void Start()
    {
        vstate = VillagerState.Peaceful;

        VISION_OCCLUDE_MASK = LayerMask.GetMask("Moveable", "Environment");
        facingDirection = new Vector2(0, -1);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForSuspiciousActivity();
    }

    // Updates villager to be suspicious, if needed.
    void CheckForSuspiciousActivity()
    {
        if(vstate == VillagerState.Suspicious)
        {
            return;
        }

        // if (rend.isVisible) (if on camera)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, VISION_RADIUS);
        foreach (Collider2D collider in colliders)
        {
            // Check if suspicious object is in field of view.
            if (Vector2.Angle((collider.transform.position - transform.position), facingDirection) < FIELD_OF_VIEW
                && IsSuspicious(collider))
            {
                // Check if suspicious object is occluded.
                Vector2 origin = transform.position;
                // Cast a ray in the facing direction.
                RaycastHit2D hit = Physics2D.Raycast(origin, facingDirection, VISION_RADIUS, VISION_OCCLUDE_MASK);
                // Check if the closest occluding object is the sus object.
                if (hit.collider == collider)
                {
                    continue;
                }

                // We found a sus object!
                Debug.Log(name + " Detected: " + collider.gameObject.name);
                vstate = VillagerState.Suspicious;
                break;
            }
        }
    }

    private bool IsSuspicious(Collider2D collider)
    {
        return collider.CompareTag(CORPSE_TAG)
            || (collider.CompareTag(VILLAGER_TAG) && collider.GetComponent<VillagerController>().vstate == VillagerState.Suspicious);
    }
}
