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
        // 이동 로직
        float h = Input.GetAxis("Horizontal");
        rigid.velocity = new Vector2(h * maxSpeed, rigid.velocity.y);

        // 방향 전환
        if (h < 0)
            spriteRenderer.flipX = true;
        else if (h > 0)
            spriteRenderer.flipX = false;

        // 점프 처리
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isjump"))
        {
            isChargingJump = true;
            StartCoroutine(ChargeJump());
            anim.SetBool("isjump", true);
        }

        // 애니메이션 설정
        anim.SetBool("isWalking", Mathf.Abs(h) > 0f);
        anim.SetBool("isJumping", true);
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        //행동에 따른 애니메이션 전환
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("iswalking", false);
        else
            anim.SetBool("iswalking", true);
    }

    IEnumerator ChargeJump() //점프를 차징하는 동작을 구현하는 로직 코루틴 함수를 사용하는 이유? 코루틴 함수가 이 차징 시스템에 적합하기 때문인데, 코루틴 함수는 유연하게 중간에 일시적으로 작업을 중지할 수 있고, 또 값을 넘길수도 있기 때문
    {
        float elapsedTime = 0f;  //차징하는 시간을 나타내는 변수 elapsedTime을 초기화함
        jumpPower = 0f; //점프 파워를 0으로 초기화함. 이 변수는 차징된 점프파워를 저장하는 변수임

        while (isChargingJump) //이 반복문은 isChargingJump변수가 참일 때만 반복하는 문인데, 이걸로 차징 중일때 일어나는 로직을 집어넣는 것임. 이건 Jump메서드 안에 있어서 상호작용을 하는 구조
        {
            elapsedTime += Time.deltaTime; //Time.deltaTime는 이전 프레임부터 현재 프레임까지의 시간 간격을 나타내는 값인데, 차징하는 시간 + Time.deltaTime라는 변수를 더하여 차징하는 시간을 지속적으로 업데이트하는 것
            jumpPower = Mathf.Lerp(minJumpPower, maxJumpPower, elapsedTime / chargeTime);

            if (!Input.GetButton("Jump") || elapsedTime >= chargeTime) //만약 스페이스 버튼을 떼거나 elapsedTime이 chargeTime 값의 이상이 되면, Jump 메서드를 호출하고 반복문을 종료함
            {
                Jump(); //위의 if문의 두번째 조건 때문에, 스페이스바를 누르고 일정 시간이 지나면 자동으로 점프이벤트가 발생하게됨
                break; //반복문 종료
            }

            yield return null;
        }
    }

    void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0f); // 수직 속도 초기화
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        isChargingJump = false;
        jumpPower = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Die Zone")) //Die Zone Tag를 가지고 있는 오브젝트와 충돌 시, -1f, 2.57f, 0f좌표로 플레이어를 이동시킴.
        {
            transform.position = new Vector3(-1f, 2.57f, 0f);
        }
        if (collision.gameObject.CompareTag("TpDown")) //Tp Tag를 가지고 있는 오브젝트와 충돌 시, 현재 위치에서 Y좌표가 10아래인 곳으로 플레이어를 이동시킴.
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z);
        }
        if (collision.gameObject.CompareTag("TpUp")) //TpUp Tag를 가지고 있는 오브젝트와 충돌 시, 현재 위치 플레이어를에서 Y좌표를 5를 상승시킴.
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
        }
        if (collision.gameObject.CompareTag("GoBack")) //구현실패 (계획 : 플레이어가 이 오브젝트와 충돌시, 반대 방향으로 날아가게 하려고 했으나,, 구현 실패)
        {
            // 충돌한 오브젝트의 위치와 플레이어의 위치 비교
            if (collision.transform.position.x > transform.position.x)
            {
                // 플레이어가 오른쪽으로 이동 중인 경우
                rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
            }
            else
            {
                // 플레이어가 왼쪽으로 이동 중인 경우
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            }
        }
    }
    void FixedUpdate()
    {
        // Raycast 활용
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

