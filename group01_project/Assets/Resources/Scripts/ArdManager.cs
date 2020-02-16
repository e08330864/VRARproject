using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArdManager : MonoBehaviour
{
    private SerialManager serial = null;
    private GameObject goToSetHeight = null;
    private bool heightSetupMode = false;
    private int height = 0;
    [SerializeField]
    private Color heightSetUpColor = new Color(0, 255, 0);
    private Color oldColor = new Color();
    private Renderer renderer = null;

    // Start is called before the first frame update
    void Start()
    {
        if ((serial = gameObject.GetComponent<SerialManager>()) == null)
        {
            Debug.LogError("serial is NULL in ArdManager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (heightSetupMode)
        {
            string input = serial.getSerialInput();
            if (input != null)
            {
                if (input == "&")
                {   // setup mode canceled
                }
                else
                {   // set new height
                    try
                    {
                        height = Convert.ToInt32(input);
                        goToSetHeight.transform.position = new Vector3(goToSetHeight.transform.position.x,
                            height / 1000f,
                            goToSetHeight.transform.position.z);
                    }
                    catch
                    {
                        Debug.Log("Wrong height input from Arduino");
                    }
                }
                // clean up
                if (renderer != null && oldColor != null)
                {
                    renderer.material.SetColor("_Color", oldColor);
                }
                goToSetHeight = null;
                heightSetupMode = false;
            }
        }

    }

    public void startSetupMode(GameObject go)
    {
        if (!heightSetupMode)
        {
            heightSetupMode = true;
            renderer = go.transform.Find("default").gameObject.GetComponent<Renderer>();
            oldColor = renderer.material.GetColor("_Color");
            renderer.material.SetColor("_Color", heightSetUpColor);
            serial.sendString("&");
            goToSetHeight = go;
        }
    }

    public bool getSetupModeStatus()
    {
        return heightSetupMode;
    }
}
