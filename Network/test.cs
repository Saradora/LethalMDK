using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityMDK.Injection;
using UnityMDK.Logging;

namespace LethalMDK.Network;

[InjectToComponent(typeof(RoundManager))]
public class Test : MonoBehaviour
{
    [Tooltip("The name identifier used for this custom message handler.")]
    public string MessageName = "MyCustomNamedMessage";

    private NetworkEvent _testMessage;

    private void Awake()
    {
        _testMessage = new NetworkEvent(MessageName);
        
        Log.Error($"Test start");

        if (Messaging.IsServerOrHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            _testMessage.MessageReceivedFromClient += ReceiveMessageAsServer;
        }
        else
        {
            _testMessage.MessageReceivedFromServer += ReceiveMessageAsClient;
        }
    }

    private IEnumerator Start()
    {
        if (Messaging.IsServerOrHost) yield break;
        yield return new WaitForSeconds(2f);
        _testMessage.InvokeToServer();
    }

    private void OnDestroy()
    {
        _testMessage.Dispose();
        // Whether server or not, unregister this.
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientConnectedCallback;
    }

    private void OnClientConnectedCallback(ulong obj)
    {
        _testMessage.InvokeToAllClients();
    }

    private void ReceiveMessageAsClient()
    {
        Log.Error("Received event from server!");
    }

    private void ReceiveMessageAsServer(ulong clientId)
    {
        Log.Error("Received event from client: " + clientId);
    }
}