using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPointScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private Sprite[] checkpoints;
    [SerializeField] private bool hasBeenHitBefore = false;
    private void Awake()
    {
        if (hasBeenHitBefore)
        {
            image.sprite = checkpoints[1];
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger") || collision.CompareTag("Player"))
        {
            GlobalManager.playerCheckpointPosition = new Vector2(transform.position.x, transform.position.y + 0.5f);
            image.sprite = checkpoints[1];
            hasBeenHitBefore = true;
        }
    }
}
