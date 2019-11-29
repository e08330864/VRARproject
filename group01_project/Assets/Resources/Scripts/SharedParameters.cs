using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SharedParameters : NetworkBehaviour
{
    // Start is called before the first frame update
    bool allowGameSpaceExtension = false;

    [Server]
    public void AllowGameSpaceExtension()
    {
        allowGameSpaceExtension = true;
    }

    [Server]
    public void DenyGameSpaceExtension()
    {
        allowGameSpaceExtension = false;
    }

    [Server]
    public bool GameSpaceExtensionPossible()
    {
        return allowGameSpaceExtension;
    }
}
