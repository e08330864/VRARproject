﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LeapGameSpaceExtension: MonoBehaviour
{
    public ParametersAuthorityManager parametersAuthorityManager;
    public SharedParameters viveSharedScript;

    //private bool allowGameSpaceExtension = false;

    private Vector3? thisSharedPosition = null;
    private Vector3? lastSharedPosition = null;

    // Update is called once per frame
    void Update()
    {

        thisSharedPosition = viveSharedScript.GetNewPosition();

        //update center position of the leap player
        if(thisSharedPosition.HasValue && lastSharedPosition.HasValue && lastSharedPosition.Value != thisSharedPosition.Value)
        {
            transform.position = thisSharedPosition.Value;
        }
        lastSharedPosition = thisSharedPosition;
    }

    public void SetAllowGameSpaceExtension(bool value)
    {
        parametersAuthorityManager.SetGameSpaceExtensionPossible(value);
    }
}
