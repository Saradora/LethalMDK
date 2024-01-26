using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityMDK.Injection;
using UnityMDK.Logging;

namespace LethalMDK.Network;

[Initializer]
public abstract class NetworkVariable
{
    private static HashSet<NetworkVariable> _variables = new();

    private static NetworkVariable<int> _testVariable = new();

    protected string messageName;
    
    [Initializer]
    private static void Init()
    {
        Messaging.OnSingletonChange += OnSingletonChange;
        Log.Error(_testVariable.messageName);
        var newVar = new NetworkVariable<bool>();
        Log.Error(newVar.messageName);
    }

    private static void OnSingletonChange(NetworkManager singleton)
    {
        Log.Error($"Singleton changed!");
    }

    private static void AddVariable(NetworkVariable variable)
    {
        _variables.Add(variable);
    }

    internal NetworkVariable()
    {
    }
}

public class NetworkVariable<T> : NetworkVariable where T : unmanaged, IEquatable<T>
{
    public NetworkVariable([CallerMemberName] string name = "")
    {
        var stackFrame = new StackFrame(1, true);
        var method = stackFrame.GetMethod();
        if (method is null)
            throw new Exception("Can't find caller of NetworkVariable");

        if (method.GetType().Name != "RuntimeConstructorInfo")
            throw new Exception($"Network Variables can only be created as member variables");
        
        messageName = name;
    }
}