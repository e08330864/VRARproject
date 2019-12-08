using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Actor : NetworkBehaviour {

    public Character character;
    public FootCharacter footCharacter;
    public new Transform transform;
    private GameObject createdObject = null;

    List<NetworkIdentity> sharedObjects = new List<NetworkIdentity>(); // shared objects on the server or localActor

    [SyncVar]
    private string prefabNameHand = "";

    [SyncVar]
    private string prefabNameFoot = "";

    //this part is for object sharing
    //List<NetworkIdentity> sharedObjects = new List<NetworkIdentity>(); // shared objects on the server or localActor

    protected virtual void Awake()
    {
        transform = base.transform;
    }


    // Use this for initialization
    void Start() {

        if (isServer || isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                // Inform the local player about his new character
                LocalPlayerController.Singleton.SetActor(this);
                CmdInitialize(prefabNameHand);
                CmdInitialize(prefabNameFoot);
            }


            //this part is for object sharing
            //*******************************
            if (isServer)
            {
                // find objects that can be manipulated 
                // TIPP : you can use a specific tag for all GO's that can be manipulated by players
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("shared"))
                {
                    sharedObjects.Add(go.GetComponent<NetworkIdentity>());
                }
            }
            if (isLocalPlayer)
            {
                // find objects that can be manipulated 
                // assign this Actor to the localActor field of the AuthorityManager component of each shared object
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("shared"))
                {
                    sharedObjects.Add(go.GetComponent<NetworkIdentity>());

                    ParametersAuthorityManager paramAuthority = go.GetComponent<ParametersAuthorityManager>();
                    if (paramAuthority != null)
                    {
                        paramAuthority.AssignActor(this);
                    }

                    ViveParamsAuthorityManager viveParamAuthority = go.GetComponent<ViveParamsAuthorityManager>();
                    if (viveParamAuthority != null)
                    {
                        viveParamAuthority.AssignActor(this);
                    }

                }
            }
        }
        else
        {
            // Initialize on startup
            InitializeHands(prefabNameHand);
            InitializeFoot(prefabNameFoot);
        }
    }

    /// <summary>
    /// Updates the actor position and rotation.
    /// This function should be called only by the <see cref="LocalPlayerController"/>.
    /// </summary>
    public void UpdateActorLeft(Vector3 leftPos, Quaternion leftRot) //runs only on LocalPlayer!
    {
        if (character != null)
        {
            character.UpdateCharacterLeft(leftPos, leftRot);
        }
    }

    public void UpdateActorFoot(Vector3 footPos, Quaternion footRot)
    {
        if (footCharacter != null)
        {
            footCharacter.UpdateCharacterRight(footPos, footRot);
        }
    }

    /// <summary>
    /// Updates the actor position and rotation.
    /// This function should be called only by the <see cref="PlayerController"/>.
    /// </summary>
    public void UpdateActorRight(Vector3 rightPos, Quaternion rightRot) //runs only on LocalPlayer!
    {
        if (character != null)
        {
            character.UpdateCharacterRight(rightPos, rightRot);
        }
    }

    public void SetRightCharacterActive(bool active)
    {
        character.SetRightActive(active);
    }

    public void SetLeftCharacterActive(bool active)
    {
        character.SetLeftActive(active);
    }

    /// <summary>
    /// Initialize the player locally.
    /// </summary>
    /// <param name="prefab">Prefab character name.</param>
    public void InitializeHands(string prefab)
    {
        Debug.Log("InitializeFoot Hands: " + prefab);
        if (prefab.Contains("Hand"))
        {
            prefabNameHand = prefab;
            name = name.Replace("(Clone)", "");
        }
    }

    public void InitializeFoot(string prefab)
    {
        Debug.Log("InitializeFoot Foot: " + prefab);
        if (prefab.Contains("Foot"))
        {
            prefabNameFoot = prefab;
        }
    }

    /// <summary>
    /// Spawns the character of actor on all clients.
    /// This runs on server only.
    /// </summary>
    /// <param name="prefab">Prefab name of the character.</param>
    private void SpawnCharacter(string prefab)
    {
        // Spawn character
        GameObject modelPrefab = Resources.Load("Prefabs/" + prefab) as GameObject;
        GameObject model = (GameObject)Instantiate(modelPrefab, transform.position, transform.rotation) as GameObject;
        NetworkServer.Spawn(model);

        if (prefab.Contains("Hand"))
        {
            // Attach character to player
            AttachCharacter(model.GetComponent<Character>());
        }
        else if (prefab.Contains("Foot"))
        {
            // Attach character to player
            AttachFootCharacter(model.GetComponent<FootCharacter>());
        }
       
    }

    /// <summary>
    /// Initializes the character on server to inform all clients. 
    /// This command calls the Initialize() method and spawns the character.
    /// </summary>
    [Command]
    public void CmdInitialize(string prefab)
    {
        if (prefab.Length > 0)
        {
            CreateCharacter(prefab);
        }
    }

    

    /// <summary>
    /// Creates the character and initializes on server.
    /// </summary>
    /// <param name="prefab">The character prefab name.</param>
    [Server]
    public void CreateCharacter(string prefab)
    {
        SpawnCharacter(prefab);
        InitializeHands(prefab);
        InitializeFoot(prefab);
    }

    /// <summary>
    /// Main routine to attach the character to this actor
    /// This runs only on Server.
    /// </summary>
    /// <param name="newCharacter">New character to attach.</param>
    [Server]
    public void AttachCharacter(Character newCharacter)
    {
        newCharacter.AttachToActor(netId);
    }

    /// <summary>
    /// Main routine to attach the character to this actor
    /// This runs only on Server.
    /// </summary>
    /// <param name="newCharacter">New character to attach.</param>
    [Server]
    public void AttachFootCharacter(FootCharacter newFootCharacter)
    {
        newFootCharacter.AttachToActor(netId);
    }


    //this part is for object sharing
    // fill in the implementation
    //*******************************

    // should only be run on localPlayer (by the AuthorityManager of a shared object)
    // ask the server for the authority over an object with NetworkIdentity netID
    public void RequestObjectAuthority(NetworkIdentity netID)
    {
        Debug.Log("Actor: in RequestObjectAuthority");
        CmdAssignObjectAuthorityToClient(netID);
    }

    // should only be run on localPlayer (by the AuthorityManager of a shared object)
    // ask the server to remove the authority over an object with NetworkIdentity netID
    public void ReturnObjectAuthority(NetworkIdentity netID)
    {
        Debug.Log("Actor: in ReturnObjectAuthority");
        CmdRemoveObjectAuthorityFromClient(netID);
    }

    // run on the server
    // netID is NetworkIdentity of a shared object the authority if which should be passed to the client
    [Command]
    void CmdAssignObjectAuthorityToClient(NetworkIdentity netID)
    {
        Debug.Log("in CmdAssign");

        AuthorityManager authorityManager = netID.GetComponent<AuthorityManager>();
        ParametersAuthorityManager parametersAuthorityManager = netID.GetComponent<ParametersAuthorityManager>();
        ViveParamsAuthorityManager viveParamsAuthorityManager = netID.GetComponent<ViveParamsAuthorityManager>();

        if (authorityManager != null)
        {
            authorityManager.AssignClientAuthority(this.connectionToClient);
        }
        else if(parametersAuthorityManager != null)
        {
            parametersAuthorityManager.AssignClientAuthority(this.connectionToClient);
        }
        else if(viveParamsAuthorityManager != null)
        {
            viveParamsAuthorityManager.AssignClientAuthority(this.connectionToClient);
        }
    }

    // run on the server
    // netID is NetworkIdentity of a shared object the authority if which should be removed from the client
    [Command]
    void CmdRemoveObjectAuthorityFromClient(NetworkIdentity netID)
    {
        Debug.Log("in CmdRemove for shared object netid=" + netID);
        //netID.GetComponent<AuthorityManager>().RemoveClientAuthority(this.connectionToClient);

        AuthorityManager authorityManager = netID.GetComponent<AuthorityManager>();
        ParametersAuthorityManager parametersAuthorityManager = netID.GetComponent<ParametersAuthorityManager>();
        ViveParamsAuthorityManager viveParamsAuthorityManager = netID.GetComponent<ViveParamsAuthorityManager>();

        if (authorityManager != null)
        {
            Debug.Log("...for shared objects");
            authorityManager.RemoveClientAuthority(this.connectionToClient);
            return;
        }
        else if (parametersAuthorityManager != null)
        {
            Debug.Log("...for parametersAuthorityManager");
            parametersAuthorityManager.RemoveClientAuthority(this.connectionToClient);
            return;
        }
        else if (viveParamsAuthorityManager != null)
        {
            Debug.Log("...for viveParamsAuthorityManager");
            viveParamsAuthorityManager.RemoveClientAuthority(this.connectionToClient);
        }
    }
    //*******************************

    //#######################################################################################################################################

    public void CreateObject(string objPrefab, Vector3 pos, float objectScale)
    {
        Debug.Log("Actor: in CreateObject");
        if (!isLocalPlayer)
        {
            return;
        }
        Debug.Log("Actor: CmdCreateObject calling");
        CmdCreateObject(objPrefab, pos, objectScale);
    }

    [Command]
    public void CmdCreateObject(string objPrefab, Vector3 pos, float objectScale)
    {
        Debug.Log("Actor: create object in CmdCreateObject on position = " + pos);
        GameObject obj = Resources.Load("Prefabs/Objects/" + objPrefab) as GameObject;
        createdObject = Instantiate(obj);
        //createdObject.transform.localScale = createdObject.transform.localScale * objectScale;
        createdObject.transform.position = pos;
        //createdObject.GetComponent<AuthorityManager>().AssignActor(this);
        //sharedObjects.Add(createdObject.GetComponent<NetworkIdentity>());
        NetworkServer.Spawn(createdObject);
    }

    /// <summary>
    /// destroys the object
    /// </summary>
    /// <param name="obj"></param>
    [Command]
    public void CmdDestroyObject()
    {
        if (!createdObject) return;
        Debug.Log("Actor: Destroy object..." + createdObject.name);
        //sharedObjects.Remove(createdObject.GetComponent<NetworkIdentity>());
        NetworkServer.Destroy(createdObject);
    }
    //#######################################################################################################################################

}
