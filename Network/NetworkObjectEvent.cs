using LethalMDK.Patches;
using Unity.Netcode;

namespace LethalMDK.Network;

public class NetworkObjectEvent
{
    /*
    private readonly NetworkObject _networkObject;
    private readonly string _baseName;
    
    private string _messageName;
    private ulong? _networkObjectId;
    private CustomMessagingManager _messenger;

    
    public NetworkObjectEvent(string name, NetworkObject obj)
    {
        if (!obj)
            throw new NullReferenceException("Network object is null, cannot create network object event.");

        _baseName = name;
        _networkObject = obj;
        _networkObject.RegisterToObjectIdChanges(OnObjectIdChanged);
        Messaging.MessagingManagerChanged += OnMessengerChanged;
        
        if (_networkObject.IsSpawned) _networkObjectId = obj.NetworkObjectId;
        _messenger = NetworkManager.Singleton ? Messaging.Manager : null;
    }

    private void OnObjectIdChanged(ulong value)
    {
        _networkObjectId = value;
        _messageName = 
    }

    private void OnMessengerChanged(CustomMessagingManager messenger)
    {
        if (_messenger is not null && _messageName is not null)
        {
            _messenger.UnregisterNamedMessageHandler(_messageName);
        }
        _messenger = messenger;
    }*/
}