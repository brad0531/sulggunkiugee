using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                        // 따라갈 대상(캐릭터)
    public Vector3 offset = new Vector3(1, 1, -1); // 카메라와 타겟 간 거리
    public float followSpeed = 20f;                  // 따라가는 속도
    public float defaultSize = 10f; // 카메라 사이즈 기본값 설정

    private Camera cam;

    private void Start()
    {
        Camera.main.targetTexture = null;
    }
    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = defaultSize; // 10f로 카메라 사이즈 변경
    }
    void LateUpdate() // 가장 늦게 실행되는 업데이트, 떨림 제거
    {
        if (target == null) return;
        FollowTarget();
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = GetDesiredPosition();
        MoveCamera(desiredPosition);
    }
    private Vector3 GetDesiredPosition()
    {
        {
            // x만 타겟 기준으로 따라가고, y/z는 offset을 유지
            return new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y, // offset 유지
                target.position.z + offset.z  // offset 유지
            );
        }
    }
    private void MoveCamera(Vector3 desiredPosition)
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
