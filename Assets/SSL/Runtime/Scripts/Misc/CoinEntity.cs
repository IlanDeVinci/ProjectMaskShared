using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinEntity : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private int coinValue;
    private bool canGive = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerTrigger") ||collision.CompareTag("Player"))
        {
            if (canGive)
            {
                GlobalManager.playerMoney += coinValue;
            }
            canGive = false;
            Destroy(gameObject);
        }
    }
}