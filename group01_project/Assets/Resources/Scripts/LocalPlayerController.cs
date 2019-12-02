using UnityEngine;
using System.Collections;

public class LocalPlayerController : MonoBehaviour {

    public Actor actor; // The network actor runs on all clients
    public Transform left;
    public Transform right;
    public Transform foot;

    public static LocalPlayerController Singleton { get; private set; }
    /// <summary>
    /// Sets the player on start up.
    /// </summary>
    /// <param name="newActor">New actor.</param>
    public void SetActor(Actor newActor)
    {
        actor = newActor;

        // Initialize locally to update on all clients
        Actor actorPrefab = (Resources.Load("Prefabs/" + actor.name.Replace("(Clone)", "")) as GameObject).GetComponent<Actor>();
        actor.transform.SetParent(transform);

        var prefabNameHands = "";
        prefabNameHands = actorPrefab.character != null ? actorPrefab.character.name : "";
        actor.InitializeHands(prefabNameHands);

        var prefabNameFoot = "";
        prefabNameFoot = actorPrefab.footCharacter != null ? actorPrefab.footCharacter.name : "";
        actor.InitializeFoot(prefabNameFoot);
        
    }

    private void Awake()
    {
        Singleton = this;
    }


    public void UpdateActorLeft(Vector3 leftPos, Quaternion leftRot)
    {
        actor.UpdateActorLeft(leftPos, leftRot);
    }

    public void UpdateActorRight(Vector3 rightPos, Quaternion rightRot)
    {
        actor.UpdateActorRight(rightPos, rightRot);
    }

    public void UpdateFoot(Vector3 footPos, Quaternion footRot)
    {
        //Debug.Log("Starting Update foot " + footPos.ToString());
        actor.UpdateActorFoot(footPos, footRot);
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if(actor)
        {
            if (left.gameObject.activeSelf)
                UpdateActorLeft(left.position, left.rotation);

            if (right.gameObject.activeSelf)
                UpdateActorRight(right.position, right.rotation);

            if (transform.parent != null && transform.parent.name.Contains("Vive"))
            {
                if (foot.gameObject.activeSelf)
                    UpdateFoot(foot.position, foot.rotation);
            }
        }
      
    }
}
