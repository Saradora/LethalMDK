using HarmonyLib;
using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Patches;

[HarmonyPatch(typeof(NetworkObject))]
public static class NetworkObject_Patching
{
    private static Dictionary<NetworkObject, Action<bool>> _isSpawnedEvents = new();
    private static Dictionary<NetworkObject, Action<ulong>> _networkIdsEvents = new();

    [HarmonyPatch("IsSpawned", MethodType.Setter)]
    private static void IsSpawned_SetterPostfix(bool value, NetworkObject __instance)
    {
        Log.Error($"Network object is spawned");

        if (!_isSpawnedEvents.ContainsKey(__instance))
            return;
        
        _isSpawnedEvents[__instance]?.Invoke(value);
    }

    public static void RegisterToIsSpawnChanges(this NetworkObject obj, Action<bool> action)
    {
        if (!_isSpawnedEvents.ContainsKey(obj)) _isSpawnedEvents.Add(obj, action);
        else _isSpawnedEvents[obj] += action;
    }

    public static void UnregisterFromIsSpawnedChanges(this NetworkObject obj, Action<bool> action)
    {
        if (!_isSpawnedEvents.ContainsKey(obj)) return;
        _isSpawnedEvents[obj] -= action;
        if (_isSpawnedEvents[obj].GetInvocationList().Length <= 0) _isSpawnedEvents.Remove(obj);
    }

    [HarmonyPatch("NetworkObjectId", MethodType.Setter)]
    private static void NetworkObjectId_SetterPostfix(ulong value, NetworkObject __instance)
    {
        Log.Error($"Network object id changed!");

        if (!_networkIdsEvents.ContainsKey(__instance))
            return;
        
        _networkIdsEvents[__instance]?.Invoke(value);
    }

    public static void RegisterToObjectIdChanges(this NetworkObject obj, Action<ulong> action)
    {
        if (!_networkIdsEvents.ContainsKey(obj)) _networkIdsEvents.Add(obj, action);
        else _networkIdsEvents[obj] += action;
    }

    public static void UnregisterFromObjectIdChanges(this NetworkObject obj, Action<ulong> action)
    {
        if (!_networkIdsEvents.ContainsKey(obj)) return;
        _networkIdsEvents[obj] -= action;
        if (_networkIdsEvents[obj].GetInvocationList().Length <= 0) _networkIdsEvents.Remove(obj);
    }
}