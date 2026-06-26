using Unity.Netcode;
using UnityEngine;
using System;
using Unity.Collections;
using UnityEngine.InputSystem;

public class PlayerStats : NetworkBehaviour
{
    public int m_ScorePerPress = 1;
    public int m_StartHealth = 100;


     
    private readonly NetworkVariable<int> m_Score = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

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
        m_Score.OnValueChanged += HandleScoreChanged;
        m_Hp.OnValueChanged += HandleHealthChanged;
        m_DisplayName.OnValueChanged +=HandleNamehanged;
        

        ApplyScore(m_Score.Value);
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
        m_Score.OnValueChanged -= HandleScoreChanged;
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
            RequestScoreRpc(m_ScorePerPress);
        }

         
        if(IsServer && keyboard.hKey.wasPressedThisFrame)
        {
            m_Hp.Value -= 10;
        }
    }

    //score => rpc 변경
    [Rpc(SendTo.Server)]
    private void RequestScoreRpc(int amount, RpcParams rpcParams = default)
    {
        ulong sender = rpcParams.Receive.SenderClientId;
        // [검증용] 이 로그는 서버(Host 창)에서만 떠야 정상 — 실제 점수 변경은 서버가 수행
        Debug.Log($"[server] 점수 요청 수신: client {sender} +{amount} (Host 창에만 떠야 정상)");
        m_Score.Value += amount;
    }

    private void HandleScoreChanged(int prev, int cur)
    {
        ApplyScore(cur);
        // [검증용] 이 로그는 양쪽 창 모두 떠야 정상 — NetworkVariable이 복제됐다는 증거
        Debug.Log($"[playerstats] 스코어 {prev} -> {cur} | IsServer={IsServer}, IsOwner={IsOwner}, owner=client{OwnerClientId} (양쪽 창 모두 떠야 정상)");
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
