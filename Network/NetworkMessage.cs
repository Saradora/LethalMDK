using Unity.Collections;
using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public static class Messaging
{
    public static CustomMessagingManager Manager => NetworkManager.Singleton.CustomMessagingManager;

    public static bool IsServerOrHost => NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost;

    private static readonly List<ulong> _listWithoutHost = new();

    public static ulong ServerClientId => NetworkManager.ServerClientId;

    public static IReadOnlyList<ulong> ClientIds
    {
        get
        {
            var ids = NetworkManager.Singleton.ConnectedClientsIds;
            _listWithoutHost.Clear();
            ulong serverId = NetworkManager.ServerClientId;
            foreach (var clientId in ids)
            {
                if (clientId == serverId)
                {
                    Log.Warning($"Skipping host: {clientId}");
                    continue;
                }
                _listWithoutHost.Add(clientId);
            }
            return _listWithoutHost;
        }
    }
}

public class NetworkEvent
{
    public void SendMessage()
    {
        
    }
}

public abstract class NetworkMessage<T> : IDisposable where T : unmanaged, IEquatable<T>
{
    protected readonly string messageName;

    protected NetworkMessage(string name)
    {
        messageName = name;
        if (!NetworkManager.Singleton)
            throw new NullReferenceException("Network Singleton hasn't yet been instantied, you need to construct the NetworkMessage later.");
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(messageName, ReceiveMessage);
    }

    protected abstract void ReceiveMessage(ulong senderId, FastBufferReader messagePayload);

    public void Dispose()
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.CustomMessagingManager is not null)
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(messageName);
    }
}

public class NetworkClientMessage<T> : NetworkMessage<T> where T : unmanaged, IEquatable<T>
{
    public NetworkClientMessage(string name) : base(name)
    {
    }

    public event Action<T, ulong> MessageReceived;

    protected override void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        if (!Messaging.IsServerOrHost) return;
        
        var receivedMessageContent = new ForceNetworkSerializeByMemcpy<T>(new T());
        messagePayload.ReadValueSafe(out receivedMessageContent);
        MessageReceived?.Invoke(receivedMessageContent.Value, senderId);
    }

    public void SendToServer(T value)
    {
        if (Messaging.IsServerOrHost) return;
        
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(value);
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        using (writer)
        {
            writer.WriteValueSafe(messageContent);
            Log.Warning($"Send message: {value}");
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(messageName, Messaging.ServerClientId, writer);
        }
    }
}

public class NetworkServerMessage<T> : NetworkMessage<T> where T : unmanaged, IEquatable<T>
{
    public NetworkServerMessage(string name) : base(name)
    {
    }

    public event Action<T> MessageReceived;

    protected override void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        if (Messaging.IsServerOrHost) return;
        
        var receivedMessageContent = new ForceNetworkSerializeByMemcpy<T>(new T());
        messagePayload.ReadValueSafe(out receivedMessageContent);
        MessageReceived?.Invoke(receivedMessageContent.Value);
    }

    public void SendToClient(T value, ulong clientId)
    {
        if (!Messaging.IsServerOrHost) return;
        if (clientId == Messaging.ServerClientId) return;
        
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(value);
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        using (writer)
        {
            writer.WriteValueSafe(messageContent);
            Log.Warning($"Send message: {value}");
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(messageName, clientId, writer);
        }
    }

    public void SendToAllClients(T value)
    {
        if (!Messaging.IsServerOrHost) return;
        
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(value);
        var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        using (writer)
        {
            writer.WriteValueSafe(messageContent);
            Log.Warning($"Send message: {value}");
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(messageName, Messaging.ClientIds, writer);
        }
    }
}