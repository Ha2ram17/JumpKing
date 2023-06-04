using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target; // ī�޶� ���� ������Ʈ, �� �÷��̾� ������Ʈ�� ���⿡ �Ҵ��մϴ�.
    public float smoothSpeed = 0.5f; // ī�޶� �̵��� �ε巯�� ������ �Ǽ������� ����
    public float yOffset = 0f; // ī�޶��� y�� ������ ���� �����ϴ� ����

    private Vector3 desiredPosition; // ��ǥ�� �ϴ� ī�޶� ��ġ�� ������ ����

    private void LateUpdate()
    {
        // �÷��̾��� ���� ��ġ�� y�� ������ ���� ���� ��ġ�� ��ǥ�� �����ϱ�
        desiredPosition = new Vector3(transform.position.x, target.position.y + yOffset, transform.position.z);
        // �ε巯�� �̵��� ���� Lerp�� ����Ͽ� ���� ī�޶� ��ġ���� ��ǥ ��ġ�� �̵�
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
