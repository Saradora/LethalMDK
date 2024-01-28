﻿using Unity.Netcode;

namespace LethalMDK.Network;

public class GlobalNetMessage<T> : GlobalNetMessageBase<T> where T : unmanaged, IEquatable<T>
{
    public GlobalNetMessage(string name, bool assemblySpecific = false) : base(name, assemblySpecific)
    {
    }

    protected override bool TryReadBuffer(FastBufferReader buffer, out T outValue)
    {
        outValue = default;
        if (!buffer.TryBeginReadValue(outValue))
            return false;

        buffer.ReadValue(out ForceNetworkSerializeByMemcpy<T> value);
        outValue = value.Value;
        return true;
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

    protected override bool TryReadBuffer(FastBufferReader buffer, out T outValue)
    {
        outValue = default;
        if (!buffer.TryBeginReadValue(outValue))
            return false;
        
        buffer.ReadValue(out outValue);
        return true;
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

    protected override bool TryReadBuffer(FastBufferReader buffer, out string outValue)
    {
        buffer.ReadValueSafe(out outValue);
        return true;
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