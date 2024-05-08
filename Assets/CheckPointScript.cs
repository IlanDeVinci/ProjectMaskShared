using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SpriteRenderer image;
    private static bool hasBeenHitBefore = false;

    private void Awake()
    {
        if (hasBeenHitBefore)
        {
            image.color = Color.green;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger") || collision.CompareTag("Player"))
        {
            GlobalManager.playerCheckpointPosition = transform.position;
            image.color = Color.green;
            hasBeenHitBefore = true;
        }
    }
}
