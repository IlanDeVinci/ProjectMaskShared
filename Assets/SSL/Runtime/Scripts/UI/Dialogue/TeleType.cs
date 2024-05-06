using PrimeTween;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeleType : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private int totalVisibleChars = 0;
    public int counter = 0;
    private int visibleCount = 0;
    public bool isDone = false;
    private Coroutine coroutine;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioClip soundClip;
    public Tween tween;


    // Start is called before the first frame update

    public void ShowAll()
    {
        StopCoroutine(coroutine);
        tween.Complete();
        //textMeshPro.maxVisibleCharacters = totalVisibleChars;
        isDone = true;
    }

    public void DoTypeWriter()
    {
        coroutine = StartCoroutine(TypeWriter());
    }

    private IEnumerator TypeWriter()
    {
        textMeshPro.maxVisibleCharacters = 0;
        isDone = false;
        yield return new WaitForEndOfFrame();
        totalVisibleChars = textMeshPro.textInfo.characterCount;
        counter = 0;
        int indexOfName = textMeshPro.text.IndexOf(':');
        int newCount = totalVisibleChars;
        Debug.Log(indexOfName);
        if (indexOfName != -1)
        {
            counter = indexOfName;
        }
        textMeshPro.maxVisibleCharacters = counter;
        int audioCounter = 0;
        tween = Tween.TextMaxVisibleCharacters(textMeshPro, totalVisibleChars, totalVisibleChars/10, ease:Ease.Linear);
        textMeshPro.alpha = 1;

        while (tween.isAlive)
        {
            if (audioCounter % 3 == 0)
            {
                soundSource.Stop();
                soundSource.clip = soundClip;
                soundSource.Play();
            }

            audioCounter++;
            yield return new WaitForSeconds(0.03f);
        }

        yield return tween.ToYieldInstruction();
        isDone = true;
    }
}