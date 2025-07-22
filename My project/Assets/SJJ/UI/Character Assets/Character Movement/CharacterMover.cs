using TMPro;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public Transform characterTransform; // Character 이미지의 Transform
    public float moveSpeed = 5f; // 이동 속도
    private Vector2 targetPosition = new Vector3(0f, 10f, 0f);
    private bool isMoving = false;

    public Animator Runanimator;
    void Start()
    {
        characterTransform.position = targetPosition; // 타겟이 되는 포지션으로 이동(지금은 0, 0, 0) 나중에는 시작점
        //MoveTo(new Vector3(5f, 0f, 0f)); 
    }
    void Update()
    {
        MoveTo(targetPosition);
        if (isMoving)
        {
            MoveCharacter();
        }
    }

    // 외부에서 호출: 캐릭터를 특정 위치로 이동
    public void MoveTo(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isMoving = true;
        if (Runanimator != null)
            Runanimator.SetBool("isMoving", true); // 애니메이션 트리거
    }

    private void MoveCharacter()
    {
        characterTransform.position = Vector3.MoveTowards(
            characterTransform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(characterTransform.position, targetPosition) < 0.01f)
        {
            characterTransform.position = targetPosition;
            isMoving = false;
            if (Runanimator != null)
                Runanimator.SetBool("isMoving", false); // 정지 애니메이션
        }
    }
}
