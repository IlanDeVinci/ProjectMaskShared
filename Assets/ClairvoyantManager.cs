using PrimeTween;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ClairvoyantManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> clairvoyantObjects;
    [SerializeField] public bool isClairvoyant => GlobalManager.isPlayerClairvoyant;
    // Start is called before the first frame update
    private Tween visibilitytween;
    private float alpha = 0.0f;
    private bool hasChangedVisibility = false;
    void Start()
    {
        foreach (Transform child in transform)
        {
            clairvoyantObjects.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalManager.isPlayerClairvoyant)
        {
            if (hasChangedVisibility)
            {
                visibilitytween.Stop();
                visibilitytween = Tween.Custom(cycles: 1, startValue: alpha, endValue: 0, duration: 1, ease: Ease.InOutSine, onValueChange: val => alpha = val);
                hasChangedVisibility = false;
            }
            if (!visibilitytween.isAlive)
            {
                visibilitytween.Stop();
                visibilitytween = Tween.Custom(cycles: 2, cycleMode: CycleMode.Yoyo, startValue: alpha, endValue: 0.005f, duration: 1, ease: Ease.InOutElastic, onValueChange: val => alpha = val);

            }

        }
        else
        {
            if (!hasChangedVisibility)
            {
                hasChangedVisibility = true;
                visibilitytween.Stop();
                visibilitytween = Tween.Custom(cycles: 1, startValue: alpha, endValue: 1, duration: 1, ease: Ease.InOutSine, onValueChange: val => alpha = val);
            }
        }

        foreach (GameObject obj in clairvoyantObjects)
        {
            if (obj != null)
            {
                if (obj.GetComponentInChildren<Tilemap>() != null)
                {
                    Tilemap tilemap = obj.GetComponentInChildren<Tilemap>();
                    tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, alpha);
                    obj.GetComponentInChildren<TilemapCollider2D>().enabled = true;
                    if (GlobalManager.isPlayerClairvoyant) obj.GetComponentInChildren<TilemapCollider2D>().enabled = false;
                }



                if (obj.GetComponent<SpriteRenderer>() != null)
                {
                    SpriteRenderer spriterenderer = obj.GetComponent<SpriteRenderer>();
                    spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, alpha);
                }
                if (obj.GetComponentInChildren<SpriteRenderer>() != null)
                {
                    SpriteRenderer spriterenderer = obj.GetComponentInChildren<SpriteRenderer>();

                    spriterenderer.color = new Color(spriterenderer.color.r, spriterenderer.color.g, spriterenderer.color.b, alpha);
                }
                if (obj.GetComponentInChildren<CanvasGroup>() != null)
                {
                    CanvasGroup[] canvasGroups = obj.GetComponentsInChildren<CanvasGroup>();
                    foreach (CanvasGroup canvasgroup in canvasGroups)
                    {
                        canvasgroup.alpha = alpha;
                    }
                }

                if (obj.GetComponent<Collider2D>() != null && obj.GetComponent<Rigidbody2D>() == null)
                {
                    if (GlobalManager.isPlayerClairvoyant)
                    {
                        obj.GetComponent<Collider2D>().enabled = true;
                    }
                    else
                    {
                        obj.GetComponent<Collider2D>().enabled = false;
                    }
                }
            }
        }
    }
}
