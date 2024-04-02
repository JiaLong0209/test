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
    public Vector3 StartPosition; // 起始位置
    public Transform Player1; // Player1的Transform，需要在Unity編輯器中設置

    void Start()
    {
        controller = GetComponent<CharacterController>();
        StartPosition = transform.localPosition; // 在遊戲開始時記錄起始位置
        // Debug.Log($"Player2: {transform.position} \nPlayer1: {Player1.position}");
    }

    void Update()
    {
        AutoFollowPlayer1();
        Debug.Log($"Player2: {transform.position} \nPlayer1: {Player1.position}");

        if (transform.position.y < 1f)
        {
            ResetPosition2(); // 如果角色的 y 軸座標小於 1，重置位置
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
        // 保留水平方向上的移動，忽略垂直方向
        direction.y = 0;

        // 確定 Player2 是否應該移動
        if (direction.magnitude > 0.1f) // 確保有足夠的移動才轉身
        {
            controller.Move(direction * Speed * Time.deltaTime);
            // 使 Player2 的身體轉向 Player1
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
