using UnityEngine;
using UnityEngine.Networking;

// TODO: define the behaviour of a shared object when it is manipulated by a client

public class OnGrabbedBehaviour : MonoBehaviour
{
    bool grabbed;
    Actor currentActor;

    Rigidbody rigidbody;
    NetworkIdentity netId;

    // Use this for initialization
    void Start () {

        if ((rigidbody = GetComponent<Rigidbody>()) == null) { Debug.LogError("rigidbody is NULL in OnGrabbedBehaviour"); }
        if ((netId = GetComponent<NetworkIdentity>()) == null) { Debug.LogError("netId is NULL in OnGrabbedBehaviour"); }
    }
	
	// Update is called once per frame
	void Update () {
        if (currentActor == null || !netId.hasAuthority) return;
        GameObject leftHandGO = currentActor.transform.Find("ViveHands/Left").gameObject;
        GameObject rightHandGO = currentActor.transform.Find("ViveHands/Right").gameObject;

        if (leftHandGO && rightHandGO)
        {
            Transform leftHand = leftHandGO.transform;
            Transform rightHand = rightHandGO.transform;
            
            // GO´s behaviour when it is in a grabbed state (owned by a client) should be defined here
            if (grabbed)
            {
                this.transform.position = (leftHand.position + rightHand.position) / 2.0f;
            }
        }
	}

    // called first time when the GO gets grabbed by a player
    public void OnGrabbed(Actor actor)
    {
        currentActor = actor;
        grabbed = true;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    // called when the GO gets released by a player
    public void OnReleased()
    {
        grabbed = false;
        currentActor = null;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }
}
