using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SharedParameters : NetworkBehaviour
{
    // Start is called before the first frame update

    [SyncVar]
    bool allowGameSpaceExtension = false;

    [SyncVar] 
    Vector3 updatedLeapPosition = new Vector3(0.0f, 0.0f, 0.0f);

    [SyncVar]
    Vector2 playSpaceMeasures = new Vector2(0.0f, 0.0f);

    [Command]
    public void CmdSetGameSpaceExtension(bool extensionAllowed)
    {
        if (!isServer)
        {
            Debug.Log("ERROR: Allow Gamespace called on client");
            return;
        }
        Debug.Log("Shared Parameter on Server - GameSpaceExtensionAllowed: " + extensionAllowed);
        allowGameSpaceExtension = extensionAllowed;
    }

    [Command]
    public void CmdSetNewPosition(Vector3 position)
    {
        if (!isServer)
        {
            Debug.Log("ERROR: SetPosition called on client");
            return;
        }
        Debug.Log("Shared Parameter on Server - new shift");
        updatedLeapPosition = position;
    }


    [Command]
    public void CmdSetPlaySpaceMeasures(Vector2 measures)
    {
        if (!isServer)
        {
            Debug.Log("ERROR: SetPlaySpaceMeasures called on client");
            return;
        }
        Debug.Log("Shared Parameter on Server - new playSpaceMeasure");
        playSpaceMeasures = measures;
    }

    public bool GameSpaceExtensionPossible()
    {
        return allowGameSpaceExtension;
    }

    public Vector3 GetNewPosition()
    {
        return updatedLeapPosition;
    }

    public Vector2 GetPlaySpaceMeasures()
    {
        return playSpaceMeasures;
    }
}
