﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LeapGameSpaceExtension: MonoBehaviour
{
    private GameObject sharedParameters;
    private ParametersAuthorityManager parametersAuthorityManager;
    private SharedParameters sharedScript;
    private bool allowGameSpaceExtension = false;

    // Update is called once per frame

    void Update()
    {
        if(sharedParameters == null)
        {
            InitializeSharedParameters();
        }
        if(sharedParameters != null)
        {
            parametersAuthorityManager = sharedParameters.GetComponent<ParametersAuthorityManager>();
            sharedScript = sharedParameters.GetComponent<SharedParameters>();

        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            allowGameSpaceExtension = !allowGameSpaceExtension;
            parametersAuthorityManager.SetGameSpaceExtensionPossible(allowGameSpaceExtension);
        }

        if(sharedScript != null)
        {
            //update center position of the leap player
            this.transform.position = transform.position + sharedScript.GetExtensionShift();
        }
        
    }

    public void InitializeSharedParameters()
    {
        sharedParameters = GameObject.Find("Parameters");
    }
}
