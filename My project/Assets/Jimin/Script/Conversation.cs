using UnityEngine;
using System.Collections;
using TMPro;

public class Conversation : MonoBehaviour
{
    public TMP_Text targetText;
    public TMP_Text name;

    private string[] text;
    private float delay = 0.075f;
    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    IEnumerator ConversationPrint(float d)
    {
        isTyping = true;
        int count = 0;

        while (count != text.Length) {

            if (count < text.Length)
            {
                targetText.text += text[count].ToString();
                count++;
            }

            yield return new WaitForSeconds(delay);
        }
    }

    void ShowNextText()
    {

    }

    void Start()
    {
        /* text = targetText.text.ToString();
        targetText.text = " ";
        StartCoroutine(ConversationPrint(delay)); */
    }

    void Update()
    {
        /* if (Input.GetMouseButtonDown(0))
        {
            if (!isTyping)
            {
                ShowNextText();
            }
        } */
    }
}
