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

    private NetworkClientMessage<Guid> _testMessageClient;
    private NetworkServerMessage<Guid> _testMessageServer;

    private void Awake()
    {
        _testMessageClient = new NetworkClientMessage<Guid>(MessageName);
        _testMessageServer = new NetworkServerMessage<Guid>(MessageName);
        
        Log.Error($"Start wesh");

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            _testMessageClient.MessageReceived += ReceiveMessageAsServer;
        }
        else
        {
            _testMessageServer.MessageReceived += ReceiveMessageAsClient;
        }
    }

    private IEnumerator Start()
    {
        if (NetworkManager.Singleton.IsServer) yield break;
        yield return new WaitForSeconds(2f);
        _testMessageClient.SendToServer(Guid.NewGuid());
    }

    private void OnDestroy()
    {
        _testMessageClient.Dispose();
        _testMessageServer.Dispose();
        // Whether server or not, unregister this.
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientConnectedCallback;
    }

    private void OnClientConnectedCallback(ulong obj)
    {
        _testMessageServer.SendToAllClients(Guid.NewGuid());
    }

    private void ReceiveMessageAsClient(Guid guid)
    {
        Log.Error(guid);
    }

    private void ReceiveMessageAsServer(Guid guid, ulong clientId)
    {
        Log.Error(guid);
    }
}