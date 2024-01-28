using Unity.Netcode;

namespace LethalMDK.Network;

public abstract class NetEventBase : MessageReceiver
{
    public event Action<ulong> EventReceived;
    public event Action<ulong> EventReceivedFromClient;
    public event Action EventReceivedFromServer;

    protected override void OnReceiveMessage(ulong senderId, FastBufferReader buffer)
    {
        EventReceived?.Invoke(senderId);
        if (senderId == NetworkMessaging.ServerClientId)
            EventReceivedFromServer?.Invoke();
        else
            EventReceivedFromClient?.Invoke(senderId);
    }

    public void InvokeToServer(NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        uint? hash = GetHash();
        if (hash is null)
            throw new NullReferenceException("Cannot register event as it is uninitialized.");
        
        using var writer = NetworkMessaging.GetWriter(hash.Value, 0);
        NetworkMessaging.TrySendMessageToServer<NetworkEvent>(hash.Value, writer, delivery);
    }
    
    public void InvokeToClient(ulong clientId, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        uint? hash = GetHash();
        if (hash is null)
            throw new NullReferenceException("Cannot register event as it is uninitialized.");
        
        using var writer = NetworkMessaging.GetWriter(hash.Value, 0);
        NetworkMessaging.TrySendMessageToClient<NetworkEvent>(hash.Value, clientId, writer, delivery);
    }

    public void InvokeToAllClients(bool includeHost = false, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        uint? hash = GetHash();
        if (hash is null)
            throw new NullReferenceException("Cannot register event as it is uninitialized.");
        
        using var writer = NetworkMessaging.GetWriter(hash.Value, 0);
        NetworkMessaging.TrySendMessageToAllClients<NetworkEvent>(hash.Value, writer, includeHost, delivery);
    }
}