using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("EnemyBullet") && !collision.CompareTag("CameraTrigger"))
        {
            if (collision.CompareTag("PlayerTrigger"))
            {
                collision.gameObject.GetComponent<HealthManager>().TakeDamage(5);
            }
            Destroy(this.gameObject, 0.01f);

        }

    }
}
