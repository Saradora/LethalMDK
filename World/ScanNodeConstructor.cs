using LethalMDK.Logging;
using UnityEngine;
using UnityMDK.Injection;

namespace LethalMDK.World;

public class ScanNodeConstructor : ComponentInjector
{
    private readonly int _nodeType;
    private readonly string _headerText;
    private readonly string _subText;
    private readonly int _minRange;
    private readonly int _maxRange;
    private readonly bool _requireLineOfSight;
    private readonly int _creatureId;
    private readonly Vector3 _offset;

    public ScanNodeConstructor
        (
        ScanNodes.EType nodeType, 
        string headerText, 
        string subText, 
        int minRange, 
        int maxRange, 
        Vector3 offset,
        bool requireLineOfSight, 
        int creatureId
        )
    {
        _nodeType = (int)nodeType;
        _headerText = headerText;
        _subText = subText;
        _minRange = minRange;
        _maxRange = maxRange;
        _offset = offset;
        _requireLineOfSight = requireLineOfSight;
        _creatureId = creatureId;
    }
    
    public override bool CanBeInjected(Component component)
    {
        return !component.GetComponentInChildren<ScanNodeProperties>(true);
    }

    public override void Inject(Component component)
    {
        Log.Print($"Creating scan node for {component.GetType().Name}");
        GameObject scanNode = new GameObject("Scan Node");
        scanNode.layer = Layers.ScanNode;
        
        ScanNodeProperties properties = scanNode.AddComponent<ScanNodeProperties>();
        properties.headerText = _headerText;
        properties.subText = _subText;
        properties.minRange = _minRange;
        properties.maxRange = _maxRange;
        properties.creatureScanID = _creatureId;
        properties.nodeType = _nodeType;
        properties.requiresLineOfSight = _requireLineOfSight;

        BoxCollider collider = scanNode.AddComponent<BoxCollider>();
        collider.size = Vector3.one * 0.2f;
        
        scanNode.transform.SetParent(component.transform);
        scanNode.transform.SetLocalPositionAndRotation(_offset, Quaternion.identity);
    }
}