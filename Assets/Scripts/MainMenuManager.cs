using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Collections;

public class MainMenuManager : NetworkBehaviour
{
    [SerializeField] private NetworkManager networkManager;

    private NetworkDiscovery _networkDiscovery;
    private bool _serverFound = false;

    private void Awake()
    {
        _networkDiscovery = networkManager.GetComponent<NetworkDiscovery>();
    }

    private void OnEnable()
    {
        _networkDiscovery.OnServerFound.AddListener(OnServerFound);
    }

    public void StartGame()
    {
        gameObject.SetActive(true);

        _networkDiscovery.StartDiscovery();

        StartCoroutine(WaitForServerResponse());
    }

    private IEnumerator WaitForServerResponse()
    {
        float timeout = 2f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            if (_serverFound) 
                yield break;

            elapsed += Time.deltaTime;

            yield return null;
        }

        networkManager.StartHost();
        _networkDiscovery.AdvertiseServer();
    }

    private void OnServerFound(ServerResponse info)
    {
        _serverFound = true;
        _networkDiscovery.StopDiscovery();

        networkManager.networkAddress = info.uri.Host;
        networkManager.StartClient();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
