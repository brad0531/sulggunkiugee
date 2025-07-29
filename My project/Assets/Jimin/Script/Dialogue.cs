using UnityEngine;
using System.Collections;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TMP_Text targetText;
    public TMP_Text targetName;

    private float delay = 0.075f;
    private int index = 0;
    private bool isTyping = false;

    IEnumerator DialoguePrint(int index)
    {
        isTyping = true;

        var script = GameManager.Instance.getScript(1, true, index);
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


    void ClickSkip()
    {
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
                    StopCoroutine(DialoguePrint(index));
                    targetText.text = dialogue;
                    isTyping = false;
                }

                if (!isTyping)
                {
                    index++;
                    StartCoroutine(DialoguePrint(index));
                }
            }
        }
    }

    void Start()
    {
        if (!isTyping)
        {
            StartCoroutine(DialoguePrint(index));
        }

    }

    void Update()
    {
        ClickSkip();
    }
}
