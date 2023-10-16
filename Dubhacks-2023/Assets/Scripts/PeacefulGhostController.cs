using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeacefulGhostController : MonoBehaviour
{
    public GameObject player;
    public float distFromPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = (Vector2)player.transform.position - player.GetComponent<PlayerController>().facingDirection * distFromPlayer;
    }
}
