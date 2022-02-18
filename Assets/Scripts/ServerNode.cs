using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Globalization;

public class ServerNode : MonoBehaviour
{
    public GameObject chair;
    public GameObject ball;
    public GameObject box;
    public GameObject basestation;

    List<int> ListForValuesCheck = new List<int>(); 
    public int Mode;
    public float X;
    public float Y;
    public float Z;
    private TcpListener server=null;
    private TcpClient client=null;


    private Int32 port = 12345;
    // protected IPAddress localAddr = IPAddress.Parse("10.16.112.72"); // Lena comp
    protected IPAddress localAddr = IPAddress.Parse("10.16.96.240"); // Aleksey comp
    
    // Buffer for reading data
    private Byte [] bytes = new Byte[256];
    protected String data = null;

    private NetworkStream stream = null;

    static int numberOfBytesRead = 0;
    string resieved_str;

    // Start is called before the first frame update
    void Start()
    {
        chair.SetActive(false);
        ball.SetActive(false);
        box.SetActive(false);
        basestation.SetActive(false);

        IPEndPoint serverEndPoint = new IPEndPoint(localAddr, port);
        server = new TcpListener(serverEndPoint);

        ListForValuesCheck.Add(0);  
        ListForValuesCheck.Add(1);
        ListForValuesCheck.Add(2);
        ListForValuesCheck.Add(3);
        ListForValuesCheck.Add(4);   
    }

    // Update is called once per frame
    void Update()
    {
        if (server == null){
            Debug.Log("Server is not established");
            return;
        } else {
            server.Start();
        }

        if(client == null){
            Debug.Log("Waiting for a connection... ");
            if (server.Pending()){
                client = server.AcceptTcpClient();
                stream = client.GetStream();
                Debug.Log("Connected!");

            }
            return;
        }

        if (stream == null){
            Debug.Log("Stream is not established");
            // stream = client.GetStream();
        }

        numberOfBytesRead = stream.Read(bytes, 0, bytes.Length);
        data = System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytesRead);

        string[] words = data.Split(',');

        Mode = Convert.ToInt32(words[0]);

        if (Mode == -1){return;}

        if (ListForValuesCheck.Contains(Mode))
        {
            Debug.Log(Mode);
            X = float.Parse(words[1], CultureInfo.InvariantCulture.NumberFormat);
            Y = float.Parse(words[2], CultureInfo.InvariantCulture.NumberFormat);
            Z = float.Parse(words[3], CultureInfo.InvariantCulture.NumberFormat);

            if (Mode == 0)
            {
                chair.SetActive(true);
                // chair.transform.position = new Vector3(-13.34295f, 0.07629311f, -29.3037f);
            }
            else if (Mode == 1)
            {
                ball.SetActive(true);
                // ball.transform.position = new Vector3(-11.44f, 0.5f, -15.65f);
            }else if (Mode == 2)
            {
                box.SetActive(true);
            }else if (Mode == 3)
            {
                basestation.SetActive(true);
            }

            ListForValuesCheck.Remove(Mode);
        }else
        {
            Mode = -1;
        }


    }
}
