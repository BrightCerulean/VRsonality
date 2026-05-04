using Unity.Services.Core;
using Unity.Services.Vivox;
using Unity.Services.Authentication;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class VivoxManager : MonoBehaviour
{
    [SerializeField] string channelName = "GameChannel";
    private bool _vivoxReady = false;

    async void Start()
    {
        // Initialize Unity Services and Vivox — but do NOT join channel yet
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            await VivoxService.Instance.InitializeAsync();
            _vivoxReady = true;
            Debug.Log("Vivox initialized and ready.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Vivox init failed: " + e.Message);
        }

        // Listen for NGO connection — join channel only after this fires
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    void OnClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        Debug.Log("NGO connected — joining Vivox channel.");
        StartCoroutine(JoinChannelWhenReady());
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        LeaveChannel();
    }

    IEnumerator JoinChannelWhenReady()
    {
        // Wait for Vivox to finish initializing if it hasn't yet
        float timeout = 10f;
        float elapsed = 0f;
        while (!_vivoxReady)
        {
            elapsed += Time.deltaTime;
            if (elapsed > timeout)
            {
                Debug.LogError("Vivox init timeout.");
                yield break;
            }
            yield return null;
        }

        JoinChannel();
    }

    async void JoinChannel()
    {
        try
        {
            await VivoxService.Instance.JoinGroupChannelAsync(
                channelName,
                ChatCapability.AudioOnly
            );
            Debug.Log("Joined Vivox channel: " + channelName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Vivox join failed: " + e.Message);
        }
    }

    async void LeaveChannel()
    {
        try
        {
            await VivoxService.Instance.LeaveAllChannelsAsync();
            Debug.Log("Left Vivox channel.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Vivox leave failed: " + e.Message);
        }
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        LeaveChannel();
    }
}