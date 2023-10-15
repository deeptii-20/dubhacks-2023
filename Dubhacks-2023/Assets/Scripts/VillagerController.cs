using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VillagerState {
    Peaceful = 0,
    Suspicious = 1,
}

public class VillagerController : MonoBehaviour
{
    public string normalDialogue;
    public string suspiciousDialogue;

    private Renderer rend;

    // General villager fields
    private static Color COLOR_WHEN_SUS = Color.red;
    private Vector2 facingDirection;
    public VillagerState vstate;

    // Body sighting related fields
    private static float VISION_RADIUS = 5.0f;
    private static float FIELD_OF_VIEW = 80.0f; // from center
    private static LayerMask VISION_OCCLUDE_MASK;
    private static string CORPSE_TAG = "Corpse";
    private static string VILLAGER_TAG = "Villager";

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
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
                && ObjIsSuspicious(collider))
            {
                // Check if suspicious object is occluded.
                Vector2 origin = transform.position;
                Vector2 toSusObj = new Vector2(collider.transform.position.x - origin.x, collider.transform.position.y - origin.y);
                // Cast a ray to the body.
                RaycastHit2D hit = Physics2D.Raycast(origin, toSusObj, VISION_RADIUS, VISION_OCCLUDE_MASK);
                
                // Check if the occluding object is closer than the sus object.
                if (hit.collider != null && Vector2.Distance(origin, hit.collider.transform.position) < toSusObj.magnitude)
                {
                    // Debug.Log(name + " saw " + hit.collider.name + " occluded sus object " + collider.name);
                    // if occluding object is closer than the sus object, ignore the object
                    continue;
                }

                // We found a sus object!
                // Debug.Log(name + " Detected: " + collider.name);
                BecomeSuspicious();
                break;
            }
        }
    }

    // Make this villager suspicious.
    private void BecomeSuspicious()
    {
        vstate = VillagerState.Suspicious;
        rend.material.color = COLOR_WHEN_SUS;
    }

    private bool ObjIsSuspicious(Collider2D collider)
    {
        return collider.CompareTag(CORPSE_TAG)
            || (collider.CompareTag(VILLAGER_TAG) && collider.GetComponent<VillagerController>().vstate == VillagerState.Suspicious);
    }

    public string GetDialogue() {
        return vstate == VillagerState.Suspicious ? suspiciousDialogue : normalDialogue;
    }
}
