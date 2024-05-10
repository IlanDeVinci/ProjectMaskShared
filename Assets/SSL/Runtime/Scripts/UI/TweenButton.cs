using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TweenButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Tween tween;
    public Tween tweenScale;
    private Image image;
    [SerializeField] private Color color;
    [SerializeField] TweenSettings<Vector3> settings;
    [SerializeField] private AudioClip soundEffect;
    [SerializeField] private AudioSource soundPlayer;
    public bool canTween = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(canTween)
        {
            soundPlayer.Stop();
            soundPlayer.clip = soundEffect;
            soundPlayer.Play();
            tween.Stop();
            tweenScale = Tween.Scale(transform, settings);
            tween = Tween.Color(image, color, duration: 0.5f);
        }

    }

    public void StopTweens()
    {
        tween.Stop();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(canTween)
        {
            tween.Stop();
            tweenScale = Tween.Scale(transform, settings.WithDirection(toEndValue: false));
            tween = Tween.Color(image, Color.white, duration: 0.5f);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
        image = GetComponent<Image>();
    }


    private void OnDestroy()
    {
        tween.Stop();
        tweenScale.Stop();
    }
}