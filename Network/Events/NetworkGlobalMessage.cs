using System.Reflection;
using Unity.Collections;
using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public class NetworkGlobalMessage<T> : IDisposable where T : unmanaged, IEquatable<T>
{
    private readonly string _messageName;
    private readonly uint _hash;

    public event Action<T, ulong> MessageReceivedFromClient;
    public event Action<T> MessageReceivedFromServer;

    public NetworkGlobalMessage(string name, bool assemblySpecific = false)
    {
        _messageName = name;
        if (assemblySpecific) _messageName = Assembly.GetCallingAssembly().GetName().Name + "+" + _messageName;
        _hash = _messageName.Hash32();
        
        NetworkMessaging.RegisterEvent<T>(_hash, _messageName, ReceiveMessage);
    }

    public void Dispose()
    {
        NetworkMessaging.UnregisterEvent(_hash, ReceiveMessage);
    }

    private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        messagePayload.ReadValueSafe(out ForceNetworkSerializeByMemcpy<T> receivedMessageContent);
        
        if (senderId == NetworkMessaging.ServerClientId)
            MessageReceivedFromServer?.Invoke(receivedMessageContent.Value);
        else
            MessageReceivedFromClient?.Invoke(receivedMessageContent.Value, senderId);
    }

    public void SendToServer(T data)
    {
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(data);
        using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        writer.WriteValueSafe(messageContent);
        try
        {
            NetworkMessaging.TrySendMessageToServer<T>(_hash, writer);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }
    
    public void SendToClient(T data, ulong clientId)
    {
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(data);
        using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        writer.WriteValueSafe(messageContent);
        try
        {
            NetworkMessaging.TrySendMessageToClient<T>(_hash, clientId, writer);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    public void SendToAllClients(T data, bool includeHost = false)
    {
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(data);
        using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        writer.WriteValueSafe(messageContent);
        try
        {
            NetworkMessaging.TrySendMessageToAllClients<T>(_hash, writer, includeHost);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }
}