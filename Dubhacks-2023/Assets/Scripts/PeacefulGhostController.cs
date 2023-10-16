using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeacefulGhostController : MonoBehaviour
{
    // folow player
    public GameObject player;
    public float distFromPlayer;
    public float baseSpeed;

    // at rest
    public bool isAtRest;

    // Start is called before the first frame update
    void Start()
    {
        isAtRest = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAtRest) {
            Vector2 target = (Vector2)player.transform.position - player.GetComponent<PlayerController>().facingDirection * distFromPlayer;
            Vector2 moveDir = target - (Vector2) transform.position;
            moveDir.Normalize();
            if (Vector2.Distance(target, (Vector2) transform.position) >= 0.01f) {
                transform.Translate(moveDir * baseSpeed * Time.deltaTime);
            }
        }
    }

    public IEnumerator MoveToRestPos(Vector2 restPos) {
        isAtRest = true;

        Vector2[] possibleDirections = {Vector2.up, Vector2.down, Vector2.left, Vector2.right};
        Vector2 optimalDirection = Vector2.zero;

        float currDistToTarget = Vector2.Distance((Vector2) transform.position, restPos);

        // move to rest pos
        while (currDistToTarget >= 0.5f) {
            currDistToTarget = Vector2.Distance((Vector2) transform.position, restPos);
            foreach (Vector2 direction in possibleDirections) {
                float newDistToTarget = Vector2.Distance((Vector2)transform.position + direction, restPos);
                if (newDistToTarget < currDistToTarget) {
                    currDistToTarget = newDistToTarget;
                    optimalDirection = direction;
                }
            }
            transform.Translate(optimalDirection * Time.deltaTime * baseSpeed / 2.0f);
            yield return null;
        }
        yield return null;
    }
}
