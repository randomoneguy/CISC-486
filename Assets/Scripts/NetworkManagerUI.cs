using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_InputField ipAddressInput;
    [SerializeField] private UnityEngine.UI.Button hostButton;
    [SerializeField] private UnityEngine.UI.Button clientButton;
    [SerializeField] private UnityEngine.UI.Button disconnectButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI hostIPText; // Optional: Display host's IP address
    
    private string localIPAddress = "";
    private string publicIPAddress = "";

    private void Start()
    {
        // Set default IP to localhost
        if (ipAddressInput != null)
        {
            ipAddressInput.text = "127.0.0.1";
        }

        // Setup button listeners
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(StartHost);
        }

        if (clientButton != null)
        {
            clientButton.onClick.AddListener(StartClient);
        }

        if (disconnectButton != null)
        {
            disconnectButton.onClick.AddListener(Disconnect);
            disconnectButton.gameObject.SetActive(false);
        }

        // Subscribe to network events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        // Get local IP address
        GetLocalIPAddress();
        
        // Optionally get public IP (async, may take a moment)
        StartCoroutine(GetPublicIPAddress());

        UpdateStatus("Ready to connect");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
    }

    public void StartHost()
    {
        // Configure server to listen on all interfaces
        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.ServerListenAddress = "0.0.0.0"; // Listen on all interfaces
            transport.ConnectionData.Port = 7777;
        }

        if (NetworkManager.Singleton.StartHost())
        {
            UpdateStatus("Host started - Waiting for clients...");
            
            // Display IP information for host to share
            string ipInfo = $"Local IP: {localIPAddress}\n";
            if (!string.IsNullOrEmpty(publicIPAddress))
            {
                ipInfo += $"Public IP: {publicIPAddress}";
            }
            else
            {
                ipInfo += "Getting public IP...";
            }
            
            if (hostIPText != null)
            {
                hostIPText.text = $"Share this IP with client:\n{ipInfo}";
                hostIPText.gameObject.SetActive(true);
            }
            
            // Also log to console for easy copy-paste
            Debug.Log($"[HOST] Share this IP with client:");
            Debug.Log($"[HOST] Local IP: {localIPAddress}");
            if (!string.IsNullOrEmpty(publicIPAddress))
            {
                Debug.Log($"[HOST] Public IP: {publicIPAddress}");
            }
            
            if (menuPanel != null) menuPanel.SetActive(false);
            if (disconnectButton != null) disconnectButton.gameObject.SetActive(true);
        }
        else
        {
            UpdateStatus("Failed to start host");
        }
    }

    public void StartClient()
    {
        string ipAddress = ipAddressInput != null ? ipAddressInput.text : "127.0.0.1";
        
        // Get Unity Transport and set connection data
        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        if (transport != null)
        {
            transport.ConnectionData.Address = ipAddress;
            transport.ConnectionData.Port = 7777;
        }

        if (NetworkManager.Singleton.StartClient())
        {
            UpdateStatus($"Connecting to {ipAddress}...");
            if (menuPanel != null) menuPanel.SetActive(false);
            if (disconnectButton != null) disconnectButton.gameObject.SetActive(true);
        }
        else
        {
            UpdateStatus("Failed to start client");
        }
    }

    public void Disconnect()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        UpdateStatus("Disconnected");
        if (menuPanel != null) menuPanel.SetActive(true);
        if (disconnectButton != null) disconnectButton.gameObject.SetActive(false);
    }

    private void OnServerStarted()
    {
        UpdateStatus("Server started - Waiting for clients...");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            UpdateStatus($"Client {clientId} connected");
        }
        else
        {
            UpdateStatus("Connected to server!");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            UpdateStatus($"Client {clientId} disconnected");
        }
        else
        {
            UpdateStatus("Disconnected from server");
            if (menuPanel != null) menuPanel.SetActive(true);
            if (disconnectButton != null) disconnectButton.gameObject.SetActive(false);
        }
    }


    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log($"[NetworkManagerUI] {message}");
    }

    private void GetLocalIPAddress()
    {
        try
        {
            // Get local IP address (prefer IPv4)
            string hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            
            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(address))
                {
                    localIPAddress = address.ToString();
                    break;
                }
            }
            
            // Fallback: try to get from network interfaces
            if (string.IsNullOrEmpty(localIPAddress))
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || 
                        ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork && 
                                !IPAddress.IsLoopback(ip.Address))
                            {
                                localIPAddress = ip.Address.ToString();
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(localIPAddress)) break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not get local IP address: {e.Message}");
            localIPAddress = "Unknown";
        }
    }

    private IEnumerator GetPublicIPAddress()
    {
        // Use a free service to get public IP
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get("https://api.ipify.org"))
        {
            yield return www.SendWebRequest();
            
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                publicIPAddress = www.downloadHandler.text.Trim();
                Debug.Log($"[NetworkManagerUI] Public IP: {publicIPAddress}");
                
                // Update host IP text if host is already started
                if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost && hostIPText != null)
                {
                    hostIPText.text = $"Share this IP with client:\nLocal IP: {localIPAddress}\nPublic IP: {publicIPAddress}";
                }
            }
            else
            {
                Debug.LogWarning($"Could not get public IP: {www.error}");
            }
        }
    }
}

