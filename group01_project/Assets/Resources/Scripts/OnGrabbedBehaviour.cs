using UnityEngine;
using UnityEngine.Networking;

// TODO: define the behaviour of a shared object when it is manipulated by a client

public class OnGrabbedBehaviour : MonoBehaviour
{
    bool held;
    //Actor currentActor;

    Rigidbody rigidbody = null;
    NetworkIdentity netId = null;
    AuthorityManager authorityManager = null;
    GameObject handLeft = null;
    GameObject handRight = null;
    Actor actor = null;

    // Use this for initialization
    void Start ()
    {
        if ((authorityManager = GetComponent<AuthorityManager>()) == null) { Debug.LogError("authorityManager is NULL in OnGrabbedBehaviour"); }
        if ((rigidbody = GetComponent<Rigidbody>()) == null) { Debug.LogError("rigidbody is NULL in OnGrabbedBehaviour"); }
        if ((netId = GetComponent<NetworkIdentity>()) == null) { Debug.LogError("netId is NULL in OnGrabbedBehaviour"); } 
    }

    // Update is called once per frame
    void Update()
    {
        GetHands();
        if (handLeft != null && handRight != null && held)
        {
            // Debug.Log("OnGrabbedBehaviour: held = true");
            if (authorityManager.GetActor() == null || !netId.hasAuthority) return;
            // Debug.Log("OnGrabbedBehaviour: authorityManager.GetActor().gameObject.transform.parent.tag = " + authorityManager.GetActor().gameObject.transform.parent.tag);
            // handLeft = GameObject.FindGameObjectWithTag("LeftHandInteraction");
            if (authorityManager.GetLeftGrabbed())  // held with left hand
            {
                this.transform.position = handLeft.transform.position;
            } else
            {
                this.transform.position = handRight.transform.position;
            }
        }
    }

    // called first time when the GO gets grabbed by a player
    public void OnGrabbed()
    {
        Debug.Log("OnGrabbedBehaviour: OnGrabbed...isKinematic=true, useGravity=false");
        held = true;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

    }

    // called when the GO gets released by a player
    public void OnReleased()
    {
        Debug.Log("OnGrabbedBehaviour: OnReleased...isKinematic=false, useGravity=true");
        held = false;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }

    private void GetHands()
    {
        if (actor == null && (actor = authorityManager.GetActor()) != null)
        {
            if (actor.gameObject.transform.parent.tag == "vive")
            {
                handLeft = authorityManager.GetActor().transform.Find("ViveHands/Left").gameObject;
                handRight = authorityManager.GetActor().transform.Find("ViveHands/Right").gameObject;
            }
            else if (actor.gameObject.transform.parent.tag == "leap")
            {
                handLeft = GameObject.FindGameObjectWithTag("LeftHandInteraction");
                handRight = GameObject.FindGameObjectWithTag("RightHandInteraction");
            }
        }
    }
}
