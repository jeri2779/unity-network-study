using Unity.Netcode;
using UnityEngine;
using System;
using Unity.Collections;
using UnityEngine.InputSystem;

public class PlayerStats : NetworkBehaviour
{
    public int m_ScorePerPress = 1;
    public int m_StartHealth = 100;


     
    // private readonly NetworkVariable<int> m_Score = new NetworkVariable<int>(
    //     0,
    //     NetworkVariableReadPermission.Everyone,
    //     NetworkVariableWritePermission.Server
    // );

     private readonly NetworkVariable<int> m_Hp = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private readonly NetworkVariable<FixedString32Bytes> m_DisplayName = new NetworkVariable<FixedString32Bytes>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        //m_Score.OnValueChanged += HandleScoreChanged;
        m_Hp.OnValueChanged += HandleHealthChanged;
        m_DisplayName.OnValueChanged +=HandleNamehanged;
        

        ApplyScore(m_Score);
        ApplyHealth(m_Hp.Value);
        ApplyName(m_DisplayName.Value);

        if(IsServer)
        {
            m_Hp.Value = m_StartHealth;
        }
        if(IsOwner && m_DisplayName.Value.Length == 0)
        {
            m_DisplayName.Value = new FixedString32Bytes($"Player{OwnerClientId}");
        }
        
    }
    public override void OnNetworkDespawn()
    {
        //m_Score.OnValueChanged -= HandleScoreChanged;
        m_Hp.OnValueChanged -= HandleHealthChanged;
        m_DisplayName.OnValueChanged -=HandleNamehanged;
    }

    private void Update()
    {
        if(!IsOwner) return;
        

        Keyboard keyboard = Keyboard.current;
        if(keyboard == null) return;

         
        if(keyboard.eKey.wasPressedThisFrame)
        {
            RequestScoreRpc();
        }

         
        if(IsServer && keyboard.hKey.wasPressedThisFrame)
        {
            m_Hp.Value -= 10;
        }
        if(!IsServer)
        {
            RequestCurrentScoreRpc();
        }
    }
    int m_Score;

    //score => rpc 변경
    [Rpc(SendTo.Server)]
    private void RequestScoreRpc(RpcParams rpcParams = default)
    {
        ulong sender = rpcParams.Receive.SenderClientId;
        // Debug.Log($"[server] 점수 요청 수신: client {sender} +{amount}");
        // m_Score.Value += amount;

        if(sender != OwnerClientId)
        {
            return;
        }
        m_Score += m_ScorePerPress;
        BroadCastScoreRpc(m_Score);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadCastScoreRpc(int newScore)
    {
        m_Score = newScore;
        ApplyScore(m_Score);
    }

    [Rpc(SendTo.Server)]
    private void RequestCurrentScoreRpc(RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        SendCurrentScoreRpc(m_Score,RpcTarget.Single(senderClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendCurrentScoreRpc(int currentScore, RpcParams rpcParams)
    {
        ulong  senderClientId = rpcParams.Receive.SenderClientId;
        ApplyScore(m_Score);
    }


    private void HandleScoreChanged(int prev, int cur)
    {
        ApplyScore(cur);
        
        Debug.Log($"[playerstats] 스코어 {prev} -> {cur} | IsServer={IsServer}, IsOwner={IsOwner}, owner=client{OwnerClientId}");
    }
 
    private void HandleHealthChanged(int prev, int cur)
    {
        ApplyHealth(cur);
        Debug.Log($"[playerstats] HP 변경 {prev} -> {cur}");
    }
     private void HandleNamehanged(FixedString32Bytes prev, FixedString32Bytes cur)
    {
          ApplyName(cur);
        Debug.Log($"[playerstats] 이름 변경 {prev} -> {cur}");
    }

    private void ApplyScore(int value)
    {
        
    }
    private void ApplyHealth(int value)
    {
        
    }
    private void ApplyName(FixedString32Bytes value)
    {
        
    }

 
}
