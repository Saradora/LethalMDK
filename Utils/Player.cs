using GameNetcodeStuff;

namespace LethalMDK;

public class Player
{
    public static PlayerControllerB LocalPlayer => GameNetworkManager.Instance ? GameNetworkManager.Instance.localPlayerController : null;
}