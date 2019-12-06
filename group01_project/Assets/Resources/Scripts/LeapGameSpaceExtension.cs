using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LeapGameSpaceExtension: MonoBehaviour
{
    public GameObject sharedParameters;
    private Actor actor = null;
    private bool allowGameSpaceExtension = false;

    // Update is called once per frame


    void Update()
    {
        if(sharedParameters == null)
        {
            InitializeSharedParameters();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            allowGameSpaceExtension = !allowGameSpaceExtension;
            Debug.Log("Gamespace extension possible: " + allowGameSpaceExtension);

            ParametersAuthorityManager parametersAuthorityManager = sharedParameters.GetComponent<ParametersAuthorityManager>();
            parametersAuthorityManager.SetGameSpaceExtensionPossible(allowGameSpaceExtension);

            /*NetworkIdentity parameterID = sharedParameters.GetComponent<NetworkIdentity>();


            actor.RequestObjectAuthority(parameterID);
            Debug.Log("LeapExtension Has Authority: " + parameterID.hasAuthority);
            sharedParameters.CmdSetGameSpaceExtension(allowGameSpaceExtension);
            actor.ReturnObjectAuthority(parameterID);
            Debug.Log("LeapExtension Has Authority: " + parameterID.hasAuthority);*/

        }
    }

    public void InitializeActor()
    {
        GameObject playerTransform = GameObject.Find("Player");
        if (playerTransform != null)
        {
            actor = playerTransform.GetComponent<Actor>();
            Debug.Log("Actor Initialized");
        }
    }

    public void InitializeSharedParameters()
    {

        sharedParameters = GameObject.Find("Parameters");
    }
}
