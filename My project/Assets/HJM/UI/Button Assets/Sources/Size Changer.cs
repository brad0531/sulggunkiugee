using UnityEngine;
using System.Collections;

public class SizeChanger : MonoBehaviour
{
    bool condition = false;

    public RectTransform targetButton;           // 회전할 버튼
    public GameObject panelToDeactivate;         // 토글할 패널
    public MonoBehaviour scriptToDisable;        // 토글할 스크립트

    public void onClick()
    {
        // 크기 조정
        if (!condition)
        {
            ChangeAnchorMinY(0.15f); // 확대
        }
        else
        {
            ChangeAnchorMinY(0.80f); // 축소
        }

        // 버튼 회전
        RotateButton();

        // 상태 반전
        condition = !condition;

        // 상태에 따른 활성/비활성 처리
        if (condition)
        {
            // true 상태: 비활성화
            if (panelToDeactivate != null)
                panelToDeactivate.SetActive(false);

            if (scriptToDisable != null)
                scriptToDisable.enabled = false;
        }
        else
        {
            // false 상태: 다시 활성화
            if (panelToDeactivate != null)
                panelToDeactivate.SetActive(true);

            if (scriptToDisable != null)
                scriptToDisable.enabled = true;
        }
    }

    void ChangeAnchorMinY(float y)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 currentMin = rt.anchorMin;
        rt.anchorMin = new Vector2(currentMin.x, y);
    }

    void RotateButton()
    {
        float targetAngle = condition ? -90f : 90f;
        targetButton.localRotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
