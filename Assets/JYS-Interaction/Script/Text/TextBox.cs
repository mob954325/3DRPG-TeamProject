using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    Dictionary<int, string[]> talkData;

    public float alphaChangeSpeed = 5.0f;

    TextMeshProUGUI talkText;
    TextMeshProUGUI nameText;
    CanvasGroup canvasGroup;
    Image endImage;
    public GameObject scanObject;

    TextSelect textSelet;

    public string talkString;
    public int talkIndex = 0;
    public float charPerSeconds = 0.05f;

    private bool talkingEnd;
    private bool talking;
    private bool typingTalk;
    private bool typingStop;

    //private bool isNpc;

    public NPCBase NPCdata;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        Transform child = transform.GetChild(0);
        talkText = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(1);
        nameText = child.GetComponent<TextMeshProUGUI>();

        child = transform.GetChild(2);
        endImage = child.GetComponent<Image>();

        child = transform.GetChild(3);
        textSelet = child.GetComponent<TextSelect>();

        talkData = new Dictionary<int, string[]>();

        StopAllCoroutines();
        GenerateData();
    }

    private void Start()
    {
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        

        GameManager.Instance.onTalkNPC += () =>
        {
            //isNpc = true;
            Action();
        };

        GameManager.Instance.onTalkObj += () =>
        {
            //isNpc = false;
            ObjAction();
        };
    }

    public void Action()
    {
        talkText.text = "";
        nameText.text = "";
        //scanObject = gameObject;
        if (typingTalk == false)
        {
            endImage.color = new Color(endImage.color.r, endImage.color.g, endImage.color.b, 0f);
            StartCoroutine(TalkStart());
        }
        else
        {
            typingStop = true;
            //StopCoroutine(TypingText(talkText.text));
            NPCdata = scanObject.GetComponent<NPCBase>();
            if (!talkingEnd)
            {
                talkIndex--;
            }
            Talk(NPCdata.id);
            nameText.text = $"{NPCdata.nameNPC}";
            //talkText.text = $"{talkString}";
            endImage.color = new Color(endImage.color.r, endImage.color.g, endImage.color.b, 1f);
            typingTalk = false;
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

            NPCdata = scanObject.GetComponent<NPCBase>();
            Talk(NPCdata.id);

            nameText.text = $"{NPCdata.nameNPC}";
            talkText.text = $"{talkString}";

            StartCoroutine(TypingText(talkText.text));

        }
        else if (talking && !talkingEnd) 
        {
            Talk(NPCdata.id);

            nameText.text = $"{NPCdata.nameNPC}";
            talkText.text = $"{talkString}";

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

        }
    }

    /// <summary>
    /// 텍스트 타이핑 효과
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
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

                typingTalk = false;
            }
        }
    }

    void Talk(int id)
    {
        if ((talkIndex + 1) == talkData[id].Length)
        {
            talkString = talkData[id][talkIndex];
            talkingEnd = true;
            return;
        }
        talkString = talkData[id][talkIndex];
        talking = true;
        talkIndex++;
    }

    void GenerateData()
    {
        talkData.Add(0, new string[] { "초기값" });
        talkData.Add(100, new string[] { "아이템을 획득했다." });
        talkData.Add(110, new string[] { "이미 아이템을 획득한 상자이다." });
        talkData.Add(1000, new string[] { "애국가는 말 그대로 '나라를 사랑하는 노래'를 뜻한다.", "1896년 '독립신문' 창간을 계기로 여러 가지의 애국가 가사가 신문에 게재되기 시작했는데", "이 노래들을 어떤 곡조로 불렀는가는 명확하지 않다.", "다만 대한제국이 서구식 군악대를 조직해 1902년 '대한제국 애국가'라는 이름의 국가를 만들어" ," 나라의 주요 행사에 사용했다는 기록은 지금도 남아 있다." });
        talkData.Add(1010, new string[] { "다음대사","AAAAA" });
        talkData.Add(1011, new string[] { "선택지 1 선택완료", "AAAAA" });
        talkData.Add(1012, new string[] { "선택지 2 선택완료", "BBBBB" });
        talkData.Add(1013, new string[] { "선택지 3 선택완료", "CCCCC" });
        talkData.Add(2000, new string[] { "가나다라마바사  아자차카타파하  가나다라마바사  아자차카타파하  가나다라마바사  아자차카타파하" });
    }

    void OnSelet(int SeletId)
    {
        textSelet.onSeletStart();

        if (SeletId == 0)
        {

        }
        else if(SeletId == 1)
        {

        }else if(SeletId == 2)
        {

        }else
        {

        }
        
    }


}
