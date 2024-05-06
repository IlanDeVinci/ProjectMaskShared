using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTreeManager : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private int mDelta = 200;
    public float xPos;
    public float yPos;
    [SerializeField] private float maxXPos;
    [SerializeField] private float maxYPos;
    [SerializeField] private int mSpeed;
    [SerializeField] private bool canMove = true;
    private Tween tweenX;
    private Tween tweenY;
    [SerializeField] private Ease ease;
    [SerializeField] private TweenSettings settings;
    [SerializeField] private GameObject buttonPrefab;
    private GameObject savedButton;
    private bool isGoingRight = false;
    private bool isGoingUp = false;
    [SerializeField] public List<UpgradeButton> buttonsList;
    private void Start()
    {
        xPos = 1000;
        yPos = 0;
        InitTree();
    }

    private void InitTree()
    {
        foreach(GlobalUpgrades.Upgrade upgrade in GlobalUpgrades.Instance.Upgrades)
        {
            savedButton = Instantiate(buttonPrefab);
            savedButton.transform.SetParent(backgroundImage.transform, true);
            UpgradeButton upgradeButton = savedButton.GetComponent<UpgradeButton>();
            upgradeButton.upgrade = upgrade;
            savedButton.SendMessage("Initiate");
            buttonsList.Add(upgradeButton);
        }
        foreach (UpgradeButton upgradeButton in buttonsList)
        {
            Debug.Log("sentmessage");
            upgradeButton.SendMessage("LinkButtons");
        }
        
    }

    private void DoMovement()
    {
        if (Input.mousePosition.x >= Screen.width - mDelta && xPos > -maxXPos)
        {

            float startValue = xPos;
            float endValue = Mathf.Min(xPos - transform.right.x * mSpeed, maxXPos);
            if (!isGoingRight || (tweenX.isAlive && tweenX.progress > 0.5f) || !tweenX.isAlive)
            {
                isGoingRight = true;
                tweenX.Stop();
                tweenX = Tween.Custom(startValue, endValue, settings, onValueChange: newVal => xPos = newVal);

            }
        }
        if (Input.mousePosition.x <= 0 + mDelta && xPos < maxXPos)
        {
            float startValue = xPos;
            float endValue = Mathf.Min(xPos + transform.right.x * mSpeed, maxXPos);
            if (isGoingRight || (tweenX.isAlive && tweenX.progress > 0.5f) || !tweenX.isAlive)
            {
                isGoingRight = false;
                tweenX.Stop();
                tweenX = Tween.Custom(startValue, endValue, settings, onValueChange: newVal => xPos = newVal);

            }

        }
        if (Input.mousePosition.y >= Screen.height - mDelta && yPos > -maxYPos)
        {
            float startValue = yPos;
            float endValue = Mathf.Min(yPos - transform.up.y * mSpeed, maxYPos);
            if (!isGoingUp || (tweenY.isAlive && tweenY.progress > 0.5f) || !tweenY.isAlive)
            {
                isGoingUp = true;
                tweenY.Stop();
                tweenY = Tween.Custom(startValue, endValue, settings, onValueChange: newVal => yPos = newVal);

            }
        }
        if (Input.mousePosition.y <= 0 + mDelta && yPos < maxYPos)
        {
            float startValue = yPos;
            float endValue = Mathf.Min(yPos + transform.up.y * mSpeed, maxYPos);
            if (!isGoingUp || (tweenY.isAlive && tweenY.progress > 0.5f) || !tweenY.isAlive)
            {
                isGoingUp = false;
                tweenY.Stop();
                tweenY = Tween.Custom(startValue, endValue, settings, onValueChange: newVal => yPos = newVal);

            }
        }

        if (yPos > maxYPos)
        {
            yPos = maxYPos;
        }

        if (yPos < -maxYPos)
        {
            yPos = -maxYPos;
        }

        if (xPos > maxXPos)
        {
            xPos = maxXPos;
        }

        if (xPos < -maxXPos)
        {
            xPos = -maxXPos;
        }

        backgroundImage.transform.position = new Vector2(xPos + 960, yPos + 540);
    
}
    private void Update()
    {
        if (GlobalManager.isGamePaused)
        {
            if (canMove)
            {
                DoMovement();
            }
        }

    }

}