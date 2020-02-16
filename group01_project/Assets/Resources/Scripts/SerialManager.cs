using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;

public class SerialManager : MonoBehaviour
{
    [SerializeField]
    private string comPort = "COM5";
    [SerializeField]
    private int boudRate = 9600;

    private SerialPort sp;
    private List<String> input = new List<String>();
    private List<String> output = new List<String>();

    void Start()
    {
        sp = new SerialPort(comPort, boudRate);
        sp.DtrEnable = true;
        sp.RtsEnable = true;
        sp.Open();

        if (sp != null && sp.IsOpen)
        {
            Debug.Log("Serial port " + comPort + " is open!");
        }
        else
        {
            Debug.Log("Couldn't open serial port on " + comPort);
        }
        sp.ReadTimeout = 30;
        sp.WriteTimeout = 30;
        // Wait on Ardinio
        int counter = 100;
        while (counter > 0)
        {
            serialInput(sp);
            if (input.Count > 0)
            {
                if (0 == input[0].CompareTo("Arduino is ready"))
                {
                    input.RemoveAt(0);
                    break;
                }
                else
                {
                    Debug.Log("Arduino sent wrong initial message: " + input[0]);
                    input.RemoveAt(0);
                }
            }
            --counter;
            if (counter == 0)
            {
                Debug.Log("Timeout waiting for Arduino.");
                gameObject.SetActive(false);
            }
        }

        // Test...

        //for (int i = 0; i < 5; i++)
        //{
        //    sendString(i.ToString());
        //}
        //sendString("&");
    }

    void Update()
    {
        if (sp != null && sp.IsOpen)
        {
            // serial output
            if (output.Count > 0)
            {
                String send = output[0];
                sp.Write(send);
                output.RemoveAt(0);
            }
            // serial input
            serialInput(sp);
        }
    }


    private void serialInput(SerialPort port)
    {
        try
        {
            String line = "";
            while ((line = port.ReadLine()).Length > 0)
            {
                input.Add(line);
                Debug.Log("recieved: " + line);
            }
        }
        catch (TimeoutException e)
        {
        }
    }

    private void OnApplicationQuit()
    {
        if (sp != null)
        {
            if (sp.IsOpen)
            {
                Debug.Log("Closed serial port " + comPort);
                sp.Close();
            }
            sp = null;
        }
    }

    /// <summary>
    /// puts the text to be sent via serial port on stack for processing
    /// sending is processed in order of text input sequence
    /// </summary>
    /// <param name="text">text to be sent</param>
    public void sendString(String text)
    {
        output.Add(text + "\0");
    }

    /// <summary>
    /// gets input strings from serial port in order of input FIFO
    /// if no string is available the function returns null
    /// </summary>
    /// <returns>input string from port, null if no input is available</returns>
    public String getSerialInput()
    {
        if (input.Count > 0)
        {
            String ret = input[0];
            input.RemoveAt(0);
            return ret;
        }
        else
        {
            return null;
        }
    }
}