using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashController : MonoBehaviour
{
    public float attackDamage;
    public float baseDuration;
    private float currDuration;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currDuration >= baseDuration) {
            Destroy(this.gameObject);
        }
        currDuration += Time.deltaTime;
    }

    public void SetParams(float attackDamage, float duration) {
        this.attackDamage = attackDamage;
        this.baseDuration = duration;
    }
}
