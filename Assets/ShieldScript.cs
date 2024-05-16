using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] EnemyHealthManager enemyHealthManager;
    private bool hasShown = false;
    public void ShowUp()
    {
        circleCollider.enabled = true;
        Tween.Color(spriteRenderer, Color.white, 1);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Vector2.Distance(transform.position, other.transform.position) > 7 && other.CompareTag("Knife"))
        {
            other.attachedRigidbody.velocity = -other.attachedRigidbody.velocity;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        circleCollider.enabled = false;
        spriteRenderer.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyHealthManager.currentLives == 1)
        {
            if(!hasShown)
            {
                ShowUp();
                hasShown = true;    
            }
        }
    }
}
