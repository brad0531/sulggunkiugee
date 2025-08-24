using System;
using System.Collections;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Dialogue : MonoBehaviour
{
    public TMP_Text targetText;
    public TMP_Text targetName;

    public TMP_Text TutorialText;
    public TMP_Text TutorialName;

    public Tuple<int, int> stage = new Tuple<int, int>(0, 0);

    private float delay = 0.075f;
    public int index = 0;
    public bool turn;
    private float clickCooldown = 0.3f;
    private float lastClickTime = 0;

    public static Dialogue Instance { get; private set; }

    private bool isTyping = false;

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

    void FixedUpdate()
    {
        if (Time.time - lastClickTime > clickCooldown)
        {
            if ((Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
            {
                Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

                Debug.Log(stage.Item1);

                if (stage.Item1 == 0)
                {
                    HandleTutorialClick();
                }

                if (stage.Item1 != 0)
                {
                    HandleClick();
                }

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

    public IEnumerator MainDialogue(int index, int m, bool turn)
    {
        isTyping = true;

        var script = GameManager.Instance.getScript(m, turn, index);
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


    void HandleTutorialClick()
    { 
        var script = GameManager.Instance.getScript(0, false, index);
        string name = script.Item1;
        string dialogue = script.Item2;

                
        if (isTyping)
        {
            //StopCoroutine(TutorialDialogue(index));
            StopAllCoroutines();
            TutorialText.text = dialogue;
            isTyping = false;
            return;
        }

        if (!isTyping)
        {
             if ((index >= 0 && index <= 4) || (index >= 6 && index <= 11) || (index >= 14 && index <= 37))
             {
                 index++;
                 StartCoroutine(TutorialDialogue(index));
             }
        }
    }

    void HandleClick()
    {

        stage = GameManager.Instance.getStage();
  
        var script = GameManager.Instance.getScript(1, true, index);
        string name = script.Item1;
        string dialogue = script.Item2;

        if (index < GameManager.Instance.Scripts.Second.Count)
        {
            if (isTyping)
            {
                StopCoroutine(MainDialogue(index, stage.Item1, turn));
                targetText.text = dialogue;
                isTyping = false;
                return;
            }

            if (!isTyping)
            {
                index++;
                StartCoroutine(MainDialogue(index, stage.Item1, turn));
            }
        }
    }
}
