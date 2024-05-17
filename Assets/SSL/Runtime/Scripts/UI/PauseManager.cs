using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject panelBackground;

    [SerializeField] private GameObject upgradeTree;
    [SerializeField] private Image fade;
    [SerializeField] private GameObject player;
    [SerializeField] private CanvasGroup[] buttons;

    private Tween tween;
    private Sequence sequence;
    public void Fade()
    {
        tween = Tween.Alpha(fade, 1, 1, cycleMode: CycleMode.Yoyo, cycles: 2, useUnscaledTime: true);
    }

    public void FadeIn()
    {
        sequence = Sequence.Create(useUnscaledTime: true).Group(Tween.Alpha(fade, 0, 0.0001f, useUnscaledTime: true)).Chain(Tween.Alpha(fade, 1, 1, useUnscaledTime: true));

    }
    public void FadeOut()
    {
        sequence = Sequence.Create(useUnscaledTime: true).Group(Tween.Alpha(fade, 1, 0.0001f, useUnscaledTime: true)).Chain(Tween.Alpha(fade, 0, 1, useUnscaledTime: true));
    }

    // Start is called before the first frame update
    void Start()
    {
        FadeOut();
        canvasGroup.alpha = 0f;
        panel.SetActive(false);
        panelBackground.SetActive(false);
    }
    public void Retry()
    {
        StartCoroutine(RetryCo());
    }

    private IEnumerator RetryCo()
    {
        FadeIn();
        yield return new WaitForSeconds(1);
        player.SetActive(true);
        player.GetComponent<HeroEntity>().Reload();
        FadeOut();
        panel.SetActive(false);
    }
    private IEnumerator Pause()
    {
        if (!GlobalManager.isGamePaused)
        {
            GlobalManager.isGamePaused = true;
            panel.SetActive(true);
            panelBackground.SetActive(true);
            Tween.Scale(panel.transform, 0.5f, 1, 1, Ease.InOutCubic, useUnscaledTime:true);
            tween = Tween.Custom(canvasGroup.alpha, 1, 1, ease: Ease.InOutCubic, useUnscaledTime: true,
                onValueChange: newVal => canvasGroup.alpha = newVal);
            foreach(CanvasGroup canvasGroup in buttons)
            {
                Tween.Custom(startValue:0f, endValue:1f, duration:1, ease:Ease.OutSine, startDelay: 0.5f, useUnscaledTime: true, onValueChange:val => canvasGroup.alpha = val);
                Tween.PositionX(canvasGroup.transform, canvasGroup.transform.position.x-100, canvasGroup.transform.position.x, 1, Ease.OutSine, startDelay:0.5f, useUnscaledTime: true);
            }
            yield return tween.ToYieldInstruction();
        }
        else
        {
            Time.timeScale = 1f;
            Tween.Scale(panel.transform, 1f, 0.5f, 1, Ease.InOutCubic, useUnscaledTime: true);
            tween = Tween.Custom(canvasGroup.alpha, 0, 1, ease: Ease.InOutCubic, useUnscaledTime: true,
                onValueChange: newVal => canvasGroup.alpha = newVal);
            yield return tween.ToYieldInstruction();
            GlobalManager.isGamePaused = false;
            panel.SetActive(false);
            panelBackground.SetActive(false);
            upgradeTree.SetActive(false);
            foreach (CanvasGroup canvasGroup in buttons)
            {
                canvasGroup.alpha = 0;
            }

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