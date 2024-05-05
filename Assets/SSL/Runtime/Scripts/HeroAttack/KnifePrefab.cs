using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifePrefab : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float degats = 1;

    [SerializeField] private float lifetime = 2f;

    // Start is called before the first frame update
    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
}