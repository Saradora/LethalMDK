using UnityEngine;

namespace LethalMDK;

public class LayerMasks
{
    public static readonly int Default = 1 << Layers.Default;
    public static readonly int InteractableObject = 1 << Layers.InteractableObject;
    public static readonly int Room = 1 << Layers.Room;
    public static readonly int ScanNode = 1 << Layers.ScanNode;
}

public static class Layers
{
    /// <summary>
    /// Number 1.
    /// </summary>
    public static readonly int Default = LayerMask.NameToLayer(nameof(Default));
    /// <summary>
    /// Number 2.
    /// </summary>
    public static readonly int TransparentFX = LayerMask.NameToLayer(nameof(TransparentFX));
    /// <summary>
    /// Number 3.
    /// </summary>
    public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
    /// <summary>
    /// Number 4.
    /// </summary>
    public static readonly int Player = LayerMask.NameToLayer(nameof(Player));
    /// <summary>
    /// Number 5.
    /// </summary>
    public static readonly int Water = LayerMask.NameToLayer(nameof(Water));
    /// <summary>
    /// Number 6.
    /// </summary>
    public static readonly int UI = LayerMask.NameToLayer(nameof(UI));
    /// <summary>
    /// Number 7.
    /// </summary>
    public static readonly int Props = LayerMask.NameToLayer(nameof(Props));
    /// <summary>
    /// Number 8.
    /// </summary>
    public static readonly int HelmetVisor = LayerMask.NameToLayer(nameof(HelmetVisor));
    /// <summary>
    /// Number 9.
    /// </summary>
    public static readonly int Room = LayerMask.NameToLayer(nameof(Room));
    /// <summary>
    /// Number 10.
    /// </summary>
    public static readonly int InteractableObject = LayerMask.NameToLayer(nameof(InteractableObject));
    /// <summary>
    /// Number 11.
    /// </summary>
    public static readonly int Foliage = LayerMask.NameToLayer(nameof(Foliage));
    /// <summary>
    /// Number 12.
    /// </summary>
    public static readonly int Colliders = LayerMask.NameToLayer(nameof(Colliders));
    /// <summary>
    /// Number 13.
    /// </summary>
    public static readonly int PhysicsObject = LayerMask.NameToLayer(nameof(PhysicsObject));
    /// <summary>
    /// Number 14.
    /// </summary>
    public static readonly int Triggers = LayerMask.NameToLayer(nameof(Triggers));
    /// <summary>
    /// Number 15.
    /// </summary>
    public static readonly int MapRadar = LayerMask.NameToLayer(nameof(MapRadar));
    /// <summary>
    /// Number 16.
    /// </summary>
    public static readonly int NavigationSurface = LayerMask.NameToLayer(nameof(NavigationSurface));
    /// <summary>
    /// Number 17.
    /// </summary>
    public static readonly int RoomLight = LayerMask.NameToLayer(nameof(RoomLight));
    /// <summary>
    /// Number 18.
    /// </summary>
    public static readonly int Anomaly = LayerMask.NameToLayer(nameof(Anomaly));
    /// <summary>
    /// Number 19.
    /// </summary>
    public static readonly int LineOfSight = LayerMask.NameToLayer(nameof(LineOfSight));
    /// <summary>
    /// Number 20.
    /// </summary>
    public static readonly int Enemies = LayerMask.NameToLayer(nameof(Enemies));
    /// <summary>
    /// Number 21.
    /// </summary>
    public static readonly int PlayerRagdoll = LayerMask.NameToLayer(nameof(PlayerRagdoll));
    /// <summary>
    /// Number 22.
    /// </summary>
    public static readonly int MapHazards = LayerMask.NameToLayer(nameof(MapHazards));
    /// <summary>
    /// Number 23.
    /// </summary>
    public static readonly int ScanNode = LayerMask.NameToLayer(nameof(ScanNode));
    /// <summary>
    /// Number 24.
    /// </summary>
    public static readonly int EnemiesNotRendered = LayerMask.NameToLayer(nameof(EnemiesNotRendered));
    /// <summary>
    /// Number 25.
    /// </summary>
    public static readonly int MiscLevelGeometry = LayerMask.NameToLayer(nameof(MiscLevelGeometry));
    /// <summary>
    /// Number 26.
    /// </summary>
    public static readonly int Terrain = LayerMask.NameToLayer(nameof(Terrain));
    /// <summary>
    /// Number 27.
    /// </summary>
    public static readonly int PlaceableShipObjects = LayerMask.NameToLayer(nameof(PlaceableShipObjects));
    /// <summary>
    /// Number 28.
    /// </summary>
    public static readonly int PlacementBlocker = LayerMask.NameToLayer(nameof(PlacementBlocker));
    /// <summary>
    /// Number 29.
    /// </summary>
    public static readonly int Railing = LayerMask.NameToLayer(nameof(Railing));
    /// <summary>
    /// Number 30.
    /// </summary>
    public static readonly int DecalStickableSurface = LayerMask.NameToLayer(nameof(DecalStickableSurface));
    
}

public class Tags
{
    public static readonly string InteractTrigger = nameof(InteractTrigger);
}