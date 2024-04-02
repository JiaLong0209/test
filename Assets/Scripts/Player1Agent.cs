using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Player1Agent : Agent
{
    public Transform Target; // 目标位置，用于计算距离等
    private CharacterController controller;
    public float Speed = 1f; // 移动速度
    public float JumpHeight = 1f; // 跳跃高度
    private Vector3 playerVelocity; // 玩家速度
    private bool isGrounded; // 是否在地面上
    public LayerMask GroundLayer; // 地面层，用于检测是否接触地面
    public Transform GroundCheck; // 用于检测是否在地面的Transform
    public float GroundDistance = 0.2f; // 与地面的检测距离
    private float lastDistanceToTarget = float.MaxValue;
    public float maxEpisodeTime = 30f; // 设定最大时间（秒）
    private float episodeTimer;
    public GameObject player2; // 需要在编辑器中设置
    private Vector3 startPosition;
    public int round = 0;
    public int round_win = 0;

    public void printInfo(){
        double winningRate = 0.0f;
        if(round != 0){
            winningRate = Math.Round(((double)round_win / round) * 100, 2);
        }
        Debug.Log($"Current episode reward: {GetCumulativeReward()}\nRound: {round}  Win: {round_win}  Winning Rate: {winningRate}%");
        // Debug.Log($"--------------------Reset-----------------------\nRound: {round}  Win: {round_win}  Winning Rate: {winningRate}%");
    }

    public override void Initialize()
    {
        controller = GetComponent<CharacterController>();
        startPosition = transform.localPosition; // 将当前局部位置作为初始位置
        episodeTimer = maxEpisodeTime; // 初始化计时器
    }

    public override void OnEpisodeBegin()
    {
        // 重置Agent的位置到初始位置、速度和计时器
        transform.localPosition = startPosition;
        // Debug.Log($" Message Player1: \n1.position: {transform.position}\nstart_position: {startPosition}\n2.position: {player2.transform.position}!!!");
        playerVelocity = Vector3.zero;
        episodeTimer = maxEpisodeTime; // 重置计时器
        lastDistanceToTarget = float.MaxValue;
    }

    public void Update()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 收集Agent的观察值
        // 例如：Agent与目标的距离和方向
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(player2.transform.localPosition);
        sensor.AddObservation(playerVelocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 执行动作
        var move = Vector3.zero;
        move.x = actions.ContinuousActions[0];
        move.z = actions.ContinuousActions[1];

        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundLayer, QueryTriggerInteraction.Ignore);
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        controller.Move(move * Speed * Time.deltaTime);

        if (actions.ContinuousActions[2] > 0.5f && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * Physics.gravity.y);
        }

        playerVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 奖励
        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);
        if (distanceToTarget < 3.0f) // 如果Agent接近目标
        {
            Debug.Log("Player1 reach target!");
            EndAndResetEpisode(true); // 结束这个回合
        }
        // else
        // {
        //     Debug.Log($"Distance to Target: {distanceToTarget}");
        //     AddReward(-0.01f); // 每步给予小的负奖励，鼓励Agent快速到达目标
        // }

        // // 计算与目标的距离
        if (distanceToTarget < lastDistanceToTarget)
        {
            AddReward(50.0f * (1/distanceToTarget)); // 如果Agent靠近目标，给予小量正奖励;
        }else{
            AddReward((-distanceToTarget+3.0f) * 0.15f);
        }

        lastDistanceToTarget = distanceToTarget;


        if (this.transform.localPosition.y < 1)
        {
            EndAndResetEpisode();
        }

        // 更新计时器
        episodeTimer -= Time.deltaTime;
        if (episodeTimer <= 0)
        {
            EndAndResetEpisode();
        }else{
            AddReward(-Time.deltaTime * 100f);
        }

        // 检查与Player2的碰撞
        float distanceToPlayer2 = Vector3.Distance(this.transform.localPosition, player2.transform.localPosition);
        float player2CollideSize = 2.5f;
        if (distanceToPlayer2 < player2CollideSize) // 假定距离阈值为2.5
        {
            EndAndResetEpisode();
        }
        else if(distanceToPlayer2 >= player2CollideSize && distanceToPlayer2 < 15.0f){
            AddReward(-20.0f * (1/distanceToPlayer2));
        }

        // Debug.Log($"Player1 position: {transform.position} \nTarget position: {Target.position}\nDist: {distanceToTarget}");

        printInfo();
    }

    private void ResetEnvironment()
    {
        // 重置Player1的位置和状态
        // transform.localPosition = startPosition; // 假定你有一个初始位置变量
        float range_x = 25f;
        float base_x = 0f;
        float range_z = 25f;
        float base_z = -0f;
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-range_x + base_x, range_x + base_x),2, UnityEngine.Random.Range(-range_z + base_z, range_z + base_z));
        playerVelocity = Vector3.zero;
        isGrounded = false;
        episodeTimer = maxEpisodeTime;

        // 重置Player2的位置和状态
        base_z = 0f;
        player2.transform.localPosition = new Vector3(UnityEngine.Random.Range(-range_x + base_x, range_x + base_x),2, UnityEngine.Random.Range(-range_z + base_z, range_z + base_z));

        // Change the position of target        
        base_z = 0f;
        Target.transform.localPosition = new Vector3(UnityEngine.Random.Range(-range_x + base_x, range_x + base_x),2, UnityEngine.Random.Range(-range_z + base_z, range_z + base_z));      
    }

    private void EndAndResetEpisode(bool isWin = false)
    {
        if(isWin){
            round_win++;
            AddReward(300.0f);
        }else{
            AddReward(-300.0f);
        }
        round++;
        printInfo();
        EndEpisode();
        ResetEnvironment(); // 确保在调用EndEpisode后立即调用
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetButton("Jump") ? 1f : 0f; // 注意: 使用GetButton而不是GetButtonDown
    }

}
