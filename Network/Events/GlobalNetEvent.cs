using System.Reflection;
using Unity.Collections;
using Unity.Netcode;

namespace LethalMDK.Network;

public class GlobalNetEvent : NetEventBase
{
    private readonly uint _hash;

    public GlobalNetEvent(string name, bool assemblySpecific = false)
    {
        string messageName = name;
        if (assemblySpecific) messageName = Assembly.GetCallingAssembly().GetName().Name + "+" + messageName;
        _hash = messageName.Hash32();
        
        RegisterEvent<NetworkEvent>();
    }

    protected override uint? GetHash()
    {
        return _hash;
    }
}