using System;
using System.Collections;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public TMP_Text targetText;
    public TMP_Text targetName;

    public Tuple<int, int> stage;

    private float delay = 0.075f;
    private int index = 0;
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

    /* IEnumerator TutorialDialogue(int i)
    {
        isTyping = true;
    } */


    void ClickSkip()
    {
        stage = GameManager.Instance.getStage();
        int MainStage = stage.Item1;

        if (Input.touchCount > 0)
        {
            Touch touchFirst = Input.GetTouch(0);

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
    }

    void Start()
    {

    }

    void Update()
    {
        ClickSkip();
    }
}
