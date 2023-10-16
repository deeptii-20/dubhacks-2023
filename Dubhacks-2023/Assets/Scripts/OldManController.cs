using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OldManState {
    Intro = 0,
    GhostChecking = 1,
    Angry = 2,
    Win = 3,
}

public class OldManController : MonoBehaviour
{
    public OldManState defaultState = OldManState.Intro;
    public OldManState currState;

    // game state
    public int numVillagers;
    public int totalNumGhosts;
    public int numCapturedGhosts;

    // cemetery
    public Vector2 monumentPos = Vector2.zero; // cemetery center
    public float[] cemeterySize = new float[] {6, 4};   // [width, height]

    // dialogue for talking directly to old man
    public string[] introTalking;   // talk to old man for the first time (no ghosts)
    public string[] introHasGhostsTalking;  // talk to old man for the first time (have ghosts)
    public string[] hasGhostsTalking;   // talk to old man after capturing ghosts
    public string[] noGhostsTalking;  // talk to old man with no captured ghosts
    public string[] noVillagersTalking; // talk to old man when no villagers remain
    public string[] angryTalking;   // talk to old man when he's angry

    // dialogue for releasing ghosts at monument
    public string[] introMonument;  // interact with monument before talking to the old man (have ghosts)
    public string[] introNoGhostsMonument;  // interact with monument before talking to the old man (no ghosts)
    public string[] noGhostsMonument;   // interact with monument with no captured ghosts
    public string[] hasGhostsMonument;  // interact with monument after capturing ghosts
    public string[] noVillagersMonument;    // interact with monument when no villagers remain
    public string[] angryMonument;  // interact with monument when old man is angry
    public string[] winMonument;    // interact with monument after releasing all ghosts


    // Start is called before the first frame update
    void Start()
    {
        currState = defaultState;
        totalNumGhosts = GameObject.FindGameObjectsWithTag("Enemy").Length;
        numCapturedGhosts = 0;
    }

    // Update is called once per frame
    void Update()
    {
        numVillagers = GameObject.FindGameObjectsWithTag("Villager").Length;
    }

    public void PlacePeacefulGhost(GameObject ghost) {
            // pick a random location within cemetary bounds
            Vector2 randPos = new Vector2(
                monumentPos.y + Random.Range(cemeterySize[0] / 2, cemeterySize[0]) * (Random.Range(0.0f, 1.0f) > 0.5f ? -1 : 1), 
                monumentPos.y + Random.Range(cemeterySize[1] / 2, cemeterySize[1]) * (Random.Range(0.0f, 1.0f) > 0.5f ? -1 : 1)
            );
            // move ghost to that location and mark as at rest
            StartCoroutine(ghost.GetComponent<PeacefulGhostController>().MoveToRestPos(randPos));
    }

    public string[] GetTalkingDialogue(bool hasGhosts) {
        if (currState != OldManState.Angry && numVillagers <= 0) {
            return noVillagersTalking;
        }
        switch(currState) {
            case OldManState.Intro:
                currState = OldManState.GhostChecking;
                if (hasGhosts) {
                    return introHasGhostsTalking;
                }
                return introTalking;
            case OldManState.GhostChecking:
                if (hasGhosts) {
                    return hasGhostsTalking;
                }
                return noGhostsTalking;
            case OldManState.Angry:
                return angryTalking;
        }
        return new string[] {"..."};
    }

    public string[] GetMonumentDialogue(bool hasGhosts) {
        if (currState != OldManState.Angry && numVillagers <= 0) {
            return noVillagersMonument;
        }
        switch(currState) {
            case OldManState.Intro:
                currState = OldManState.GhostChecking;
                if (!hasGhosts) {
                    return introNoGhostsMonument;
                }
                introMonument[introMonument.Length - 1] = introMonument[introMonument.Length - 1].Replace("#", "" + (totalNumGhosts - numCapturedGhosts));
                return introMonument;
            case OldManState.GhostChecking:
                if (hasGhosts) {
                    hasGhostsMonument[hasGhostsMonument.Length - 1] = hasGhostsMonument[hasGhostsMonument.Length - 1].Replace("#", "" + (totalNumGhosts - numCapturedGhosts));
                    return hasGhostsMonument;
                }
                return noGhostsMonument;
            case OldManState.Angry:
                return angryMonument;
        }
        return new string[] {"..."};
    }
}
