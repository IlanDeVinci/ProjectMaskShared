using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI textm;
    private Tween tween;
    public float scale = 1;
    void Start()
    {
        _canvasGroup.alpha = 0f;
    }

    public void ShowTooltip(string tip)
    {
        tween.Stop();
        textm.text = tip;
        tween = Tween.Alpha(_canvasGroup, 1, 0.7f, useUnscaledTime: true);
    }

    public void HideTooltip()
    {
        tween.Stop();
        tween = Tween.Alpha(_canvasGroup, 0, 0.4f, useUnscaledTime: true);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        transform.position = new Vector3(mouse.x + (250 * scale), mouse.y + (110 * scale), 0);

        if (mouse.x > Screen.width / 1.3f)
        {
            transform.position = new Vector3(mouse.x - (250 * scale), mouse.y + (110 * scale), 0);

        }

        if(mouse.y > Screen.height / 1.3f)
        {
            transform.position = new Vector3(transform.position.x, mouse.y - (110 * scale), 0);
        }
    }
}