using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float minJumpPower;
    public float maxJumpPower;
    public float chargeTime;
    public Transform groundCheck;
    public LayerMask groundLayer;
    
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private bool isGrounded;
    private bool isChargingJump;
    private float jumpPower;
     
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // �̵� ����
        float h = Input.GetAxis("Horizontal");
        rigid.velocity = new Vector2(h * maxSpeed, rigid.velocity.y);

        // ���� ��ȯ
        if (h < 0)
            spriteRenderer.flipX = true;
        else if (h > 0)
            spriteRenderer.flipX = false;

        // ���� ó��
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isjump"))
        {
            isChargingJump = true;
            StartCoroutine(ChargeJump());
            anim.SetBool("isjump", true);
        }

        // �ִϸ��̼� ����
        anim.SetBool("isWalking", Mathf.Abs(h) > 0f);
        anim.SetBool("isJumping", true);
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        //�ൿ�� ���� �ִϸ��̼� ��ȯ
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("iswalking", false);
        else
            anim.SetBool("iswalking", true);
    }

    IEnumerator ChargeJump() //������ ��¡�ϴ� ������ �����ϴ� ���� �ڷ�ƾ �Լ��� ����ϴ� ����? �ڷ�ƾ �Լ��� �� ��¡ �ý��ۿ� �����ϱ� �����ε�, �ڷ�ƾ �Լ��� �����ϰ� �߰��� �Ͻ������� �۾��� ������ �� �ְ�, �� ���� �ѱ���� �ֱ� ����
    {
        float elapsedTime = 0f;  //��¡�ϴ� �ð��� ��Ÿ���� ���� elapsedTime�� �ʱ�ȭ��
        jumpPower = 0f; //���� �Ŀ��� 0���� �ʱ�ȭ��. �� ������ ��¡�� �����Ŀ��� �����ϴ� ������

        while (isChargingJump) //�� �ݺ����� isChargingJump������ ���� ���� �ݺ��ϴ� ���ε�, �̰ɷ� ��¡ ���϶� �Ͼ�� ������ ����ִ� ����. �̰� Jump�޼��� �ȿ� �־ ��ȣ�ۿ��� �ϴ� ����
        {
            elapsedTime += Time.deltaTime; //Time.deltaTime�� ���� �����Ӻ��� ���� �����ӱ����� �ð� ������ ��Ÿ���� ���ε�, ��¡�ϴ� �ð� + Time.deltaTime��� ������ ���Ͽ� ��¡�ϴ� �ð��� ���������� ������Ʈ�ϴ� ��
            jumpPower = Mathf.Lerp(minJumpPower, maxJumpPower, elapsedTime / chargeTime);

            if (!Input.GetButton("Jump") || elapsedTime >= chargeTime) //���� �����̽� ��ư�� ���ų� elapsedTime�� chargeTime ���� �̻��� �Ǹ�, Jump �޼��带 ȣ���ϰ� �ݺ����� ������
            {
                Jump(); //���� if���� �ι�° ���� ������, �����̽��ٸ� ������ ���� �ð��� ������ �ڵ����� �����̺�Ʈ�� �߻��ϰԵ�
                break; //�ݺ��� ����
            }

            yield return null;
        }
    }

    void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0f); // ���� �ӵ� �ʱ�ȭ
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        isChargingJump = false;
        jumpPower = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Die Zone")) //Die Zone Tag�� ������ �ִ� ������Ʈ�� �浹 ��, -1f, 2.57f, 0f��ǥ�� �÷��̾ �̵���Ŵ.
        {
            transform.position = new Vector3(-1f, 2.57f, 0f);
        }
        if (collision.gameObject.CompareTag("TpDown")) //Tp Tag�� ������ �ִ� ������Ʈ�� �浹 ��, ���� ��ġ���� Y��ǥ�� 10�Ʒ��� ������ �÷��̾ �̵���Ŵ.
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z);
        }
        if (collision.gameObject.CompareTag("TpUp")) //TpUp Tag�� ������ �ִ� ������Ʈ�� �浹 ��, ���� ��ġ �÷��̾���� Y��ǥ�� 5�� ��½�Ŵ.
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
        }
        if (collision.gameObject.CompareTag("GoBack")) //�������� (��ȹ : �÷��̾ �� ������Ʈ�� �浹��, �ݴ� �������� ���ư��� �Ϸ��� ������,, ���� ����)
        {
            // �浹�� ������Ʈ�� ��ġ�� �÷��̾��� ��ġ ��
            if (collision.transform.position.x > transform.position.x)
            {
                // �÷��̾ ���������� �̵� ���� ���
                rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
            }
            else
            {
                // �÷��̾ �������� �̵� ���� ���
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            }
        }
    }
    void FixedUpdate()
    {
        // Raycast Ȱ��
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isjump", false);
                }
        }
    }
}

