using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TextBox : MonoBehaviour
{
    public float alphaChangeSpeed = 5.0f;

    TextMeshProUGUI talkText;
    TextMeshProUGUI nameText;
    CanvasGroup canvasGroup;
    Image endImage;
    public GameObject scanObject;
    Animator endImageAnimator;

    TextSelect textSelet;
    Interaction interaction;

    public string talkString;
    public int talkIndex = 0;
    public float charPerSeconds = 0.05f;

    private bool talkingEnd;
    private bool talking;
    private bool typingTalk;
    private bool typingStop;
    public bool onScanObject;

    public NPCBase NPCdata;
    TextBoxManager textBoxManager; // TextBoxManager에 대한 참조

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        Transform child = transform.GetChild(1);
        talkText = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(2);
        nameText = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(3);
        endImage = child.GetComponent<Image>();
        endImageAnimator = child.GetComponent<Animator>();

        child = transform.GetChild(4);
        textSelet = child.GetComponent<TextSelect>();

        interaction = FindObjectOfType<Interaction>();

        // TextBoxManager에 대한 참조 가져오기
        textBoxManager = FindObjectOfType<TextBoxManager>();
    }

    private void Start()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        endImageAnimator.speed = 0.0f;

        if (scanObject != null)
        {
            GameManager.Instance.onTalkNPC += () =>
            {
                Action();
            };
        }
    }

    private void Update()
    {
        if (interaction != null)
        {
            scanObject = interaction.scanIbgect; // scanIbgect 값을 가져옴
        }
    }

    public void Action()
    {
        talkText.text = "";
        nameText.text = "";

        if (scanObject != null)
        {
            NPCdata = scanObject.GetComponent<NPCBase>();
        }
        else
        {
            NPCdata = null;
        }

        if (typingTalk == false && NPCdata != null)
        {
            endImageAnimator.speed = 0.0f;
            endImage.color = new Color(endImage.color.r, endImage.color.g, endImage.color.b, 0f);
            StartCoroutine(TalkStart());
        }
        else if (typingTalk == true && NPCdata != null)
        {
            typingStop = true;
            NPCdata = scanObject.GetComponent<NPCBase>();
            if (!talkingEnd)
            {
                talkIndex--;
            }
            Talk(NPCdata.id);
            nameText.text = $"{NPCdata.nameNPC}";
            endImageAnimator.speed = 1.0f;
            endImage.color = new Color(endImage.color.r, endImage.color.g, endImage.color.b, 1f);
            typingTalk = false;
        }
        else
        {
            Debug.Log("대상이 없음");
        }
    }

    public void ObjAction()
    {
        talkText.text = "";
        nameText.text = "";
        if (typingTalk == false)
        {
            StartCoroutine(TalkStart());
        }
        else
        {
            typingStop = true;
            NPCdata = scanObject.GetComponent<NPCBase>();
            if (!talkingEnd)
            {
                talkIndex--;
            }
            Talk(NPCdata.id);
            typingTalk = false;
        }
    }

    IEnumerator TalkStart()
    {
        if (!talking && !talkingEnd)
        {
            while (canvasGroup.alpha < 1.0f)
            {
                canvasGroup.alpha += Time.deltaTime * alphaChangeSpeed;
                yield return null;
            }
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            Talk(NPCdata.id);

            nameText.text = $"{NPCdata.nameNPC}";
            talkText.text = $"{talkString}";

            if (NPCdata.selectId)
            {
                textSelet.onSeletStart();
            }
            else
            {
                textSelet.onSeletEnd();
            }

            NPCdata.isTalk = true;
            StartCoroutine(TypingText(talkText.text));
        }
        else if (talking && !talkingEnd)
        {
            Talk(NPCdata.id);

            nameText.text = $"{NPCdata.nameNPC}";
            talkText.text = $"{talkString}";

            if (NPCdata.selectId)
            {
                textSelet.onSeletStart();
            }
            else
            {
                textSelet.onSeletEnd();
            }

            StartCoroutine(TypingText(talkText.text));
        }
        else
        {
            while (canvasGroup.alpha > 0.0f)
            {
                canvasGroup.alpha -= Time.deltaTime * alphaChangeSpeed;
                yield return null;
            }
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            talkText.text = "";
            nameText.text = "";
            talkIndex = 0;
            talking = false;
            talkingEnd = false;
            NPCdata.isTalk = false;
        }
    }

    IEnumerator TypingText(string text)
    {
        typingStop = false;
        typingTalk = true;
        talkText.text = null;
        for (int i = 0; i < text.Length; i++)
        {
            if (typingStop)
            {
                talkText.text = $"{talkString}";
                break;
            }
            talkText.text += text[i];
            yield return new WaitForSeconds(charPerSeconds);
            if (i + 2 > text.Length)
            {
                endImageAnimator.speed = 1.0f;
                endImage.color = new Color(endImage.color.r, endImage.color.g, endImage.color.b, 1f);
                typingTalk = false;
            }
        }
    }

    void Talk(int id)
    {
        if ((talkIndex + 1) == textBoxManager.GetTalkData(id).Length)
        {
            talkString = textBoxManager.GetTalkData(id)[talkIndex];
            talkingEnd = true;
            return;
        }
        talkString = textBoxManager.GetTalkData(id)[talkIndex];
        talking = true;
        talkIndex++;
    }

    public void OnSelect(int selectId)
    {
        NPCdata.id += selectId;
        talkingEnd = false;
        Action();
        textSelet.onSeletEnd();
    }

    // 대화가 출력될 때 호출되는 메서드 (예: 대화 시작 시 또는 대화가 업데이트될 때 호출)
 
}

