 using Unity.Netcode;
using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{

    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        if(networkManager == null)
        {
            networkManager = NetworkManager.Singleton;
        }
    }
    void OnGUI()
    {
        if (GUILayout.Button("Host"))  NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client"))
        {
            // 로컬 테스트: 127.0.0.1, 기본 포트 7777
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            transport.SetConnectionData("127.0.0.1", 7777);
            NetworkManager.Singleton.StartClient();
        }
        if (GUILayout.Button("Shutdown")) NetworkManager.Singleton.Shutdown();
    }
}