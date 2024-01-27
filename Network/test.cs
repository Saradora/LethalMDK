using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityMDK.Injection;
using UnityMDK.Logging;

namespace LethalMDK.Network;

[InjectToComponent(typeof(RoundManager))]
public class Test : MonoBehaviour
{
    private const string MessageName = "MyCustomNamedMessage";

    private readonly NetworkGlobalMessage<Guid> _testMessage = new(MessageName);

    private void Awake()
    {
        Log.Error($"Test start");

        if (NetworkMessaging.IsServerOrHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            _testMessage.MessageReceivedFromClient += ReceiveEventAsServer;
        }
        else
        {
            _testMessage.MessageReceivedFromServer += ReceiveEventAsClient;
        }
    }

    private IEnumerator Start()
    {
        if (NetworkMessaging.IsServerOrHost) yield break;
        yield return new WaitForSeconds(2f);
        _testMessage.SendToServer(Guid.NewGuid());
    }

    private void OnDestroy()
    {
        _testMessage.Dispose();
        // Whether server or not, unregister this.
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientConnectedCallback;
    }

    private void OnClientConnectedCallback(ulong obj)
    {
        _testMessage.SendToAllClients(Guid.NewGuid());
    }

    private void ReceiveEventAsClient(Guid guid)
    {
        Log.Error("Received guid from server! " + guid);
    }

    private void ReceiveEventAsServer(Guid guid, ulong clientId)
    {
        Log.Error($"Received event from client '{clientId}'! {guid}");
    }
}