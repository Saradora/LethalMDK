using Unity.Netcode;

namespace LethalMDK.Network;

internal struct NetworkEvent {}

public class MessageHandler
{
    public Type ReturnType { get; }
    public string Name { get; }
    internal CustomMessagingManager.HandleNamedMessageDelegate Action { get; private set; }

    private MessageHandler(string name, CustomMessagingManager.HandleNamedMessageDelegate action, Type returnType)
    {
        ReturnType = returnType;
        Name = name;
        Action = action;
    }

    public static MessageHandler Create<TReturnType>(string name, CustomMessagingManager.HandleNamedMessageDelegate action)
    {
        return new MessageHandler(name, action, typeof(TReturnType));
    }

    public void Subscribe<TReturnType>(CustomMessagingManager.HandleNamedMessageDelegate action)
    {
        if (ReturnType != typeof(TReturnType))
            throw new ArgumentException($"Cannot create network event with type {typeof(TReturnType).Name} because an event with the same name already exists with a different type of {ReturnType.Name}.");
        
        Action += action;
    }

    public void Unsubscribe(CustomMessagingManager.HandleNamedMessageDelegate action)
    {
        Action -= action;
    }
}