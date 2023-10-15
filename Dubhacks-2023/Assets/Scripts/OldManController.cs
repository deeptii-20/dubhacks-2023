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
    private OldManState currState;

    // Start is called before the first frame update
    void Start()
    {
        currState = defaultState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Villager" && collision.gameObject.GetComponent<VillagerController>().vstate == VillagerState.Suspicious) {
            currState = OldManState.Angry;
        }
    }
}
