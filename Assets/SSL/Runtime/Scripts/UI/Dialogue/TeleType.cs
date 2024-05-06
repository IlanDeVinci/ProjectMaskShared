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


    // Start is called before the first frame update

    public void ShowAll()
    {
        StopCoroutine(coroutine);
        textMeshPro.maxVisibleCharacters = totalVisibleChars;
        isDone = true;
    }

    public void DoTypeWriter()
    {
        coroutine = StartCoroutine(TypeWriter());
    }

    private IEnumerator TypeWriter()
    {
        isDone = false;
        textMeshPro.alpha = 0;
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

        visibleCount = 0;
        int audioCounter = 0;
        while (visibleCount <= totalVisibleChars - 1)
        {
            if (audioCounter % 3 == 0)
            {
                soundSource.Stop();
                soundSource.clip = soundClip;
                soundSource.Play();
            }

            audioCounter++;
            visibleCount = counter % (totalVisibleChars + 1);
            textMeshPro.maxVisibleCharacters = visibleCount;
            counter++;
            textMeshPro.alpha = 1;
            yield return new WaitForSeconds(0.03f);

            newCount = textMeshPro.textInfo.characterCount;
            if (totalVisibleChars != newCount)
            {
                yield break;
            }
        }

        isDone = true;
    }
}