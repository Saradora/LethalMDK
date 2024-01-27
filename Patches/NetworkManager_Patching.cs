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
            if (NetworkManager.Singleton != null) NetworkMessaging.TriggerMessengerChange(null);
            NetworkMessaging.TriggerSingletonChange(null);
        }
    }

    [HarmonyPatch("SetSingleton"), HarmonyPostfix]
    private static void SetSingleton_Postfix()
    {
        NetworkMessaging.TriggerSingletonChange(NetworkManager.Singleton);
    }

    [HarmonyPatch("Initialize"), HarmonyPostfix]
    private static void Initialize_Postfix(bool server)
    {
        NetworkMessaging.TriggerMessengerChange(NetworkManager.Singleton.CustomMessagingManager);
    }
}