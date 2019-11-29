using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LeapGameSpaceExtension: NetworkBehaviour
{
    public SharedParameters sharedParameters;
    private bool allowGameSpaceExtension = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Debug.Log("Allow Gamespace extension from leap side");
            allowGameSpaceExtension = true;

            sharedParameters.GetComponent<NetworkIdentity>().AssignClientAuthority(this.connectionToClient);
            sharedParameters.CmdSetGameSpaceExtension(allowGameSpaceExtension);
            sharedParameters.GetComponent<NetworkIdentity>().AssignClientAuthority(this.connectionToClient);
        }
        else
        {
            Debug.Log("Deny Gamespace extension from leap side");
            allowGameSpaceExtension = false;

            sharedParameters.GetComponent<NetworkIdentity>().AssignClientAuthority(this.connectionToClient);
            sharedParameters.CmdSetGameSpaceExtension(allowGameSpaceExtension);
            sharedParameters.GetComponent<NetworkIdentity>().AssignClientAuthority(this.connectionToClient);
        }
    }
}
