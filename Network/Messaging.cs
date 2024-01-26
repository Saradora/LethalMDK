using Unity.Netcode;
using UnityMDK.Logging;

namespace LethalMDK.Network;

public static class Messaging
{
    public static CustomMessagingManager Manager => NetworkManager.Singleton.CustomMessagingManager;

    public static bool IsServerOrHost => NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost;

    private static readonly List<ulong> _listWithoutHost = new();

    public static ulong ServerClientId => NetworkManager.ServerClientId;

    public static event Action<NetworkManager> OnSingletonChange;

    internal static void TriggerSingletonChange(NetworkManager singleton)
    {
        OnSingletonChange?.Invoke(singleton);
    }

    public static IReadOnlyList<ulong> ClientIds
    {
        get
        {
            var ids = NetworkManager.Singleton.ConnectedClientsIds;
            _listWithoutHost.Clear();
            ulong serverId = NetworkManager.ServerClientId;
            foreach (var clientId in ids)
            {
                if (clientId == serverId)
                {
                    Log.Warning($"Skipping host: {clientId}");
                    continue;
                }
                _listWithoutHost.Add(clientId);
            }
            return _listWithoutHost;
        }
    }
}