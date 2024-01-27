using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public static class NetworkMessaging
{
    private static Dictionary<uint, MessageHandler> _registeredMessages = new();
    // todo verify if the valuetuple being a struct doesn't mess with action registering and stuff
    private static CustomMessagingManager _messenger;

    public static bool IsServerOrHost => NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost;

    public static ulong ServerClientId => NetworkManager.ServerClientId;

    public static event Action<NetworkManager> SingletonChanged;
    public static event Action<CustomMessagingManager> MessagingManagerChanged;

    internal static void TriggerSingletonChange(NetworkManager singleton) => 
        SingletonChanged?.Invoke(singleton);

    internal static void TriggerMessengerChange(CustomMessagingManager messenger) =>
        MessagingManagerChanged?.Invoke(messenger);

    private static readonly List<ulong> _clientIds = new();

    public static IReadOnlyList<ulong> ClientIds
    {
        get
        {
            var ids = NetworkManager.Singleton.ConnectedClientsIds;
            _clientIds.Clear();
            ulong serverId = NetworkManager.ServerClientId;
            foreach (var clientId in ids)
            {
                if (clientId == serverId)
                {
                    Log.Warning($"Skipping host: {clientId}");
                    continue;
                }
                _clientIds.Add(clientId);
            }
            return _clientIds;
        }
    }

    static NetworkMessaging()
    {
        MessagingManagerChanged += OnMessagingManagerChanged;
    }

    private static void OnMessagingManagerChanged(CustomMessagingManager instance)
    {
        if (_messenger is not null)
        {
            foreach ((uint _, MessageHandler msg) in _registeredMessages)
            {
                _messenger.UnregisterNamedMessageHandler(msg.Name);
            }
        }

        _messenger = instance;
        
        foreach ((uint _, MessageHandler msg) in _registeredMessages)
        {
            _messenger.RegisterNamedMessageHandler(msg.Name, msg.Action);
        }
    }

    public static void RegisterEvent<TReturnType>(uint hash, string name, CustomMessagingManager.HandleNamedMessageDelegate action)
    {
        if (!_registeredMessages.ContainsKey(hash))
        {
            _registeredMessages[hash] = MessageHandler.Create<TReturnType>(name, action);
            _messenger?.RegisterNamedMessageHandler(name, _registeredMessages[hash].Action); // todo verify if i don't need to overwrite it entirely each time
        }
        else _registeredMessages[hash].Subscribe<TReturnType>(action);
    }

    public static void UnregisterEvent(uint hash, CustomMessagingManager.HandleNamedMessageDelegate action)
    {
        if (!_registeredMessages.ContainsKey(hash)) return;

        MessageHandler handler = _registeredMessages[hash];

        handler.Unsubscribe(action);

        if (handler.Action.GetInvocationList().Length <= 0)
        {
            _messenger?.UnregisterNamedMessageHandler(handler.Name);
            _registeredMessages.Remove(hash);
        }
    }

    public static void TrySendMessageToAllClients<TReturnType>(uint hash, FastBufferWriter writer, bool includeHost)
    {
        if (!IsServerOrHost)
        {
            Log.Warning($"Message couldn't be sent because you are not the server.");
            return;
        }
        
        if (!TryGetHandlerToSend<TReturnType>(hash, out var handler))
            return;
        
        if (includeHost)
            _messenger.SendNamedMessageToAll(handler.Name, writer);
        else
            _messenger.SendNamedMessage(handler.Name, ClientIds, writer);
    }

    public static void TrySendMessageToServer<TReturnType>(uint hash, FastBufferWriter writer)
    {
        if (IsServerOrHost)
        {
            Log.Warning($"Message couldn't be sent because you are the server.");
            return;
        }
        
        TrySendMessageToClientInternal<TReturnType>(hash, ServerClientId, writer);
    }

    public static void TrySendMessageToClient<TReturnType>(uint hash, ulong clientId, FastBufferWriter writer)
    {
        if (!IsServerOrHost)
        {
            Log.Warning($"Message couldn't be sent because you are not the server.");
            return;
        }
        
        TrySendMessageToClientInternal<TReturnType>(hash, clientId, writer);
    }

    private static void TrySendMessageToClientInternal<TReturnType>(uint hash, ulong clientId, FastBufferWriter writer)
    {
        if (!TryGetHandlerToSend<TReturnType>(hash, out var handler))
            return;

        _messenger.SendNamedMessage(handler.Name, clientId, writer);
    }

    private static bool TryGetHandlerToSend<TReturnType>(uint hash, out MessageHandler outHandler)
    {
        if (_messenger is null)
        {
            Log.Warning($"Message couldn't be sent because messenger isn't active");
            outHandler = null;
            return false;
        }
        
        if (!_registeredMessages.TryGetValue(hash, out outHandler))
        {
            Log.Warning($"Message couldn't be sent because it wasn't registered to the messenger");
            return false;
        }

        if (outHandler.ReturnType != typeof(TReturnType))
        {
            Log.Error($"Message couldn't be sent because the registered event with the same name has a different return type.");
            outHandler = null;
            return false;
        }

        return true;
    }
}