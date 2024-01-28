using Unity.Netcode;

namespace LethalMDK.Network;

internal struct NetworkEvent {}

public class MessageHandler
{
    public Type ReturnType { get; }
    internal CustomMessagingManager.UnnamedMessageDelegate Action { get; private set; }

    public int ActionCount => Action is null ? 0 : Action.GetInvocationList().Length;

    private MessageHandler(CustomMessagingManager.UnnamedMessageDelegate action, Type returnType)
    {
        ReturnType = returnType;
        Action = action;
    }

    public static MessageHandler Create<TReturnType>(CustomMessagingManager.UnnamedMessageDelegate action)
    {
        return new MessageHandler(action, typeof(TReturnType));
    }

    public void Subscribe<TReturnType>(CustomMessagingManager.UnnamedMessageDelegate action)
    {
        if (ReturnType != typeof(TReturnType))
            throw new ArgumentException($"Cannot create network event with type {typeof(TReturnType).Name} because an event with the same name already exists with a different type of {ReturnType.Name}.");
        
        Action += action;
    }

    public void Unsubscribe(CustomMessagingManager.UnnamedMessageDelegate action)
    {
        Action -= action;
    }
}