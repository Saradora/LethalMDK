using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public abstract class NetMessageBase<T> : MessageReceiver
{
    public event Action<T, ulong> MessageReceived;
    public event Action<T, ulong> MessageReceivedFromClient;
    public event Action<T> MessageReceivedFromServer;

    protected override void OnReceiveMessage(ulong senderId, FastBufferReader buffer)
    {
        if (!TryReadBuffer(buffer, out T value))
            return;
        
        MessageReceived?.Invoke(value, senderId);
        if (senderId == NetworkMessaging.ServerClientId)
            MessageReceivedFromServer?.Invoke(value);
        else
            MessageReceivedFromClient?.Invoke(value, senderId);
    }

    protected abstract bool TryReadBuffer(FastBufferReader buffer, out T outValue);

    protected abstract int GetWriteSize(T data);

    protected abstract void WriteToBuffer(T data, FastBufferWriter writer);

    protected virtual FastBufferWriter GetWriterAndHash(T data, out uint outHash)
    {
        uint? hash = GetHash();
        if (hash is null)
            throw new NullReferenceException("Cannot send as it is uninitialized.");

        var writer = NetworkMessaging.GetWriter(hash.Value, GetWriteSize(data));
        WriteToBuffer(data, writer);
        outHash = hash.Value;
        return writer;
    }

    public void SendToServer(T data, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        using var writer = GetWriterAndHash(data, out var hash);
        try
        {
            NetworkMessaging.TrySendMessageToServer<T>(hash, writer, delivery);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    public void SendToClient(T data, ulong clientId, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        using var writer = GetWriterAndHash(data, out var hash);
        try
        {
            NetworkMessaging.TrySendMessageToClient<T>(hash, clientId, writer, delivery);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    public void SendToAllClients(T data, bool includeHost = false, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        using var writer = GetWriterAndHash(data, out var hash);
        try
        {
            NetworkMessaging.TrySendMessageToAllClients<T>(hash, writer, includeHost, delivery);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }
}