using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Player1Agent : Agent
{
    public int PlayerId = 0;
    public Environment Env;
    public Transform Target; // 目標位置，用於計算距離等
    private CharacterController controller;
    public float Speed = 15f; // 移動速度
    public float JumpHeight = 3f; // 跳躍高度
    private Vector3 PlayerVelocity; // 玩家速度
    private bool IsGrounded; // 是否在地面上
    public LayerMask GroundLayer; // 地面層，用於檢測是否接觸地面
    public Transform GroundCheck; // 用於檢測是否在地面的Transform
    public float GroundDistance = 0.2f; // 與地面的檢測距離
    private float LastDistanceToTarget = float.MaxValue;
    public float MaxEpisodeTime = 30f; // 設定最大時間（秒）
    private float EpisodeTimer;
    public GameObject Player2; // 需要在編輯器中設定
    private Vector3 startPosition;
    public float TargetCollisionRange = 3.0f;
    public float Player2CollisionRange = 2.5f;
    public bool activeRandomPlayerPosition = false;

    public override void Initialize()
    {
        controller = GetComponent<CharacterController>();
        startPosition = transform.localPosition; // 將當前局部位置作為初始位置
        EpisodeTimer = MaxEpisodeTime; // 初始化計時器
    }

    public override void OnEpisodeBegin()
    {
        // 重置Agent的位置到初始位置、速度和計時器
        transform.localPosition = startPosition;

        // Debug.Log($" Message Player1: \n1.position: {transform.position}\nstart_position: {startPosition}\n2.position: {Player2.transform.position}!!!");
        PlayerVelocity = Vector3.zero;
        EpisodeTimer = MaxEpisodeTime; // 重置計時器
        LastDistanceToTarget = float.MaxValue;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 收集Agent的觀察值
        // 例如：Agent與目標的距離和方向
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
        // 執行動作
        var move = Vector3.zero;
        move.x = actions.ContinuousActions[0];
        move.z = actions.ContinuousActions[1];

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
        if (DistanceToTarget < TargetCollisionRange) // 如果Agent collide 目標
        {
            Debug.Log("Player1 reach target!");
            EndAndResetEpisode(true, 50); // 結束這個回合
        }else
        {
            // Debug.Log($"Distance to Target: {DistanceToTarget}");
            AddReward(((-DistanceToTarget+TargetCollisionRange) * 0.50f)-5.0f);
        }

        // // 計算與目標的距離
        if (DistanceToTarget < LastDistanceToTarget)
        {
            AddReward(150.0f * (1/DistanceToTarget)); // 如果Agent靠近目標，給予小量正獎勵;
        }else{
            AddReward(((-DistanceToTarget+TargetCollisionRange) * 0.50f)-5.0f);
        }

        LastDistanceToTarget = DistanceToTarget;


        if (this.transform.localPosition.y < 0)
        {
            EndAndResetEpisode(false,20);
        }

        // 更新計時器
        EpisodeTimer -= Time.deltaTime;
        if (EpisodeTimer <= 0)
        {
            EndAndResetEpisode(false,100);
        }else{
            AddReward(-Time.deltaTime * 800f); // -800 rewards per second
        }

        // 檢查與Player2的碰撞
        float distanceToPlayer2 = Vector3.Distance(this.transform.localPosition, Player2.transform.localPosition);
        if (Env.IsPlayer2Win) // 假定距離閾值為2.5
        {
            Env.IsPlayer2Win = false;
            Global.AddPlayerSpeed(PlayerId, Global.SpeedBonus);
            // Global.PrintPlayerSpeed(PlayerId, Global.SpeedBonus);
            EndAndResetEpisode(false, 40);
        }
        else if(distanceToPlayer2 >= Player2CollisionRange && distanceToPlayer2 < 10.0f){  // 距離 2.5 ~ 10.0
            AddReward(-200.0f * (1/(distanceToPlayer2-Player2CollisionRange/1.5f)));
        }

    }

    private void ResetEnvironment()
    {
        // 重置Player1的位置和狀態
        // transform.localPosition = startPosition; // 假定你有一個初始位置變數
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
            // Global.PrintPlayerSpeed(PlayerId, -Global.SpeedBonus);
            AddReward(100.0f * multiplier);
        }else{
            Global.PlayersWinStreak[PlayerId] = 0;
            Env.IsPlayer1Win = false;
            AddReward(-100.0f * multiplier);
        }
        Speed = Global.GetPlayerSpeed(PlayerId);
        // Global.PrintRoundInfo(PlayerId, GetCumulativeReward());
        Env.EndPlayerEpisode(PlayerId, GetCumulativeReward());
        EndEpisode();
        ResetEnvironment(); // 確保在調用EndEpisode後立即調用
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetButton("Jump") ? 1f : 0f; // 注意: 使用GetButton而不是GetButtonDown
    }

}
