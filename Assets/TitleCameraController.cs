using UnityEngine;

public class TitleCameraController : MonoBehaviour
{
    [Header("Camera Movement Settings")]
    public float moveSpeed = 2f; // 카메라 이동 속도
    public float moveDistance = 5f; // 좌우 이동 거리
    public float rotationSpeed = 10f; // 회전 속도 (선택사항)
    
    private Vector3 startPosition;
    private float time = 0f;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // 시간에 따라 좌우로 부드럽게 이동
        time += Time.deltaTime * moveSpeed;
        
        // 사인파를 사용해서 좌우로 부드럽게 이동
        float offset = Mathf.Sin(time) * moveDistance;
        
        // 카메라 위치 업데이트
        Vector3 newPosition = startPosition;
        newPosition.x += offset;
        transform.position = newPosition;
        
        // 선택사항: 카메라를 약간 회전시켜서 더 역동적으로 보이게
        // transform.RotateAround(startPosition, Vector3.up, Mathf.Sin(time * 0.5f) * 2f);
    }
}

