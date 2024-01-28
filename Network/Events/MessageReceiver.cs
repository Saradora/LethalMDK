using Unity.Netcode;

namespace LethalMDK.Network;

public abstract class MessageReceiver : IDisposable
{
    protected abstract uint? GetHash();

    protected void RegisterEvent<TReturnType>()
    {
        uint? hash = GetHash();
        if (hash is null)
            throw new NullReferenceException("Cannot register event as it is uninitialized.");
            
        NetworkMessaging.RegisterEvent<TReturnType>(hash.Value, OnReceiveMessage);
    }

    protected void UnregisterEvent()
    {
        uint? hash = GetHash();
        if (hash is null) 
            return;
        
        NetworkMessaging.UnregisterEvent(hash.Value, OnReceiveMessage);
    }

    protected abstract void OnReceiveMessage(ulong senderId, FastBufferReader buffer);
    
    public virtual void Dispose()
    {
        UnregisterEvent();
    }
}