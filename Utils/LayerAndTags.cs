using UnityEngine;

namespace LethalMDK;

public class LayerMasks
{
    public static readonly int Default = 1 << LayerMask.NameToLayer(nameof(Default));
    public static readonly int InteractableObject = 1 << LayerMask.NameToLayer(nameof(InteractableObject));
    public static readonly int Room = 1 << LayerMask.NameToLayer(nameof(Room));
    public static readonly int ScanNode = 1 << LayerMask.NameToLayer(nameof(ScanNode));
}

public class Layers
{
    public static readonly int Default = LayerMask.NameToLayer(nameof(Default));
    public static readonly int InteractableObject = LayerMask.NameToLayer(nameof(InteractableObject));
    public static readonly int Room = LayerMask.NameToLayer(nameof(Room));
    public static readonly int ScanNode = LayerMask.NameToLayer(nameof(ScanNode));
}

public class Tags
{
    public static readonly string InteractTrigger = nameof(InteractTrigger);
}