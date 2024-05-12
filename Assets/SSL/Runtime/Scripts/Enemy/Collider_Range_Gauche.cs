using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_Range_Gauche : MonoBehaviour
{
    [SerializeField] private Collider2D _colliderGauche;
    public bool IsPlayerInRangeLeft = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerTrigger"))
        {
            IsPlayerInRangeLeft = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerTrigger"))
        {
            IsPlayerInRangeLeft = false;
        }
    }
}
