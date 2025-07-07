using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                        // 따라갈 대상(캐릭터)
    public Vector3 offset = new Vector3(0, 0, -50); // 카메라와 타겟 간 거리
    public float followSpeed = 5f;                  // 따라가는 속도

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
        return target.position + offset;
    }
    private void MoveCamera(Vector3 desiredPosition)
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
