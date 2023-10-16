using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VillagerState {
    Peaceful = 0,
    Suspicious = 1,
}

public class VillagerController : MonoBehaviour
{
    public string[] normalDialogue;
    public string[] suspiciousDialogue;

    private Renderer rend;
    private Rigidbody2D rb;

    // General villager fields
    private static Color COLOR_WHEN_SUS = Color.red;
    public float MOVE_SPEED = 10.0f;
    private Vector2 facingDirection;
    public VillagerState vstate;

    // Body sighting related fields
    private static float VISION_RADIUS = 5.0f;
    private static float FIELD_OF_VIEW = 80.0f; // from center
    private static LayerMask VISION_OCCLUDE_MASK;
    private static string CORPSE_TAG = "Corpse";
    private static string VILLAGER_TAG = "Villager";

    // Pathfind to old man static fields
    private static float PATH_RADIUS = 10.0f; // max distance to identify what is a "clear" area to walk
    private static LayerMask PATH_BLOCK_MASK; // pathfinding is only blocked by environment
    public float VELOCITY_UPDATE_THRESHOLD = 0.03f;
    private static Vector2[] VELOCITY_DIRECTIONS = new Vector2[]{
        new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1),
        new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, -1)
    };
    // Pathfinding: non-static tunable vars, determined by our pathfinding training area
    public float MAX_RANDOM_WEIGHT = 0.3f; // max weight of the random weight
    public float MAX_ENV_DIST_WEIGHT = 0.8f;
    public float MAX_OLD_MAN_WEIGHT = 0.5f;

    // Private
    private Vector2 currVelocity;

    // Pathfinding: gather old man position
    private static string OLD_MAN_TAG = "OldMan";
    private static Transform OLD_MAN_TRANSFORM;


    // Start is called before the first frame update
    void Start()
    {
        OLD_MAN_TRANSFORM = GameObject.FindWithTag(OLD_MAN_TAG).transform;
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody2D>();
        vstate = VillagerState.Peaceful;

        VISION_OCCLUDE_MASK = LayerMask.GetMask("Moveable", "Environment");
        PATH_BLOCK_MASK = LayerMask.GetMask("Environment");
        facingDirection = new Vector2(0, -1);
        currVelocity = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForSuspiciousActivity();
    }
    private void FixedUpdate()
    {
        if (vstate == VillagerState.Suspicious && Random.Range(0f, 1.0f) < VELOCITY_UPDATE_THRESHOLD)
        {
            UpdateVelocity();
        }
        rb.velocity = currVelocity;
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
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
        rb.gravityScale = 0;
        rb.drag = 100;
        // Call the UpdateVelocity method periodically.
        UpdateVelocity();
    }

    // Updates the direction of walking, in an attempt to pathfind to old man.
    private void UpdateVelocity()
    {        
        // Create the weights corresponding to 8 directions to walk in.
        float[] weights = Enumerable.Range(0, VELOCITY_DIRECTIONS.Length).Select(_ => Random.Range(0.0f, MAX_RANDOM_WEIGHT)).ToArray();

        // Updates weights for each direction:
        for (int i = 0; i < weights.Length; i++)
        {
            Vector2 dir = VELOCITY_DIRECTIONS[i];

            // update weight according to dist from occluding environment
            float envDist = GetDistFromEnv(dir, transform.position, PATH_RADIUS);
            weights[i] += Mathf.Lerp(0.0f, MAX_ENV_DIST_WEIGHT, envDist / PATH_RADIUS);
            //Debug.Log("env dist weight dir " + dir + ": " + Mathf.Lerp(0.0f, MAX_ENV_DIST_WEIGHT, envDist / PATH_RADIUS));

            // update weight according to angle to old man
            float angleToMan = Vector2.Angle(dir, (OLD_MAN_TRANSFORM.position - transform.position));
            weights[i] += Mathf.Lerp(MAX_OLD_MAN_WEIGHT, 0.0f, angleToMan / 180.0f);
            //Debug.Log("old man weight for dir " + dir + ": " + Mathf.Lerp(MAX_OLD_MAN_WEIGHT, 0.0f, angleToMan / 180.0f));
        }

        // Choose the best weight
        int bestDir = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] > weights[bestDir])
            {
                bestDir = i;
            }
        }
        currVelocity = VELOCITY_DIRECTIONS[bestDir].normalized * MOVE_SPEED;
        Debug.Log(rb.velocity);
    }

    // DO NOT CALL ME
    public void InitRandomPathfindingVars()
    {
        MAX_RANDOM_WEIGHT = Random.Range(0f, 1f);
        MAX_ENV_DIST_WEIGHT = Random.Range(0f, 1f);
        MAX_OLD_MAN_WEIGHT = Random.Range(0f, 1f);
    }

    // Returns distance to environment
    private float GetDistFromEnv(Vector2 dir, Vector2 from, float maxRadius)
    {
        RaycastHit2D hit = Physics2D.Raycast(from, dir, maxRadius, PATH_BLOCK_MASK);
        if (hit.collider != null)
        {
            return hit.distance;
        }
        return maxRadius;
    }

    private bool ObjIsSuspicious(Collider2D collider)
    {
        return collider.CompareTag(CORPSE_TAG)
            || (collider.CompareTag(VILLAGER_TAG) && collider.GetComponent<VillagerController>().vstate == VillagerState.Suspicious);
    }

    public string[] GetDialogue() {
        return vstate == VillagerState.Suspicious ? suspiciousDialogue : normalDialogue;
    }
}
