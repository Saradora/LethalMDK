using System.Reflection;
using Unity.Collections;
using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public struct NetworkEvent : IDisposable
{
    private readonly string _messageName;

    private static readonly FastBufferWriter _dummyWriter = new(0, Allocator.Persistent);

    public NetworkEvent(string name)
    {
        if (!NetworkManager.Singleton)
            throw new NullReferenceException("Network Singleton hasn't yet been instantied, you need to construct the NetworkMessage later.");
        
        MessageReceivedFromClient = null;
        MessageReceivedFromServer = null;
        _messageName = Assembly.GetCallingAssembly().GetName().Name + "+" + name;
        Messaging.Manager.RegisterNamedMessageHandler(_messageName, ReceiveMessage);
    }

    public event Action<ulong> MessageReceivedFromClient;
    public event Action MessageReceivedFromServer;

    private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        if (senderId == Messaging.ServerClientId)
            MessageReceivedFromServer?.Invoke();
        else
            MessageReceivedFromClient?.Invoke(senderId);
    }

    public void InvokeToServer()
    {
        if (Messaging.IsServerOrHost)
        {
            Log.Warning($"Can't send event to server as the server.");
            return;
        }
        
        Messaging.Manager.SendNamedMessage(_messageName, Messaging.ServerClientId, _dummyWriter);
    }

    public void InvokeToClient(ulong clientId)
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
        
        Messaging.Manager.SendNamedMessage(_messageName, clientId, _dummyWriter);
    }

    public void InvokeToAllClients(bool includeHost = false)
    {
        if (!Messaging.IsServerOrHost)
        {
            Log.Warning($"Can't send message to client as another client");
            return;
        }
        
        if (includeHost)
            Messaging.Manager.SendNamedMessageToAll(_messageName, _dummyWriter);
        else
            Messaging.Manager.SendNamedMessage(_messageName, Messaging.ClientIds, _dummyWriter);
    }

    public void Dispose()
    {
        if (NetworkManager.Singleton && Messaging.Manager is not null)
            Messaging.Manager.UnregisterNamedMessageHandler(_messageName);
    }
}