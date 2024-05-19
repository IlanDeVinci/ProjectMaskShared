using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image fade;
    [SerializeField] private Image logo;

    [SerializeField] private GameObject hair;
    [SerializeField] private Image anotis;
    [SerializeField] private Image fist;
    [SerializeField] private Image bladestart;
    [SerializeField] private Image bladeoption;
    [SerializeField] private Image bladequit;
    [SerializeField] private Button buttonstart;
    [SerializeField] private Button buttonoption;
    [SerializeField] private Button buttonquit;
    [SerializeField] private CanvasGroup canvasGroupTitle;
    [SerializeField] private Transform veilRotate;
    [SerializeField] private Image flicker;

    public void StartLevel()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        Sequence sequence = Sequence.Create().Chain(Tween.Alpha(fade, 0, 1, 1));
        yield return sequence.ToYieldInstruction();
        SceneManager.LoadSceneAsync("Part3_Camera");
    }
    // Start is called before the first frame update
    void Start()
    {
        PrimeTweenConfig.warnZeroDuration = false;
        Sequence sequence = Sequence.Create().
            Group(Tween.Alpha(fade, startValue: 1, endValue: 1, duration: 0)).
            Group(Tween.Alpha(canvasGroupTitle, 1, 0, 1)).
            Group(Tween.Scale(bladestart.transform, startValue: 1, endValue:0, 0.1f)).
            Group(Tween.Scale(bladeoption.transform, startValue: 1, endValue: 0, 0.1f)).
            Group(Tween.Scale(bladequit.transform, startValue: 1, endValue: 0, 0.1f)).
            Group(Tween.Alpha(buttonstart.targetGraphic, 1, 0, 3)).
            Group(Tween.Alpha(buttonoption.targetGraphic, 1, 0, 3)).
            Group(Tween.Alpha(buttonquit.targetGraphic, 1, 0, 3)).
            Chain(Tween.Alpha(logo, 0, 1, 2, Ease.InSine)).
            Group(Tween.Scale(logo.transform, 0, 1, 2)).
            ChainDelay(1).
            Chain(Tween.Alpha(logo, 0, 2)).
            Chain(Tween.Alpha(fade, startValue: 1, 0, 3, ease: Ease.InSine)).
            Group(Tween.Alpha(canvasGroupTitle, 0, 1, 2, startDelay:2)).
            Group(Tween.PositionX(hair.transform, hair.transform.position.x - 300, hair.transform.position.x, 3, ease: Ease.OutSine)).
            Group(Tween.PositionX(anotis.transform, anotis.transform.position.x - 300, anotis.transform.position.x, 3, ease: Ease.OutSine)).
            Group(Tween.PositionY(fist.transform, fist.transform.position.y - 300, fist.transform.position.y, 1.2f, ease: Ease.OutSine)).
            Group(Tween.Scale(bladestart.transform, 0.0f, 1, 3, ease: Ease.OutSine, startDelay: 1)).
            Group(Tween.Scale(bladeoption.transform, 0.0f, 1, 3, ease: Ease.OutSine, startDelay: 1.5f)).
            Group(Tween.Scale(bladequit.transform, 0.0f, 1, 3, ease: Ease.OutSine, startDelay: 2)).
            ChainDelay(-1f).
            Chain(Tween.Alpha(buttonstart.targetGraphic, 0.0f, 1, 1, ease: Ease.OutSine, startDelay: 0)).
            Group(Tween.Alpha(buttonoption.targetGraphic, 0.0f, 1, 1, ease: Ease.OutSine, startDelay: 0.5f)).
            Group(Tween.Alpha(buttonquit.targetGraphic, 0.0f, 1, 1, ease: Ease.OutSine, startDelay: 1))



            ;
        Tween.Rotation(hair.transform, new Vector3(0, 0, 10), 2, cycles: -1, cycleMode: CycleMode.Yoyo, endDelay: 0.05f);
        Tween.Rotation(veilRotate, new Vector3(0, 0, 1.5f), 3f, ease:Ease.OutSine, cycles: -1, cycleMode: CycleMode.Yoyo, endDelay: 0.07f);
        Tween.Custom(flicker.color, Color.clear, duration:2f, ease:Ease.InOutElastic, cycles: -1, cycleMode: CycleMode.Rewind, endDelay: 0.5f, onValueChange:val=> flicker.color = val);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
