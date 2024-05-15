using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private HealthManager healthManager;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private CanvasGroup deathPanelcanvas;
    [SerializeField] private Image avatar;
    [SerializeField] private Sprite[] avatars;
    private bool isOut = false;
    // Start is called before the first frame update
    void Start()
    {
        deathPanel.SetActive(false);
        deathPanelcanvas.alpha = 0;
        healthManager = GameObject.FindAnyObjectByType<HealthManager>();
    }


    private void Update()
    {
        if (healthManager.currentHealth > healthManager.maxHealth/2)
        {
            avatar.sprite = avatars[0];
        } else if (healthManager.currentHealth > healthManager.maxHealth / 5)
        {
            avatar.sprite = avatars[1];
        }
        else
        {
            avatar.sprite = avatars[2];
        }
        if (healthManager.currentHealth <= 0)
        {
            if(!isOut)
            {
                isOut = true;
                deathPanel.SetActive(true);
                Tween.Alpha(deathPanelcanvas, 1, 1);
            }
        }
        else
        {
            deathPanel.SetActive(false);
            isOut = false;
        }

    }

}
