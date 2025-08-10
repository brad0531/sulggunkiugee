using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                         // 따라갈 대상(캐릭터)
    public Vector3 offset = new Vector3(1, -3.5f, -10); // UI와 겹치지 않게 y축 조정
    public float followSpeed = 6f;                   // 부드럽게 따라가는 속도
    public float defaultSize = 6f;                   // 카메라 기본 줌 크기

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = defaultSize;

        // Player 태그를 가진 오브젝트를 자동으로 target으로 지정
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("CameraFollow: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            offset.z
        );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
