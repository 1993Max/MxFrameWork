using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SocketDemo : MonoBehaviour {

    private readonly string serverIp = "192.168.1.13";
    private readonly int port = 5000;

	// Use this for initialization
	void Start () {
        SocketTest();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SocketTest()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress iPAddress = IPAddress.Parse(serverIp);
        EndPoint endPoint = new IPEndPoint(iPAddress, port);
        socket.Connect(endPoint);

        byte[] data = new byte[1028];
        int length = socket.Receive(data);

        var message = Encoding.UTF8.GetString(data, 0, length);
        Debug.Log("收到了服务器发送的消息--->>  " + message);

        string msg = "Client to Server";
        socket.Send(Encoding.UTF8.GetBytes(msg));
    }
}
