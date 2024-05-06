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

    void Start()
    {
        _canvasGroup.alpha = 0f;
    }

    public void ShowTooltip(string tip)
    {
        tween.Stop();
        textm.text = tip;
        tween = Tween.Alpha(_canvasGroup, 1, 0.7f);
    }

    public void HideTooltip()
    {
        tween.Stop();
        tween = Tween.Alpha(_canvasGroup, 0, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        transform.position = new Vector3(mouse.x + 250, mouse.y + 110, 0);
    }
}