using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityMDK;
using System.Security.Cryptography;
using System.Text;
using UnityMDK.Logging;
using UnityMDK.Reflection;

namespace LethalMDK.Network;

public static class NetworkPrefabs
{
    public static GameObject GetEmptyPrefab(string name)
    {
        var prefab = PrefabUtils.GetEmptyPrefab(name);
        var networkObject = prefab.AddComponent<NetworkObject>();

        var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Assembly.GetCallingAssembly().GetName().Name + name));

        networkObject.SetField("GlobalObjectIdHash", BitConverter.ToUInt32(hash, 0));
        
        RegisterNetworkPrefab(prefab);
        return prefab;
    }

    public static void RegisterNetworkPrefab(GameObject prefab)
    {
        if (!NetworkManager.Singleton)
        {
            Log.Error("Network manager isn't ready");
            return;
        }
        
        NetworkManager.Singleton.AddNetworkPrefab(prefab);
    }
}