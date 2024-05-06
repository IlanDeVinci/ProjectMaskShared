using System.Collections;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string[] Dialogs;
    private CanvasGroup canvasGroup;
    private int index = 0;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TeleType teleType;
    private string player;
    private string currentText;
    public bool isDialogueFinished = false;
    // Start is called before the first frame update

    private void Awake()
    {
        player = GlobalManager.playerName;
    }

    void Start()
    {
        gameObject.SetActive(true);
        isDialogueFinished = false;
        canvasGroup = GetComponent<CanvasGroup>();
        index = 0;
        teleType.counter = 0;
        currentText = Dialogs[index].Replace("player", player);
        currentText = currentText.Replace("flouze", GlobalManager.playerMoney.ToString());

        text.text = currentText;
        ;
        teleType.DoTypeWriter();
    }

    // Update is called once per frame
    private IEnumerator NoActive()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GlobalManager.playerMoney += 2000;

        if (index < Dialogs.Length - 1)
        {
            if (teleType.isDone)
            {
                index++;
                teleType.counter = 0;
                currentText = Dialogs[index].Replace("player", player);
                currentText = currentText.Replace("flouze", GlobalManager.playerMoney.ToString());
                text.text = currentText;
                teleType.DoTypeWriter();
            }
            else
            {
                teleType.ShowAll();
            }
        }
        else if (teleType.isDone)
        {
            Tween.Alpha(canvasGroup, 0, 1, Ease.OutCubic);
            isDialogueFinished = true;
            StartCoroutine(NoActive());
        }
        else

        {
            teleType.ShowAll();
        }
    }
}