using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ScoreBoard : NetworkBehaviour
{
    private NetworkList<ScoreEntry> entries;

    private void Awake()
    {
        entries = new NetworkList<ScoreEntry>();   
    }

    public override void OnNetworkSpawn()
    {
        entries.OnListChanged += OnEntriesChanged;

        if (IsServer)
        {
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                AddEntryForClient(id);        
            }
       
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        RedrawAll();
    }
    public override void OnNetworkDespawn()
    {
        entries.OnListChanged -= OnEntriesChanged;
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        AddEntryForClient(clientId);
    }
    private void AddEntryForClient(ulong clientId)
    {
        for(int i = 0; i < entries.Count; ++i)
        {
            if(entries[i].ClientId == clientId) return;
        }
        ScoreEntry entry = new ScoreEntry
        {
            ClientId = clientId,
            Name = new FixedString32Bytes($"Player{clientId}"),
            Score = 0,
        };

        entries.Add(entry);
    }

    private void OnEntriesChanged(NetworkListEvent<ScoreEntry> changedEvent)
    {
        Debug.Log($"[scoreBoard] {changedEvent.Type} -> {entries.Count}");
        RedrawAll();
    }
    private void RedrawAll()
    {
        for(int i = 0; i < entries.Count; ++i)
        {
            Debug.Log($"{entries[i].ClientId} / {entries[i].Name} / {entries[i].Score}");
        }
    }
}
