using System.Reflection;
using LethalMDK.Patches;
using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public abstract class ObjectNetMessageBase<T> : NetMessageBase<T>
{
    private uint? _hash;
    private readonly string _baseName;
    private readonly NetworkObject _targetObject;

    public ObjectNetMessageBase(string name, NetworkObject targetObject, bool assemblySpecific = false)
    {
        if (targetObject == null)
            throw new NullReferenceException("Cannot create an object event without a target object.");

        _baseName = name;
        if (assemblySpecific) _baseName = Assembly.GetCallingAssembly().GetName().Name + "+" + _baseName;
        _targetObject = targetObject;
        _targetObject.RegisterToObjectIdChanges(OnNetworkObjectIdChanged);
        RegisterNetworkObject();
    }

    private void OnNetworkObjectIdChanged(ulong value)
    {
        if (value != 0)
        {
            RegisterNetworkObject();
        }
        else
        {
            Dispose();
        }
    }

    protected override uint? GetHash()
    {
        return _hash;
    }
    
    private void RegisterNetworkObject()
    {
        if (_targetObject.NetworkObjectId == 0) return;
        if (_hash.HasValue)
        {
            UnregisterNetworkObject();
        }

        _hash = $"obj{_targetObject.NetworkObjectId}+{_baseName}".Hash32();
        Log.Error($"Event registered: {_hash.Value}");
        RegisterEvent<T>();
    }

    private void UnregisterNetworkObject()
    {
        if (!_hash.HasValue) return;
        UnregisterEvent();
        Log.Error($"Event unregistered: {_hash.Value}");
        _hash = null;
    }

    public override void Dispose()
    {
        UnregisterNetworkObject();
        if (_targetObject != null)
        {
            _targetObject.UnregisterFromObjectIdChanges(OnNetworkObjectIdChanged);
        }
    }
}