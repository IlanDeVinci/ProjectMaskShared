using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject panel;

    private Tween tween;


    // Start is called before the first frame update
    void Start()
    {
        canvasGroup.alpha = 0f;
        panel.SetActive(false);
    }

    private IEnumerator Pause()
    {
        if (!GlobalManager.isGamePaused)
        {
            GlobalManager.isGamePaused = true;
            panel.SetActive(true);
            tween = Tween.Custom(canvasGroup.alpha, 1, 1, ease: Ease.InOutCubic,
                onValueChange: newVal => canvasGroup.alpha = newVal);
            yield return tween.ToYieldInstruction();
        }
        else
        {
            tween = Tween.Custom(canvasGroup.alpha, 0, 1, ease: Ease.InOutCubic,
                onValueChange: newVal => canvasGroup.alpha = newVal);
            yield return tween.ToYieldInstruction();
            GlobalManager.isGamePaused = false;
            panel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!tween.isAlive)
            {
                StartCoroutine(Pause());
            }
        }
    }
}