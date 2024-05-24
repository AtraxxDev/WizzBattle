using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Server Config")]
    [SerializeField] private Button server_Btn;
    [SerializeField] private Button host_Btn;
    [SerializeField] private Button client_Btn;
    [SerializeField] private TMP_InputField ipInputField; // Campo para la dirección IP
    [SerializeField] private TMP_InputField portInputField; // Campo para el puerto

    [Space(5)]
    [Header("UI")]
    public GameObject bg;
    public LobbyManager lobbyManager;



    private void Awake()
    {
        // Configurar valores predeterminados
        ipInputField.text = "127.0.0.1"; // IP local por defecto
        portInputField.text = "7777"; // Puerto por defecto

        // Suscribirse a los eventos de cambio de texto
        ipInputField.onValueChanged.AddListener(ValidateInput);
        portInputField.onValueChanged.AddListener(ValidateInput);

        server_Btn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            bg.SetActive(false);
        });

        host_Btn.onClick.AddListener(() =>
        {
            string ipAddress = ipInputField.text;
            string portText = portInputField.text;
            ushort port;

            if (!string.IsNullOrEmpty(ipAddress) && ushort.TryParse(portText, out port))
            {
                // Configurar la dirección IP y el puerto antes de iniciar el host
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, port);
                bg.SetActive(false);
                NetworkManager.Singleton.StartHost();
                lobbyManager.CreateLobby();

            }
            else
            {
                Debug.LogWarning("IP Address or port is invalid!");
            }
        });

        client_Btn.onClick.AddListener(() =>
        {
            string ipAddress = ipInputField.text;
            string portText = portInputField.text;
            ushort port;

            if (!string.IsNullOrEmpty(ipAddress) && ushort.TryParse(portText, out port))
            {
                // Configurar la dirección IP y el puerto antes de iniciar el cliente
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipAddress, port);
                bg.SetActive(false);
                NetworkManager.Singleton.StartClient();

            }
            else
            {
                Debug.LogWarning("IP Address or port is invalid!");
            }
        });

        // Validar la entrada inicialmente
        ValidateInput("");
    }

    private void ValidateInput(string input)
    {
        bool isIPValid = !string.IsNullOrEmpty(ipInputField.text);
        bool isPortValid = !string.IsNullOrEmpty(portInputField.text) && ushort.TryParse(portInputField.text, out _);

        host_Btn.interactable = isIPValid && isPortValid;
        client_Btn.interactable = isIPValid && isPortValid;
    }

  

}
