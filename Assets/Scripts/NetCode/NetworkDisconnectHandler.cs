using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class NetworkDisconnectHandler : MonoBehaviour
{
    [SerializeField] private UIManager m_UIManager;
    private void Awake()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // Esto se ejecuta cuando el cliente local se desconecta
            Debug.Log("Client disconnected. Returning to menu.");
            ReturnToMenu();
        }
    }

    private void OnServerStopped(bool wasShutdown)
    {
        Debug.Log($"Server stopped. Clean shutdown: {wasShutdown}. Returning to menu.");
        ReturnToMenu();
    }

    private void ReturnToMenu()
    {
        m_UIManager.bg.SetActive(true);
        // Carga la escena de menú (asegúrate de que la escena esté incluida en Build Settings)
       // SceneManager.LoadScene("MenuScene");
    }
}
