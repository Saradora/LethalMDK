using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityMDK.Injection;
using UnityMDK.Logging;

namespace LethalMDK.Network;

[InjectToComponent(typeof(LungProp))]
public class Test : MonoBehaviour
{
    private const string MessageName = "MyCustomNamedMessage";

    private ObjectNetEvent _startEvent;

    private void Awake()
    {
        Log.Error($"Test start");
        _startEvent = new ObjectNetEvent(MessageName, GetComponent<NetworkObject>());

        if (NetworkMessaging.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            _startEvent.EventReceivedFromClient += ReceiveEventAsServer;
        }
        else
        {
            _startEvent.EventReceivedFromServer += ReceiveEventAsClient;
        }
    }

    private IEnumerator Start()
    {
        if (NetworkMessaging.IsServer) yield break;
        yield return new WaitForSeconds(2f);
        int clientId = (int)NetworkManager.Singleton.LocalClientId;
        Log.Error($"{name} client {clientId} send!");
        _startEvent.InvokeToServer();
    }

    private void OnDestroy()
    {
        // Whether server or not, unregister this.
        _startEvent?.Dispose();
        if (!NetworkManager.Singleton) return;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientConnectedCallback;
    }

    private void OnClientConnectedCallback(ulong obj)
    {
        Log.Error($"{name} server send!");
        _startEvent.InvokeToAllClients();
    }

    private void ReceiveEventAsClient()
    {
        Log.Error($"{name} received from server!");
    }

    private void ReceiveEventAsServer(ulong clientId)
    {
        Log.Error($"{name} received from {clientId}!");
    }
}