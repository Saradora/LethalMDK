using UnityEngine;
using UnityMDK.Injection;

namespace LethalMDK.World;

public static class ScanNodes
{
    public enum EType
    {
        Utility = 0,
        Creature = 1,
        Scrap = 2,
    }

    public static void AddScanNode<TComponent>(EType nodeType, string headerText, string subText, int minRange, int maxRange, Vector3? offset = null, bool requireLineOfSight = true, int creatureId = -1) where TComponent : Component
    {
        ScanNodeConstructor constructor = new(nodeType, headerText, subText, minRange, maxRange, offset ?? Vector3.zero, requireLineOfSight, creatureId);
        SceneInjection.AddComponentInjector<TComponent>(constructor);
    }
}