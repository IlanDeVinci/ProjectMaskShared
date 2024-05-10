using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LaserLightScript : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] public Color color;
    [SerializeField] public float intensity;
    [SerializeField] public float radius;
    [SerializeField] public float duration;
    [SerializeField] public float delay;
        
    void Start()
    {/*
        Tween.Custom(color, Color.clear, duration, startDelay: delay, onValueChange:val=> _light.color = val);
        Tween.Custom(0, radius, duration, startDelay: delay, onValueChange: val => _light.pointLightOuterRadius = val);
        Tween.Custom(0, intensity, duration, startDelay: delay, onValueChange: val => _light.intensity = val);
        */
        StartCoroutine(Light());
    }

    private IEnumerator Light()
    {
        Sequence sequence = Sequence.Create().Group(Tween.Custom(color, Color.clear, duration, startDelay: delay, onValueChange: val => { 
            _light.color = val;
            Debug.Log(val);
        })
            .Group(Tween.Custom(0, radius, duration, startDelay: delay, onValueChange: val => _light.pointLightOuterRadius = val)
            .Group(Tween.Custom(0, intensity, duration, startDelay: delay, onValueChange: val => _light.intensity = val).Chain(Tween.Custom(_light.intensity, 0, 0.5f, startDelay: delay, onValueChange: val => _light.intensity = val))))
);
        yield return sequence.ToYieldInstruction();
        Destroy(gameObject,0.1f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
