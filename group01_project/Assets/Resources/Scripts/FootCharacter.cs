using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FootCharacter : NetworkBehaviour {

    [SyncVar(hook = "SetActor")]
    private NetworkInstanceId actorId;
    private Actor actor;

    public Transform right;

    private bool rightActive = false;

    private void Awake()
    {
        name = name.Replace("(Clone)", "");
        
    }

    /// <summary>
    /// Initialize.
    /// </summary>
    private void Start()
    {
        if (isClient)
        {
            SetActor(actorId);
        }

    }

    public void UpdateCharacterRight(Vector3 rightPos, Quaternion rightRot)
    {
        if (!rightActive)
            SetRightActive(true);
        right.position = rightPos;
        right.rotation = rightRot;
    }

    public void SetRightActive(bool active)
    {
        right.gameObject.SetActive(active);
        rightActive = active;
    }

    // Update is called once per frame
    void Update () {
	
	}

    /// <summary>
    /// Sets the player as parent and initialize.
    /// </summary>
    /// <param name="id">The NetworkInstaceId of the player.</param>
    public void SetActor(NetworkInstanceId id)
    {
        actorId = id;

        if (!id.IsEmpty())
        {
            // Set new Player
            GameObject actorObject = NetworkServer.FindLocalObject(id);
            if (!actorObject)
            {
                actorObject = ClientScene.FindLocalObject(id);
            }

            actor = actorObject.GetComponent<Actor>();
            actor.footCharacter = this;
            transform.SetParent(actor.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

        }
    }

    /// <summary>
    /// Attaches the character to the player. This runs only on server.
    /// Note that the <see cref="connectionToClient"/> is only valid on the
    /// <see cref="Actor"/> on the server. This means that only the
    /// <see cref="actor"/> has a correct <see cref="connectionToClient"/>.
    /// </summary>
    /// <param name="id">The NetworkInstanceId of the new player.</param>
    [Server]
    public void AttachToActor(NetworkInstanceId id)
    {
 
        SetActor(id);

        if (actor.connectionToClient != null)
        {
            var networkIdentity = GetComponent<NetworkIdentity>();
            if (networkIdentity.localPlayerAuthority)
                networkIdentity.AssignClientAuthority(actor.connectionToClient);
        }
    }

}
