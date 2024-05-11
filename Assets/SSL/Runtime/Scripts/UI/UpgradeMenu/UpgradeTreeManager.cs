using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI coins;
    [SerializeField] private RectTransform rect;
    private PauseManager pauseManager;
    private void Start()
    {
        pauseManager = GameObject.FindAnyObjectByType<PauseManager>();
        GlobalManager.isFirstTimeOpeningTree = true;
        canvasGroup.alpha = 0;
        canMove = false;
        xPos = 0;
        yPos = 0;
        InitTree();
        /*
        Tween tween = Tween.Custom(0, 1800, 2, onValueChange: newVal => xPos = newVal);
        Tween.Custom(0, -200, 2, onValueChange: newVal => yPos = newVal);
        */
    }

    private IEnumerator ShowTree()
    {
        yield return new WaitForSeconds(1);
        backgroundImage.transform.localPosition = new Vector2(xPos, yPos);
        canvasGroup.alpha = 1;
        StartCoroutine(OpenTree());
        SetAllColors();
    }
    public void ShowUpgradeTree()
    {
        canMove = false;
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        /*
        xPos = 0;
        yPos = 0;
            */
        StartCoroutine(ShowTree());
    }

    public void SetAllColors()
    {
        foreach (UpgradeButton upgradeButton in buttonsList)
        {
            upgradeButton.SetColors();
        }
        coins.text = GlobalManager.playerMoney.ToString();
    }

    public void HideUpgradeTree()
    {
        pauseManager.Fade();
        StartCoroutine(HideTree());

    }

    private IEnumerator HideTree()
    {
        yield return new WaitForSeconds(1);
        canMove = false;
        gameObject.SetActive(false);
    }

    private IEnumerator OpenTree()
    {
        yield return new WaitForEndOfFrame();
        if(GlobalManager.isFirstTimeOpeningTree)
        {
            xPos = 0; yPos = -200;
            Tween tween = Tween.Custom(0, 1800, 2, onValueChange: newVal => xPos = newVal);
            Tween.Custom(0, -201, 2, onValueChange: newVal => yPos = newVal);

            yield return tween.ToYieldInstruction();
            GlobalManager.isFirstTimeOpeningTree = false;
        }

        canMove = true;

    }


    private void InitTree()
    {
        foreach(GlobalUpgrades.Upgrade upgrade in GlobalUpgrades.Instance.Upgrades)
        {
            savedButton = Instantiate(buttonPrefab);
            savedButton.transform.SetParent(backgroundImage.transform, true);
            UpgradeButton upgradeButton = savedButton.GetComponent<UpgradeButton>();
            upgradeButton.upgrade = upgrade;
            upgradeButton.canvas = rect;
            savedButton.SendMessage("Initiate");
            buttonsList.Add(upgradeButton);
        }
        foreach (UpgradeButton upgradeButton in buttonsList)
        {
            upgradeButton.SendMessage("LinkButtons");
            upgradeButton.SendMessage("SetColors");
        }
        //backgroundImage.transform.localPosition= new Vector2(1800, 0);
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
        backgroundImage.transform.localPosition = new Vector2(xPos, yPos);

    }

}