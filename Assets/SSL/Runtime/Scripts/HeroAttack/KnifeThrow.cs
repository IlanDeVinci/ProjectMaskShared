using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class KnifeThrow : MonoBehaviour
{
    public GameObject projectile;
    private GameObject savedProjectile;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float reloadtime = 0.5f;
    [SerializeField] private float offsetx = 0;
    [SerializeField] private float offsety = 0;

    private bool isReloading = false;
    private float shootTime = 0f;
    private Vector2 launchPos;

    public float orientX = 1;

    public void ThrowKnife(float orientx)
    {
        if (isReloading == false)
        {
            Debug.Log("shooted");

            shootTime = 0f;
            isReloading = true;
            var random = new Random();
            float launchSpeed = speed + random.Next(-2, 2);
            if (orientx > 0)
            {
                launchPos = new Vector2(transform.position.x + offsetx, transform.position.y + offsety);

                savedProjectile = Instantiate(projectile, launchPos, Quaternion.identity);
                savedProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(launchSpeed, 1f);
                savedProjectile.GetComponent<KnifePrefab>().knifeSpeed = 0.1f;
            }
            else
            {
                launchPos = new Vector2(transform.position.x - offsetx, transform.position.y + offsety);

                savedProjectile = Instantiate(projectile, launchPos, Quaternion.identity);
                savedProjectile.GetComponent<Rigidbody2D>().velocity = new Vector2(-launchSpeed, 1f);
                savedProjectile.GetComponent<KnifePrefab>().knifeSpeed = -0.1f;

                savedProjectile.transform.localScale = new Vector3 (-savedProjectile.transform.localScale.x, savedProjectile.transform.localScale.y, savedProjectile.transform.localScale.z);
            }
        }
        else
        {
            Debug.Log("cant shoot");
        }
    }

    // Start is called before the first frame update


    // Update is called once per frame
    private void Update()
    {
        shootTime += Time.deltaTime;
        if (shootTime > reloadtime)
        {
            isReloading = false;
        }
    }
}