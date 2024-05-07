using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject upgradeTree;
    [SerializeField] private Image fade;

    private Tween tween;

    public void Fade()
    {
        Tween.Alpha(fade, 1, 1, cycleMode: CycleMode.Yoyo, cycles: 2);
    }

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
            upgradeTree.SetActive(false);

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