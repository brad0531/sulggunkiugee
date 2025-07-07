using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UIFollowWorldObject : MonoBehaviour
{
    public GameObject worldTarget;
    public Vector3 targetWorldPosition;
    public RectTransform uiElement;

    private Canvas _canvas;
    private Camera _cam;
    private RectTransform _canvasRect;

    void Awake()
    {
        if (uiElement == null)
            uiElement = GetComponent<RectTransform>();

        _canvas = uiElement.GetComponentInParent<Canvas>();
        if (_canvas == null || _canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogError("WorldSpace Canvas 안에 이 스크립트를 사용해야 합니다.");
            enabled = false;
            return;
        }

        _canvasRect = _canvas.GetComponent<RectTransform>();
        _cam = _canvas.worldCamera != null ? _canvas.worldCamera : Camera.main;
    }

    void Update()
    {
        Vector3 worldPos = worldTarget != null
            ? worldTarget.transform.position
            : targetWorldPosition;

        MoveUITo(worldPos);
    }

    void MoveUITo(Vector3 worldPos)
    {
        Vector2 screenPt = RectTransformUtility.WorldToScreenPoint(_cam, worldPos);
        Vector2 localPt;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, screenPt, _cam, out localPt);
        if (uiElement.anchorMin == Vector2.zero && uiElement.anchorMax == Vector2.one)
        {
            Vector2 size = uiElement.rect.size;
            Vector2 pivot = uiElement.pivot;

            Vector2 newMin = localPt - Vector2.Scale(size, pivot);
            Vector2 newMax = newMin + size;

            uiElement.offsetMin = newMin;
            uiElement.offsetMax = newMax;
        }
        else
        {
            uiElement.anchoredPosition = localPt;
        }
    }
}