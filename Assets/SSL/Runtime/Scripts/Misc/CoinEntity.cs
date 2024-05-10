using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinEntity : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int coinValue;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject coinText;
    private bool canGive = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger") ||collision.CompareTag("Player"))
        {
            if (canGive)
            {
                GlobalManager.playerMoney += coinValue;
                particle.Play();
                spriteRenderer.color = Color.clear;
                GameObject savedtext = Instantiate(coinText, transform.position, Quaternion.identity);
                savedtext.GetComponent<CoinTextScript>().value = coinValue;
                canGive = false;
            }
         
            Destroy(circleCollider);
            Destroy(gameObject,0.5f);
        }
    }
}