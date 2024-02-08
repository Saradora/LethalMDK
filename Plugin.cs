using BepInEx;
using HarmonyLib;

namespace LethalMDK;

[BepInDependency(UnityMDK.UnityMDK.ModGuid)]
[BepInPlugin(LethalMDK.ModGuid, LethalMDK.ModName, LethalMDK.ModVersion)]
public class PluginInitializer : BaseUnityPlugin
{
    private readonly Harmony _harmonyInstance = new(LethalMDK.ModGuid);
        
    private void Awake()
    {
        _harmonyInstance.PatchAll();
    }
}