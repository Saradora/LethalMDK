using System.Reflection;
using Unity.Collections;
using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public class NetworkMessage<T> : IDisposable where T : unmanaged, IEquatable<T>
{/*
    private readonly string _messageName;

    public NetworkMessage(string name)
    {
        if (!NetworkManager.Singleton)
            throw new NullReferenceException("Network Singleton hasn't yet been instantied, you need to construct the NetworkMessage later.");
        
        MessageReceivedFromClient = null;
        MessageReceivedFromServer = null;
        _messageName = Assembly.GetCallingAssembly().GetName().Name + "+" + name;
        Messaging.Manager.RegisterNamedMessageHandler(_messageName, ReceiveMessage);
    }

    public event Action<T, ulong> MessageReceivedFromClient;
    public event Action<T> MessageReceivedFromServer;

    private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        messagePayload.ReadValueSafe(out ForceNetworkSerializeByMemcpy<T> receivedMessageContent);
        
        if (senderId == Messaging.ServerClientId)
            MessageReceivedFromServer?.Invoke(receivedMessageContent.Value);
        else
            MessageReceivedFromClient?.Invoke(receivedMessageContent.Value, senderId);
    }

    public void SendToServer(T value)
    {
        if (Messaging.IsServerOrHost)
        {
            Log.Warning($"Can't send message to server as the server.");
            return;
        }
        
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(value);
        using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        writer.WriteValueSafe(messageContent);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(_messageName, Messaging.ServerClientId, writer);
    }

    public void SendToClient(T value, ulong clientId)
    {
        if (!Messaging.IsServerOrHost)
        {
            Log.Warning($"Can't send message to client as another client");
            return;
        }
        if (clientId == Messaging.ServerClientId)
        {
            Log.Warning($"Can't send message to self");
            return;
        }
        
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(value);
        using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        writer.WriteValueSafe(messageContent);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(_messageName, clientId, writer);
    }

    public void SendToAllClients(T value, bool sendToHost = false)
    {
        if (!Messaging.IsServerOrHost)
        {
            Log.Warning($"Can't send message to client as another client");
            return;
        }
        
        var messageContent = new ForceNetworkSerializeByMemcpy<T>(value);
        using var writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<T>(), Allocator.Temp);
        writer.WriteValueSafe(messageContent);
        if (sendToHost)
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(_messageName, writer);
        else
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(_messageName, Messaging.ClientIds, writer);
    }*/

    public void Dispose()
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.CustomMessagingManager is not null)
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("blep");
    }
}