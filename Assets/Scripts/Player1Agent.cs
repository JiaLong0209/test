using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Player1Agent : Agent
{
    public int PlayerId = 0;
    public Environment Env;
    public Transform Target; // 目标位置，用于计算距离等
    private CharacterController controller;
    public float Speed = 15f; // 移动速度
    public float JumpHeight = 3f; // 跳跃高度
    private Vector3 PlayerVelocity; // 玩家速度
    private bool IsGrounded; // 是否在地面上
    public LayerMask GroundLayer; // 地面层，用于检测是否接触地面
    public Transform GroundCheck; // 用于检测是否在地面的Transform
    public float GroundDistance = 0.2f; // 与地面的检测距离
    private float LastDistanceToTarget = float.MaxValue;
    public float MaxEpisodeTime = 30f; // 设定最大时间（秒）
    private float EpisodeTimer;
    public GameObject Player2; // 需要在编辑器中设置
    private Vector3 startPosition;
    public float TargetCollisionRange = 3.0f;
    public float Player2CollisionRange = 2.5f;
    public bool activeRandomPlayerPosition = false;

    public void PrintInfo(){
        double winningRate = 0.0f;
        if(Global.Round != 0){
            winningRate = Math.Round(((double)Global.PlayersRoundWin[PlayerId] / Global.Round) * 100, 2);
        }
        Debug.Log($"Current episode reward: {GetCumulativeReward()}\nRound: {Global.Round}  Win: {Global.PlayersRoundWin[PlayerId]}  Win rate: {winningRate}% Win streak: {Global.PlayersWinStreak[PlayerId]}");
    }


    public override void Initialize()
    {
        controller = GetComponent<CharacterController>();
        startPosition = transform.localPosition; // 将当前局部位置作为初始位置
        EpisodeTimer = MaxEpisodeTime; // 初始化计时器
    }

    public override void OnEpisodeBegin()
    {
        // 重置Agent的位置到初始位置、速度和计时器
        transform.localPosition = startPosition;


        // Debug.Log($" Message Player1: \n1.position: {transform.position}\nstart_position: {startPosition}\n2.position: {Player2.transform.position}!!!");
        PlayerVelocity = Vector3.zero;
        EpisodeTimer = MaxEpisodeTime; // 重置计时器
        LastDistanceToTarget = float.MaxValue;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 收集Agent的观察值
        // 例如：Agent与目标的距离和方向
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(Player2.transform.localPosition);
        sensor.AddObservation(PlayerVelocity);
        if(Global.IsActiveSpeedObservation){
            sensor.AddObservation(Speed);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 执行动作
        var move = Vector3.zero;
        move.x = actions.ContinuousActions[0];
        move.z = actions.ContinuousActions[1];

        // foreach (var a in actions.ContinuousActions)
        // {
        //     Debug.Log($"{a}");
        // }
        // Debug.Log($"--------------------------------------");


        IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundLayer, QueryTriggerInteraction.Ignore);
        if (IsGrounded && PlayerVelocity.y < 0)
        {
            PlayerVelocity.y = 0f;
        }

        controller.Move(move * Speed * Time.deltaTime);

        if (actions.ContinuousActions[2] > 0.5f && IsGrounded)
        {
            PlayerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * Physics.gravity.y);
        }

        PlayerVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(PlayerVelocity * Time.deltaTime);

        // Target reward
        float DistanceToTarget = Vector3.Distance(this.transform.position, Target.position);
        if (DistanceToTarget < TargetCollisionRange) // 如果Agent collide 目标
        {
            Debug.Log("Player1 reach target!");
            EndAndResetEpisode(true, 50); // 结束这个回合
        }else
        {
            // Debug.Log($"Distance to Target: {DistanceToTarget}");
            AddReward(((-DistanceToTarget+TargetCollisionRange) * 0.50f)-5.0f);
        }

        // // 计算与目标的距离
        if (DistanceToTarget < LastDistanceToTarget)
        {
            AddReward(150.0f * (1/DistanceToTarget)); // 如果Agent靠近目标，给予小量正奖励;
        }else{
            AddReward(((-DistanceToTarget+TargetCollisionRange) * 0.50f)-5.0f);
        }

        LastDistanceToTarget = DistanceToTarget;


        if (this.transform.localPosition.y < 0)
        {
            EndAndResetEpisode(false,20);
        }

        // 更新计时器
        EpisodeTimer -= Time.deltaTime;
        if (EpisodeTimer <= 0)
        {
            EndAndResetEpisode(false,100);
        }else{
            AddReward(-Time.deltaTime * 800f); // -800 rewards per second
        }

        // 检查与Player2的碰撞
        float distanceToPlayer2 = Vector3.Distance(this.transform.localPosition, Player2.transform.localPosition);
        if (Env.IsPlayer2Win) // 假定距离阈值为2.5
        {
            Env.IsPlayer2Win = false;
            Global.AddPlayerSpeed(PlayerId, Global.SpeedBonus);
            Debug.Log($"Player1 Speed(+{Global.SpeedBonus}): {Global.GetPlayerSpeed(PlayerId)}");
            EndAndResetEpisode(false, 40);
        }
        else if(distanceToPlayer2 >= Player2CollisionRange && distanceToPlayer2 < 10.0f){  // Distance 2.5 ~ 10.0
            AddReward(-200.0f * (1/(distanceToPlayer2-Player2CollisionRange/1.5f)));
        }

        // PrintInfo();
    }

    private void ResetEnvironment()
    {
        // 重置Player1的位置和状态
        // transform.localPosition = startPosition; // 假定你有一个初始位置变量
        float rangeX = 20f;
        float baseX = 0f;
        float rangeZ = 25f;
        float baseZ = 0f;

        PlayerVelocity = Vector3.zero;
        IsGrounded = false;
        EpisodeTimer = MaxEpisodeTime;

        // Set a random position for Player1
        if(Global.Mode == 1){
            rangeZ = 20f;
            baseZ = -10f;
        }

        transform.localPosition = new Vector3(UnityEngine.Random.Range(-rangeX + baseX, rangeX + baseX),2, UnityEngine.Random.Range(-rangeZ + baseZ, rangeZ + baseZ));

        // Set a random position for Player2
        if(Global.Mode == 1){
            rangeZ = 10f;
            baseZ = 10f;
        }
        Player2.transform.localPosition = new Vector3(UnityEngine.Random.Range(-rangeX + baseX, rangeX + baseX),2, UnityEngine.Random.Range(-rangeZ + baseZ, rangeZ + baseZ));

        // Set a random position for Target
        if(Global.Mode == 1){
            baseZ = 20f;
        }
        Target.transform.localPosition = new Vector3(UnityEngine.Random.Range(-rangeX + baseX, rangeX + baseX),2, UnityEngine.Random.Range(-rangeZ + baseZ, rangeZ + baseZ));      
    }

    private void EndAndResetEpisode(bool isWin = false, float multiplier = 1.0f)
    {
        if(isWin){
            Global.PlayersRoundWin[PlayerId]++;
            Global.PlayersWinStreak[PlayerId]++;
            Env.IsPlayer1Win = true;
            Global.AddPlayerSpeed(PlayerId, -Global.SpeedBonus);
            Debug.Log($"Player1 Speed(-{Global.SpeedBonus}): {Global.GetPlayerSpeed(PlayerId)}");
            AddReward(100.0f * multiplier);
        }else{
            Global.PlayersWinStreak[PlayerId] = 0;
            Env.IsPlayer1Win = false;
            AddReward(-100.0f * multiplier);
        }
        Global.Round++;
        Speed = Global.GetPlayerSpeed(PlayerId);
        PrintInfo();
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
