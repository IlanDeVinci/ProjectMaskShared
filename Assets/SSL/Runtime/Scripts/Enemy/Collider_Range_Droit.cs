using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_Range_Droit : MonoBehaviour
{
    [SerializeField] private Collider2D _colliderDroit;
    public bool IsPlayerInRangeRight = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerTrigger"))
        {
            IsPlayerInRangeRight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerTrigger"))
        {
            IsPlayerInRangeRight = false;
        }
    }
}
