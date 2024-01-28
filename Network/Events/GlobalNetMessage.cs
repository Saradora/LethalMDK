using System.Reflection;
using Unity.Collections;
using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public abstract class GlobalNetMessageBase<T> : IDisposable
{
    private readonly uint _hash;

    public event Action<T, ulong> MessageReceived;
    public event Action<T, ulong> MessageReceivedFromClient;
    public event Action<T> MessageReceivedFromServer;

    internal GlobalNetMessageBase(string name, bool assemblySpecific = false)
    {
        string messageName = name;
        if (assemblySpecific) messageName = Assembly.GetCallingAssembly().GetName().Name + "+" + messageName;
        _hash = messageName.Hash32();
        
        NetworkMessaging.RegisterEvent<T>(_hash, ReceiveMessage);
    }

    public void Dispose()
    {
        NetworkMessaging.UnregisterEvent(_hash, ReceiveMessage);
    }

    private void ReceiveMessage(ulong senderId, FastBufferReader messagePayload)
    {
        T value = ReadBuffer(messagePayload);
        
        MessageReceived?.Invoke(value, senderId);
        if (senderId == NetworkMessaging.ServerClientId)
            MessageReceivedFromServer?.Invoke(value);
        else
            MessageReceivedFromClient?.Invoke(value, senderId);
    }
    
    private int GetWriteSizeInternal(T data)
    {
        return GetWriteSize(data) + FastBufferWriter.GetWriteSize<uint>();
    }

    private void WriteToBufferInternal(T data, FastBufferWriter writer)
    {
        writer.WriteValueSafe(_hash);
        WriteToBuffer(data, writer);
    }

    protected abstract T ReadBuffer(FastBufferReader buffer);

    protected abstract int GetWriteSize(T data);

    protected abstract void WriteToBuffer(T data, FastBufferWriter writer);

    public void SendToServer(T data, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        using var writer = new FastBufferWriter(GetWriteSizeInternal(data), Allocator.Temp);
        WriteToBufferInternal(data, writer);
        try
        {
            NetworkMessaging.TrySendMessageToServer<T>(_hash, writer, delivery);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }
    
    public void SendToClient(T data, ulong clientId, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        using var writer = new FastBufferWriter(GetWriteSizeInternal(data), Allocator.Temp);
        WriteToBufferInternal(data, writer);
        try
        {
            NetworkMessaging.TrySendMessageToClient<T>(_hash, clientId, writer, delivery);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }

    public void SendToAllClients(T data, bool includeHost = false, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
    {
        using var writer = new FastBufferWriter(GetWriteSizeInternal(data), Allocator.Temp);
        WriteToBufferInternal(data, writer);
        try
        {
            NetworkMessaging.TrySendMessageToAllClients<T>(_hash, writer, includeHost, delivery);
        }
        catch (Exception e)
        {
            Log.Exception(e);
        }
    }
}

public class GlobalNetMessage<T> : GlobalNetMessageBase<T> where T : unmanaged, IEquatable<T>
{
    public GlobalNetMessage(string name, bool assemblySpecific = false) : base(name, assemblySpecific)
    {
    }

    protected override T ReadBuffer(FastBufferReader buffer)
    {
        buffer.ReadValueSafe(out ForceNetworkSerializeByMemcpy<T> value);
        return value.Value;
    }

    protected override int GetWriteSize(T data)
    {
        return FastBufferWriter.GetWriteSize<T>();
    }

    protected override void WriteToBuffer(T data, FastBufferWriter writer)
    {
        writer.WriteValueSafe(new ForceNetworkSerializeByMemcpy<T>(data));
    }
}

public class GlobalNetStructMessage<T> : GlobalNetMessageBase<T> where T : unmanaged, INetworkSerializeByMemcpy
{
    public GlobalNetStructMessage(string name, bool assemblySpecific = false) : base(name, assemblySpecific)
    {
    }

    protected override T ReadBuffer(FastBufferReader buffer)
    {
        buffer.ReadValueSafe(out T value);
        return value;
    }

    protected override int GetWriteSize(T data)
    {
        return FastBufferWriter.GetWriteSize(data);
    }

    protected override void WriteToBuffer(T data, FastBufferWriter writer)
    {
        writer.WriteValueSafe(data);
    }
}

public class GlobalNetMessage : GlobalNetMessageBase<string>
{
    public GlobalNetMessage(string name, bool assemblySpecific = false) : base(name, assemblySpecific)
    {
    }

    protected override string ReadBuffer(FastBufferReader buffer)
    {
        buffer.ReadValueSafe(out string value);
        return value;
    }

    protected override int GetWriteSize(string data)
    {
        return FastBufferWriter.GetWriteSize(data);
    }

    protected override void WriteToBuffer(string data, FastBufferWriter writer)
    {
        writer.WriteValueSafe(data);
    }
}