using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 移除所有與 Command 和 Commands 類相關的序列化部分

public class Player1Movement : MonoBehaviour
{
    private CharacterController controller;
    public float Speed = 10f;
    public float RotateSpeed = 1f;
    public float Gravity = -9.8f;
    private Vector3 Velocity = Vector3.zero;
    public Transform GroundCheck;
    public int playerid = 1;
    public float CheckRadius = 0.2f;
    private bool IsGround;
    public LayerMask GroundMask;
    public float JumpHeight = 3f;
    public Vector3 StartPosition; // 起始位置
    public GameObject Player2;
    public GameObject GameOver;
    public GameObject WIN;
    public float timeLimit = 20f; // 遊戲時間限制，設定為20秒
    private float timeRemaining; // 剩餘時間
    public TextMeshProUGUI timerText; // 引用UI文本元素

    // Start is called before the first frame update
    void Start()
    {
        controller = transform.GetComponent<CharacterController>();
        StartPosition = transform.position; // 在遊戲開始時記錄起始位置
        GameOver.SetActive(false);
        WIN.SetActive(false);
        timeRemaining = timeLimit;
        UpdateTimerDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBasedOnInput();
        
        if (transform.position.z > 24.3f)
        {
            StartCoroutine(ShowGameWin());
            ResetPosition();
        }

        if (transform.position.y < 1f)
        {
            StartCoroutine(ShowGameOver());
            ResetPosition();
        }

        CheckForPlayer2Collision();

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
            if (timeRemaining <= 0)
            {
                StartCoroutine(ShowGameOver());
                ResetPosition();
            }
        }
    }

    private void CheckForPlayer2Collision()
    {
        float distanceToPlayer2 = Vector3.Distance(transform.position, Player2.transform.position);
        float collisionThreshold = 1.52f; // 這是"碰撞"被判斷發生的最大距離，根據你的需要進行調整

        if (distanceToPlayer2 < collisionThreshold)
        {
            StartCoroutine(ShowGameOver());
            ResetPosition();
        }
    }

    private void MoveBasedOnInput()
    {
        IsGround = Physics.CheckSphere(GroundCheck.position, CheckRadius, GroundMask);
        if (IsGround && Velocity.y < 0)
        {
            Velocity.y = 0f;
        }

        float horizontal = Input.GetAxis("Horizontal" + playerid);
        float vertical = Input.GetAxis("Vertical" + playerid);
        Vector3 move = transform.forward * vertical;
        controller.Move(move * Speed * Time.deltaTime);

        Velocity.y += Gravity * Time.deltaTime;
        controller.Move(Velocity * Time.deltaTime);

        transform.Rotate(Vector3.up, horizontal * RotateSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump" + playerid) && IsGround)
        {
            Velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
    }

    private void ResetPosition()
    {
        controller.enabled = false; // 在修改位置前臨時禁用角色控制器
        transform.position = StartPosition;
        Velocity = Vector3.zero; // 重置速度
        controller.enabled = true; // 重啟角色控制器
        timeRemaining = timeLimit;
        Player2.GetComponent<Player2Movement>().ResetPosition2();
    }
    
    IEnumerator ShowGameOver()
    {
        GameOver.SetActive(true);
        yield return new WaitForSeconds(2);
        GameOver.SetActive(false);
    }
    
    IEnumerator ShowGameWin()
    {
        WIN.SetActive(true);
        yield return new WaitForSeconds(2);
        WIN.SetActive(false);
    }

    void UpdateTimerDisplay()
    {
        timerText.text = $"{Mathf.CeilToInt(timeRemaining)}";
    }
}
