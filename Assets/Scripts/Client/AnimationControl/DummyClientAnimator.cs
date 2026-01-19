using Unity.Netcode.Components;
using UnityEngine;

public class OwnerNetworkAnimator : NetworkAnimator
{
    // Implementation by Unity Technologies (2026, a: 19/1/2026):
    // https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@2.8/manual/components/helper/networkanimator.html

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}

