using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelScript : MonoBehaviour
{
    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().name == "Tuto")
        {
            StartCoroutine(GoNext("Level 2"));
        }
        else
        {
            StartCoroutine(GoNext("BossScene"));

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerTrigger"))
        {
            if (SceneManager.GetActiveScene().name == "Tuto")
            {
                StartCoroutine(GoNext("Level 2"));
            }
            else
            {
                StartCoroutine(GoNext("BossScene"));

            }
        }
    }

    private IEnumerator GoNext(string name)
    {
        FindAnyObjectByType<PauseManager>().FadeIn();
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadScene(name);
    }
    // Start is called before the first frame update
    void Start()
    {
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
