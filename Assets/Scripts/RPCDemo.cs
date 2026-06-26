using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class RPCDemo : NetworkBehaviour
{
    public int m_ActionId = 1;

    private void Update()
    {
        if(!IsOwner) return;

        Keyboard keyboard = Keyboard.current;
        if(keyboard.fKey.wasPressedThisFrame)
        {
            RequestActionRpc(m_ActionId);
        }
    }
    [Rpc(SendTo.Server)]
    private void RequestActionRpc(int actionId, RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        if(actionId <= 0)
        {
            Debug.LogWarning($"[server]  wrong actionid = {actionId} / cliendId = {senderClientId}");
            return;
        }
        Debug.Log($"[server] clientid = {senderClientId} / action = {actionId}");

        AnnounceActionRpc(senderClientId, actionId);

        AckRpc(actionId, RpcTarget.Single(senderClientId,RpcTargetUse.Temp));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AnnounceActionRpc(ulong actionClientId, int actionId)
    {
        bool isMine = actionClientId == NetworkManager.LocalClientId;
        Debug.Log($"[client/host] clientid = {actionClientId} / action = {actionId} / isMine? {isMine}");
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void AckRpc(int actionId, RpcParams rpcParams)
    {
         
        Debug.Log($"[client] server response: action{actionId}");
    }
    


}