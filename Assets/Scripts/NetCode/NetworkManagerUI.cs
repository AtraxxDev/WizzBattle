using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button server_Btn;
    [SerializeField] private Button host_Btn;
    [SerializeField] private Button client_Btn;

    private void Awake()
    {
        server_Btn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });

        host_Btn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        client_Btn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
