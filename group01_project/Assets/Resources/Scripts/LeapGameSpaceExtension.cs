using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LeapGameSpaceExtension: MonoBehaviour
{
    public SharedParameters sharedParameters;
    private Actor actor = null;
    private bool allowGameSpaceExtension = false;

   

    // Update is called once per frame
    void Update()
    {
        if(actor == null)
        {
            InitializeActor();
        }


        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            allowGameSpaceExtension = !allowGameSpaceExtension;
            Debug.Log("Gamespace extension possible: " + allowGameSpaceExtension);

            NetworkIdentity parameterID = sharedParameters.GetComponent<NetworkIdentity>();

            actor.RequestObjectAuthority(parameterID);
            sharedParameters.CmdSetGameSpaceExtension(allowGameSpaceExtension);
            actor.ReturnObjectAuthority(parameterID);
            
        }
    }

    public void InitializeActor()
    {
        Transform playerTransform = transform.FindChild("Player");
        if (playerTransform != null)
        {
            actor = playerTransform.GetComponent<Actor>();
            Debug.Log("Actor Initialized");
        }
    }
}
