using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAttack : MonoBehaviour
{
    private float attackDuration = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (attackDuration > 0) {
            attackDuration -= Time.deltaTime;
        }
        else {
            this.gameObject.SetActive(false);
        }
    }

    public void SetAttackDuration(float duration) {
        attackDuration = duration;
    }
}
