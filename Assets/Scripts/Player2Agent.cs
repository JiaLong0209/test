using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Player2Agent : Agent
{
    public int PlayerId = 1;
    public Environment Env;
    public Transform Target; // 目標位置，用於計算距離等
    public CharacterController Controller;
    public float Speed = 15f; // 移動速度
    public float JumpHeight = 3f; // 跳躍高度
    private Vector3 PlayerVelocity; // 玩家速度
    public bool IsGrounded; // 是否在地面上
    public LayerMask GroundLayer; // 地面層，用於檢測是否接觸地面
    public Transform GroundCheck; // 用於檢測是否在地面的Transform
    public float GroundDistance = 0.2f; // 與地面的檢測距離
    private float LastDistanceToTarget = float.MaxValue;
    public float MaxEpisodeTime = 30f; // 設定最大時間（秒）
    private float EpisodeTimer;
    private Vector3 StartPosition;
    public float TargetCollisionRange = 1.5f;

    public override void Initialize()
    {
        Controller = GetComponent<CharacterController>();
        StartPosition = transform.localPosition; // 將當前局部位置作為初始位置
        EpisodeTimer = MaxEpisodeTime; // 初始化計時器
    }

    public override void OnEpisodeBegin()
    {

        // 重置Agent的位置到初始位置、速度和計時器
        transform.localPosition = StartPosition;

        float rangeX = 20f;
        float baseX = 0f;
        float rangeZ = 25f;
        float baseZ = 0f;

        PlayerVelocity = Vector3.zero;
        IsGrounded = false;
        EpisodeTimer = MaxEpisodeTime;

        // Set a random position for Player2
        if(Global.Mode == 1){
            rangeZ = 10f;
            baseZ = 10f;
        }
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-rangeX + baseX, rangeX + baseX),2, UnityEngine.Random.Range(-rangeZ + baseZ, rangeZ + baseZ));
        LastDistanceToTarget = float.MaxValue;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 收集Agent的觀察值
        // 例如：Agent與目標的距離和方向
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
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

        Controller.Move(move * Speed * Time.deltaTime);

        if (actions.ContinuousActions[2] > 0.5f && IsGrounded)
        {
            PlayerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * Physics.gravity.y);
        }

        PlayerVelocity.y += Physics.gravity.y * Time.deltaTime;
        Controller.Move(PlayerVelocity * Time.deltaTime);

        // Target reward
        float DistanceToTarget = Vector3.Distance(this.transform.position, Target.position);
        if (DistanceToTarget < TargetCollisionRange) // 如果Agent collide 目標
        {
            Debug.Log("Player2 reach Player1!");
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

        if (Env.IsPlayer1Win){
            Env.IsPlayer1Win = false;
            Global.AddPlayerSpeed(PlayerId, Global.SpeedBonus);
            // Global.PrintPlayerSpeed(PlayerId, Global.SpeedBonus);
            EndAndResetEpisode(false,50);
        }

        if (this.transform.localPosition.y < 0)
        {
            EndAndResetEpisode(false,50);
        }

        // 更新計時器
        EpisodeTimer -= Time.deltaTime;
        if (EpisodeTimer <= 0)
        {
            EndAndResetEpisode(false,100);
        }else{
            AddReward(-Time.deltaTime * 500f); 
        }

    }

    private void ResetEnvironment()
    {
        PlayerVelocity = Vector3.zero;
        IsGrounded = false;
    }

    private void EndAndResetEpisode(bool isWin = false, float multiplier = 1.0f)
    {
        if(isWin){
            Global.PlayersRoundWin[PlayerId]++;
            Global.PlayersWinStreak[PlayerId]++;
            Env.IsPlayer2Win = true;
            Global.AddPlayerSpeed(PlayerId, -Global.SpeedBonus);
            // Global.PrintPlayerSpeed(PlayerId, -Global.SpeedBonus);
            AddReward(100.0f * multiplier);
        }else{
            Global.PlayersWinStreak[PlayerId] = 0;
            Env.IsPlayer2Win = false;
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
