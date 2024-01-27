using System.Reflection;
using Unity.Collections;
using Unity.Netcode;

namespace LethalMDK.Network;

public class NetworkGlobalEvent : IDisposable
{
    private readonly string _messageName;
    private readonly uint _hash;

    private static readonly FastBufferWriter _dummyWriter = new(0, Allocator.Persistent);

    public event Action<ulong> EventReceivedFromClient;
    public event Action EventReceivedFromServer;

    public NetworkGlobalEvent(string name, bool assemblySpecific = false)
    {
        _messageName = name;
        if (assemblySpecific) _messageName = Assembly.GetCallingAssembly().GetName().Name + "+" + _messageName;
        _hash = _messageName.Hash32();
        
        NetworkMessaging.RegisterEvent<NetworkEvent>(_hash, _messageName, ReceiveMessage);
    }

    public void Dispose()
    {
        NetworkMessaging.UnregisterEvent(_hash, ReceiveMessage);
    }

    private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        if (senderId == NetworkMessaging.ServerClientId)
            EventReceivedFromServer?.Invoke();
        else
            EventReceivedFromClient?.Invoke(senderId);
    }

    public void InvokeToServer()
    {
        NetworkMessaging.TrySendMessageToServer<NetworkEvent>(_hash, _dummyWriter);
    }
    
    public void InvokeToClient(ulong clientId)
    {
        NetworkMessaging.TrySendMessageToClient<NetworkEvent>(_hash, clientId, _dummyWriter);
    }

    public void InvokeToAllClients(bool includeHost = false)
    {
        NetworkMessaging.TrySendMessageToAllClients<NetworkEvent>(_hash, _dummyWriter, includeHost);
    }
}