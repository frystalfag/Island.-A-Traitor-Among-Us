using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Unity.VisualScripting;

public class MainMenuManager : MonoBehaviour
{
    // пока что не нужно анкоментить, тут все ок
    /*[SerializeField] private Button server;*/
    [SerializeField] private Button client;
    [SerializeField] private Button host;
    [SerializeField] private Button playOnline;
    
    [SerializeField] private GameObject _firstCanvas;
    [SerializeField] private GameObject _secondCanvas;
    [SerializeField] private GameObject _uiGO;
    
    [SerializeField] private TMP_InputField nicknameInputField;
    
    public static string playerName = null;
    
    private void Start()
    {
        /*server.onClick.AddListener(StartServer);*/
        client.onClick.AddListener(StartClient);
        host.onClick.AddListener(StartHost);
        playOnline.onClick.AddListener(ShowSecondCanvas);
        _uiGO.SetActive(true);
    }
    public void ShowFirstCanvas()
    {
        _firstCanvas.SetActive(true);
        _secondCanvas.SetActive(false);
    }

    public void ShowSecondCanvas()
    {
        _firstCanvas.SetActive(false);
        _secondCanvas.SetActive(true);
    }
    
    private void StartServer()
    {
        NetworkManager.singleton.StartServer();
        Debug.Log("Server is running.");
    }

    private void StartClient()
    {
        NetworkManager.singleton.StartClient();
        Debug.Log("Client Started");
    }

    private void StartHost()
    {
        NetworkManager.singleton.StartHost();
        Debug.Log("Host started");
    }
}
