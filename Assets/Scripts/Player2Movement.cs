using System.Collections;
using UnityEngine;

public class Player2Movement : MonoBehaviour
{
    private CharacterController controller;
    public float Speed = 0.03f;
    public float Gravity = -9.8f;
    private Vector3 Velocity = Vector3.zero;
    public Transform GroundCheck;
    public int PlayerID = 2;
    public float CheckRadius = 0.2f;
    private bool IsGround;
    public LayerMask LayerMask;
    public float JumpHeight = 3f;
    public Vector3 StartPosition; // �_�l��m
    public Transform Player1; // Player1��Transform�A�ݭn�bUnity�s�边���]�m

    void Start()
    {
        controller = GetComponent<CharacterController>();
        StartPosition = transform.localPosition; // �b�C���}�l�ɰO���_�l��m
        // Debug.Log($"Player2: {transform.position} \nPlayer1: {Player1.position}");
    }

    void Update()
    {
        AutoFollowPlayer1();
        Debug.Log($"Player2: {transform.position} \nPlayer1: {Player1.position}");

        if (transform.position.y < 1f)
        {
            ResetPosition2(); // �p�G���⪺ y �b�y�Фp�� 1�A���m��m
        }
    }

    private void AutoFollowPlayer1()
    {
        if (!controller.enabled) return;

        IsGround = Physics.CheckSphere(GroundCheck.position, CheckRadius, LayerMask);
        if (IsGround && Velocity.y < 0)
        {
            Velocity.y = 0;
        }

        Vector3 direction = (Player1.position - transform.position).normalized;
        // �O�d������V�W�����ʡA����������V
        direction.y = 0;

        // �T�w Player2 �O�_���Ӳ���
        if (direction.magnitude > 0.1f) // �T�O�����������ʤ~�ਭ
        {
            controller.Move(direction * Speed * Time.deltaTime);
            // �� Player2 ��������V Player1
            transform.rotation = Quaternion.LookRotation(direction);
        }

        float horizontalDistance = Vector3.Distance(new Vector3(Player1.position.x, 0, Player1.position.z), new Vector3(transform.position.x, 0, transform.position.z));

        if (IsGround && horizontalDistance > 15f)
        {
            Velocity.y += Mathf.Sqrt(JumpHeight * -2 * Gravity);
        }

        Velocity.y += Gravity * Time.deltaTime;

        controller.Move(Velocity * Time.deltaTime);
    }



    public void ResetPosition2()
    {
        StartCoroutine(ResetPositionRoutine());
    }

    IEnumerator ResetPositionRoutine()
    {
        Debug.Log("Player2 Reset------------");
        controller.enabled = false;
        // transform.position = StartPosition;
        float range_x = 25f;
        float base_x = 0f;
        float range_z = 35f;
        float base_z = 0f;
        transform.localPosition = new Vector3(Random.Range(-range_x + base_x, range_x + base_x),2, Random.Range(-range_z + base_z, -range_z + base_z));
        Velocity = Vector3.zero;
        yield return new WaitForSeconds(1);
        controller.enabled = true;
    }
}
