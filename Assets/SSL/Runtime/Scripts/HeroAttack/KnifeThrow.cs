using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeThrow : MonoBehaviour
{
    public GameObject projectile;
    private GameObject savedProjectile;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float reloadtime = 0.5f;
    [SerializeField] private float offsetx = 0;
    [SerializeField] private float offsety = 0;

    private bool isReloading;
    private float shootTime = 0;
    private Vector2 launchPos;

    public float orientX = 1;

    public void ThrowKnife(float orientx)
    {
        if (!isReloading)
        {
            shootTime = Time.time;
            isReloading = true;
            if (orientx > 0)
            {
                launchPos = new Vector2(transform.position.x + offsetx, transform.position.y + offsety);

                savedProjectile = Instantiate(projectile, launchPos, Quaternion.identity);
                savedProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
            }
            else
            {
                launchPos = new Vector2(transform.position.x - offsetx, transform.position.y + offsety);

                savedProjectile = Instantiate(projectile, launchPos, Quaternion.identity);
                savedProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
                savedProjectile.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (shootTime + reloadtime < Time.time)
        {
            isReloading = false;
        }
    }
}