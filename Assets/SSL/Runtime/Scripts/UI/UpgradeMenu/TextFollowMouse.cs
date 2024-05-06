using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using PrimeTween;
using UnityEngine.UI;

public class TextFollowMouse : MonoBehaviour
{
    private float offset;
    [SerializeField] private TextMeshProUGUI textm;
    [SerializeField] private RawImage image;
    [SerializeField] private RawImage x;


    public int value = 0;
    private Vector3 startPos;

    private void Awake()
    {
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
        textm.text = "";
        textm.alpha = 1;

        if (value != 0)
        {
            textm.text = $"-{value}x";
            x.gameObject.SetActive(false);
        }
        else
        {
            Tween.Alpha(x, 0, 2);
            image.gameObject.SetActive(false);
        }

        Tween.Alpha(textm, 0, 2);
        Tween.Alpha(image, 0, 2);
        Vector3 mouse = Input.mousePosition;
        transform.position = new Vector3(mouse.x, mouse.y, 0);
        startPos = transform.position;
        offset = 0;
        Destroy(gameObject, 2);
    }

    private void Update()
    {
        textm.text = "";
        if (value != 0)
        {
            textm.text = $"-{value}x";
            x.gameObject.SetActive(false);
            image.gameObject.SetActive(true);
        }
        else
        {
            x.gameObject.SetActive(true);
        }

        offset += Time.deltaTime * 100;
        transform.position = new Vector3(startPos.x, startPos.y + offset, 0);
    }
}