using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;


public class UIManager : MonoBehaviour
{
    [Header("Authentication")]
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button authenticate_Btn;
    [SerializeField] private GameObject authenticationPanel;
    [SerializeField] private GameObject ipConfigPanel;

    [Header("Server Config")]
    [SerializeField] private Button server_Btn;
    [SerializeField] private Button host_Btn;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;
    public GameObject bg;

    [Header("Join Lobby")]
    [SerializeField] private GameObject joinLobbyPanel;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private RectTransform lobbyListContent;
    [SerializeField] private GameObject lobbyListItemPrefab;

    [Header("Code Lobby")]
    [SerializeField] private GameObject lobbyCodePanel;
    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private Button joinLobbyButton;


    [Space(5)]
    [Header("Managers")]
    public LobbyManager lobbyManager;

    private void Awake()
    {
        // Configurar valores predeterminados
        ipInputField.text = "127.0.0.1"; // IP local por defecto
        portInputField.text = "7777"; // Puerto por defecto

        // Suscribirse a los eventos de cambio de texto
        ipInputField.onValueChanged.AddListener(ValidateInput);
        portInputField.onValueChanged.AddListener(ValidateInput);

        authenticate_Btn.onClick.AddListener(OnAuthenticateButtonClicked);
        server_Btn.onClick.AddListener(OnServerButtonClicked);
        host_Btn.onClick.AddListener(OnHostButtonClicked);
        joinButton.onClick.AddListener(OnJoinButtonClicked);
        quickJoinButton.onClick.AddListener(OnQuickJoinButtonClicked);

        // Validar la entrada inicialmente
        ValidateInput("");
    }

    private void ValidateInput(string input)
    {
        bool isIPValid = !string.IsNullOrEmpty(ipInputField.text);
        bool isPortValid = !string.IsNullOrEmpty(portInputField.text) && ushort.TryParse(portInputField.text, out _);

        host_Btn.interactable = isIPValid && isPortValid;
        joinButton.interactable = isIPValid && isPortValid;
    }

    private void OnAuthenticateButtonClicked()
    {
        string username = usernameInputField.text;
        if (!string.IsNullOrEmpty(username))
        {
            lobbyManager.SetPlayerName(username);
            authenticationPanel.SetActive(false);
            ipConfigPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Username cannot be empty!");
        }
    }

    private void OnServerButtonClicked()
    {
        NetworkManager.Singleton.StartServer();
        bg.SetActive(false);
    }

    private void OnHostButtonClicked()
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
    }

    private void OnClientButtonClicked()
    {
       
    }

    private void OnJoinButtonClicked()
    {
        // Muestra el panel de unirse por código
        joinLobbyPanel.SetActive(true);
        // Oculta el panel de configuración de IP
        ipConfigPanel.SetActive(false);
        // Llama al método del LobbyManager para listar los lobbies disponibles
        lobbyManager.ListLobbies(ShowLobbyList);
 
    }

    private void OnQuickJoinButtonClicked()
    {
        // Implementa la funcionalidad para unirse rápidamente a un lobby
        lobbyManager.QuickJoinLobby();
    }


    // Método que se llama cuando se hace clic en un botón de unirse al lobby en la lista
    private void OnJoinLobbyButtonClicked(Lobby lobby)
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
            Debug.LogWarning("Invalid IP Address or Port!");
        }

        string lobbyCode = lobbyCodeInputField.text;
        if (!string.IsNullOrEmpty(lobbyCode))
        {

            // Llama al método del LobbyManager para unirse al lobby con el código ingresado
            lobbyManager.JoinLobbybyCode(lobbyCode);
        }
        else
        {
            Debug.LogWarning("Lobby code cannot be empty!");
        }
    }

    // Método para mostrar la lista de lobbies disponibles
    public void ShowLobbyList(List<Lobby> lobbies)
    {
        // Elimina cualquier elemento anterior en la lista
        foreach (Transform child in lobbyListContent)
        {
            Destroy(child.gameObject);
        }

        // Crea un nuevo elemento de lista para cada lobby y lo muestra
        foreach (var lobby in lobbies)
        {
            GameObject listItem = Instantiate(lobbyListItemPrefab, lobbyListContent);
            // Configura el texto del elemento de lista para mostrar información sobre el lobby
            TextMeshProUGUI lobbyText = listItem.GetComponentInChildren<TextMeshProUGUI>();
            lobbyText.text = $"{lobby.Name} ({lobby.Players.Count}/{lobby.MaxPlayers})";

            // Configura el evento de clic para unirse al lobby al hacer clic en el elemento de lista
            Button button = listItem.GetComponent<Button>();
            button.onClick.AddListener(() => ShowLobbyCodePanel(lobby));
        }
    }

    private void ShowLobbyCodePanel(Lobby lobby)
    {
        // Desactiva el panel actual
        joinLobbyPanel.SetActive(false);
        // Activa el panel para ingresar el código del lobby
        lobbyCodePanel.SetActive(true);

        // Configura el evento de clic para unirse al lobby con el código ingresado
        joinLobbyButton.onClick.RemoveAllListeners(); // Elimina cualquier listener existente
        joinLobbyButton.onClick.AddListener(() => OnJoinLobbyButtonClicked(lobby));
    }

}
