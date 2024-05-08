using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClairvoyantManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> clairvoyantObjects;
    [SerializeField] public bool isClairvoyant => GlobalManager.isPlayerClairvoyant;
    // Start is called before the first frame update
    private Tween visibilitytween;
    private float alpha = 0.0f;
    private bool hasChangedVisibility = false;
    void Start()
    {
        foreach(Transform child in transform)
        {
            clairvoyantObjects.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!GlobalManager.isPlayerClairvoyant)
        {
            if(hasChangedVisibility)
            {
                visibilitytween.Stop();
                visibilitytween = Tween.Custom(cycles: 1, startValue: alpha, endValue: 0, duration: 1, ease: Ease.InOutSine, onValueChange: val => alpha = val);
                hasChangedVisibility = false;
            }
            if (!visibilitytween.isAlive)
            {
                visibilitytween.Stop();
                visibilitytween = Tween.Custom(cycles:2, cycleMode:CycleMode.Yoyo, startValue:alpha, endValue:0.005f, duration:1, ease:Ease.InOutElastic, onValueChange: val => alpha = val);

            }

        }
        else
        {
            if(!hasChangedVisibility)
            {
                hasChangedVisibility = true;
                visibilitytween.Stop();
                visibilitytween = Tween.Custom(cycles: 1, startValue: alpha, endValue: 1, duration: 1, ease: Ease.InOutSine, onValueChange: val => alpha = val);
            }
        }
        foreach (GameObject obj in clairvoyantObjects)
        {
            obj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, alpha);
        }
        Debug.Log(alpha);
    }
}
