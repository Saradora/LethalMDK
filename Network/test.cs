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

    private Guid _syncedGuid;

    private ObjectNetMessage<Guid> _guidSyncEvent;

    private void Awake()
    {
        Log.Error($"Test start");
        _guidSyncEvent = new (MessageName, GetComponent<NetworkObject>());

        if (NetworkMessaging.IsServer)
        {
            _syncedGuid = Guid.NewGuid();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }
        else
        {
            _guidSyncEvent.MessageReceivedFromServer += ReceiveGuidSync;
        }
    }

    private IEnumerator Start()
    {
        if (NetworkMessaging.IsServer) yield break;
        yield return new WaitForSeconds(2f);
        int clientId = (int)NetworkManager.Singleton.LocalClientId;
    }

    private void OnDestroy()
    {
        // Whether server or not, unregister this.
        _guidSyncEvent?.Dispose();
        if (!NetworkManager.Singleton) return;
        Log.Error($"Test destroy");
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
    }

    private void OnClientConnectedCallback(ulong client)
    {
        Log.Error($"server {_syncedGuid} sent to {client}!");
        _guidSyncEvent.SendToClient(_syncedGuid, client);
    }

    private void ReceiveGuidSync(Guid syncedGuid)
    {
        _syncedGuid = syncedGuid;
        Log.Error($"{_syncedGuid} received from server!");
    }
}