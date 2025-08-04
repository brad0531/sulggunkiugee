using System;
using System.Collections;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dialogue : MonoBehaviour
{
    public TMP_Text targetText;
    public TMP_Text targetName;

    public TMP_Text TutorialText;
    public TMP_Text TutorialName;

    public Tuple<int, int> stage;

    private float delay = 0.075f;
    public int index = 0;

    public static Dialogue Instance { get; private set; }

    private bool isTyping = false;

    public IEnumerator DialoguePrint(int index, int m)
    { 
        isTyping = true;

        var script = GameManager.Instance.getScript(m, true, index);
        string speaker = script.Item1;
        string dialogue = script.Item2;

        targetName.text = speaker;
        targetText.text = "";

        for (int i = 0; i < dialogue.Length; i++)
        {
            targetText.text += dialogue[i];
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    public IEnumerator TutorialDialogue(int index)
    {
        isTyping = true;
        index = 0;

        var script = GameManager.Instance.getScript(0, false, index);
        string speaker = script.Item1;
        string dialogue = script.Item2;

        TutorialName.text = speaker;
        TutorialText.text = "";

        for (int i = 0; i < dialogue.Length; i++)
        {
            TutorialText.text += dialogue[i];
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;

        /* if(index == 5)
        {
            //공격력 상승 ui 제외 차단
            if(공격력 상승 성공 시)
            {
                index++;
                // ui 차단 해제
            } 
        }

        if (index == 12)
        {
            //다이아 던전 제외 차단
            if (다이아 던전 성공 시)
            {
                index++;
                // ui 차단 해제
            }
        }

        if (index == 13)
        {
            //묘기 해방 제외 차단
            if (묘기 해방 성공 시)
            {
                index++;
                // ui 차단 해제
            }
        } */
    } 


    void ClickSkip()
    {
        stage = GameManager.Instance.getStage();
        int MainStage = stage.Item1;

        if (Input.touchCount > 0)
        {
            Touch touchFirst = Input.GetTouch(0);

            if (MainStage != 0)
            {
                var script = GameManager.Instance.getScript(1, true, index);
                string name = script.Item1;
                string dialogue = script.Item2;

                if (touchFirst.phase == TouchPhase.Began)
                {
                    if (isTyping)
                    {
                        StopCoroutine(DialoguePrint(index, MainStage));
                        targetText.text = dialogue;
                        isTyping = false;
                    }

                    if (!isTyping)
                    {
                        index++;
                        StartCoroutine(DialoguePrint(index, MainStage));
                    }
                }
            }

            if (MainStage == 0)
            {
                var script = GameManager.Instance.getScript(0, false, index);
                string name = script.Item1;
                string dialogue = script.Item2;

                if (touchFirst.phase == TouchPhase.Began)
                {
                    if (isTyping)
                    {
                        StopCoroutine(TutorialDialogue(index));
                        TutorialText.text = dialogue;
                        isTyping = false;
                    }

                    if (!isTyping)
                    {
                        if((index >= 0 && index <= 4) || (index >= 6 && index <= 11 ) || (index >= 14 && index <= 37)) 
                        {
                            index++;
                            StartCoroutine(TutorialDialogue(index));
                        }
                    }
                }
            }
        }
    }

    void Start()
    {
        
    }

    private void Awake()
    {
        // 싱글턴(오브젝트가 중복되지 않고 하나만 존재하도록) 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        ClickSkip();

        if (Input.touchCount > 0)
        {
            Touch touchFirst = Input.GetTouch(0);

            if (touchFirst.phase == TouchPhase.Began)
            {
                if (stage.Item1 == 0 && index == 38 && !isTyping)
                {
                    var script = GameManager.Instance.getScript(0, false, index);
                    string speaker = script.Item1;
                    string dialogue = script.Item2;

                    TutorialText.text = "";
                    TutorialName.text = "";
                }
            }
        }
    }
}
