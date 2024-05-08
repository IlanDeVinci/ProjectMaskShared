using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image fade;

    public void StartLevel()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        Sequence sequence = Sequence.Create().Group(Tween.Alpha(fade, 0, 0.0001f)).Chain(Tween.Alpha(fade, 1, 1));
        yield return sequence.ToYieldInstruction();
        SceneManager.LoadSceneAsync("Part3_Camera");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
