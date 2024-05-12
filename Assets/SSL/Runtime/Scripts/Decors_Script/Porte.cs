using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porte : MonoBehaviour
{

    [SerializeField] private Collider2D _colliderDetec;
    [SerializeField] private bool _playerDetected = false;

    void FixedUpdate()
    {
        if (_playerDetected)
        {
            Debug.Log("Player detected Porte fermée");
            _colliderDetec.enabled = true;
        }
        else
        {
            Debug.Log("Player not detected Porte ouverte");
            _colliderDetec.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerTrigger"))
        {
            _playerDetected = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerTrigger"))
        {
            _playerDetected = false;
        }
    }
    
}
