using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OldManState {
    Intro = 0,
    GhostChecking = 1,
    Angry = 2,
}

public class OldManController : MonoBehaviour
{
    public OldManState defaultState = OldManState.Intro;
    public OldManState currState;

    // game state
    public int numVillagers;
    public int numGhosts;

    // Start is called before the first frame update
    void Start()
    {
        currState = defaultState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string[] GetTalkingDialogue(bool hasGhosts) {
        return new string[] {"talking..."};
    }

    public string[] GetMonumentDialogue() {
        return new string[] {"monument..."};
    }

    // NORMAL TALKING TO OLD MAN
    // intro -> "go find ghosts" -> ghost checking
    // ghost checking -> has captured -> "why don't you release them at the monument to put them to rest"
    // ghost checking -> no captured -> small talk -> "go find ghosts"
    // ghost checking -> no villagers -> "there's no village left to protect" -> game over (lose)
    // angry -> "you dare show your face after murdering one of our own???" -> game over (lose)

    // INTERACT WITH MONUMENT
    // intro -> "you're eager aren't you" -> "there are _ more ghosts to pacify" -> ghost checking
    // ghost checking -> "thanks for releasing their spirits" -> "there are _ more ghosts to pacify"
    // angry -> "releasing their ghosts isn't enough to earn forgiveness for their murders" -> game over (lose)
}
