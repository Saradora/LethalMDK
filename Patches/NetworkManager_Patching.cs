using HarmonyLib;
using LethalMDK.Network;
using Unity.Netcode;

namespace LethalMDK.Patches;

[HarmonyPatch(typeof(NetworkManager))]
public static class NetworkManager_Patching
{
    [HarmonyPatch("OnDestroy"), HarmonyPrefix]
    private static void OnDestroy_Prefix(NetworkManager __instance)
    {
        if (NetworkManager.Singleton == __instance)
        {
            Messaging.TriggerSingletonChange(null);
        }
    }

    [HarmonyPatch("SetSingleton"), HarmonyPostfix]
    private static void SetSingleton_Postfix()
    {
        Messaging.TriggerSingletonChange(NetworkManager.Singleton);
    }
}