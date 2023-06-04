using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈 오브젝트, 즉 플레이어 오브젝트를 여기에 할당합니다.
    public float smoothSpeed = 0.5f; // 카메라 이동의 부드러움 정도를 실수형으로 저장
    public float yOffset = 0f; // 카메라의 y축 오프셋 값을 저장하는 변수

    private Vector3 desiredPosition; // 목표로 하는 카메라 위치를 저장할 변수

    private void LateUpdate()
    {
        // 플레이어의 현재 위치에 y축 오프셋 값을 더한 위치를 목표로 설정하기
        desiredPosition = new Vector3(transform.position.x, target.position.y + yOffset, transform.position.z);
        // 부드러운 이동을 위해 Lerp를 사용하여 현재 카메라 위치에서 목표 위치로 이동
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
