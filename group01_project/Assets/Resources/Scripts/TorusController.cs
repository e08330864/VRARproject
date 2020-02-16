using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TorusController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject lightningPrefab = null;
    private GameObject lightning = null;

    void Start()
    {
        if ((lightning = Instantiate(lightningPrefab, transform.position, Quaternion.identity)) == null)
        {
            Debug.LogError("lightning is NULL in StationController");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "TowerElement")
        {
            Debug.Log("StationController: is TowerElement");
            if (!other.gameObject.GetComponent<NetworkIdentity>().hasAuthority)
            {
                Debug.Log("StationController: no Authority");
                StartCoroutine(ShowLightning());
            }
        }
    }

    IEnumerator ShowLightning()
    {
        lightning.SetActive(true);
        yield return new WaitForSeconds(15);
        lightning.SetActive(false);
    }

}
