using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 direction;
    public float attackDamage;
    public float baseSpeed = 20;
    public float maxDist = 5;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float currDist = Vector2.Distance(transform.position, startPos);
        if (currDist >= maxDist) {
            Destroy(this.gameObject);
        }
        transform.Translate(direction * baseSpeed * Time.deltaTime);
    }

    public void SetParams(Vector2 startPos, Vector2 direction, float attackDamage) {
        this.startPos = startPos;
        this.direction = direction;
        this.attackDamage = attackDamage;
    }

    public void Explode() {
        Destroy(this.gameObject);
    }
}
