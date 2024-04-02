using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// �����Ҧ��P Command �M Commands ���������ǦC�Ƴ���

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
    public Vector3 StartPosition; // �_�l��m
    public GameObject Player2;
    public GameObject GameOver;
    public GameObject WIN;
    public float timeLimit = 20f; // �C���ɶ�����A�]�w��20��
    private float timeRemaining; // �Ѿl�ɶ�
    public TextMeshProUGUI timerText; // �ޥ�UI�奻����

    // Start is called before the first frame update
    void Start()
    {
        controller = transform.GetComponent<CharacterController>();
        StartPosition = transform.position; // �b�C���}�l�ɰO���_�l��m
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
        float collisionThreshold = 1.52f; // �o�O"�I��"�Q�P�_�o�ͪ��̤j�Z���A�ھڧA���ݭn�i��վ�

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
        controller.enabled = false; // �b�ק��m�e�{�ɸT�Ψ��ⱱ�
        transform.position = StartPosition;
        Velocity = Vector3.zero; // ���m�t��
        controller.enabled = true; // ���Ҩ��ⱱ�
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
