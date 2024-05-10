using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinTextScript : MonoBehaviour
{
    private float offset;

    [SerializeField] private TextMeshProUGUI textm;
    public int value = 0;
    private Vector3 startPos;

    private void Awake()
    {
        var random = new System.Random();
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
        textm.text = "";
        textm.alpha = 1;
        textm.text = $"+{value}";
        Tween.Alpha(textm, 0, 1.9f);
        startPos = new Vector2(transform.position.x + (float)random.Next(-50, 50) / 40f, transform.position.y + 1.5f);
        offset = 0;
        Destroy(gameObject, 10);
    }
    void Update()
    {
        textm.text = $"+{value}";
        offset += Time.deltaTime * 1;
        transform.position = new Vector3(startPos.x, startPos.y + offset, 0);
    }
}
