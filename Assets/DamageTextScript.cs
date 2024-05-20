using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextScript : MonoBehaviour
{
    private float offset;

    [SerializeField] private TextMeshProUGUI textm;
    public Color color = Color.white;
    public int value = 0;
    private Vector3 startPos;

    private void Awake()
    {
        var random = new System.Random();
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
        textm.text = "";
        textm.alpha = 1;
        textm.text = $"-{value}";
        Tween.Alpha(textm, 0, 1.9f);
        startPos = new Vector2(transform.position.x + (float)random.Next(-50,50)/40f, transform.position.y+1.5f);
        offset = 0;
        Destroy(gameObject, 2);
    }
    void Update()
    {
        textm.color = new Color(color.r,color.g,color.b,textm.color.a);
        textm.text = $"-{value}";
        offset += Time.deltaTime * 1;
        transform.position = new Vector3(startPos.x, startPos.y + offset, 0);
    }
}
