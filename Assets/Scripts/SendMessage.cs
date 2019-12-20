using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using AsyncIO;
using NetMQ.Sockets;
using System.IO;
using System;
using System.Text;
using System.Linq;
using System.Threading;

public class SendMessage : MonoBehaviour
{
	Mutex mut = new Mutex();
	RequestSocket client;
	// Start is called before the first frame update
	void Start()
	{
		ForceDotNet.Force();
		client = new RequestSocket();
		client.Connect("tcp://localhost:5555");
		Debug.Log("connected");
	}

	// Update is called once per frame
	void Update()
	{

	}

	// generate a message when the game shuts down or switches to another Scene
	// or switched to ExampleClass2
	void OnDestroy()
	{
		mut.WaitOne();
		Debug.Log("Destroy");
		client.SendFrame("End");
		client.Close();
		NetMQConfig.Cleanup();
		mut.ReleaseMutex();
	}

	public void sendBytes(string type, string argument)
	{
		mut.WaitOne();
		System.Text.Encoding encoding = System.Text.Encoding.UTF8; //or some other, but prefer some UTF is Unicode is used
		if (type == "001" || type == "003")
			client.SendFrame(type + argument);
		else
		{
			byte[] string_bytes = encoding.GetBytes(type);
			byte[] bytes = File.ReadAllBytes(argument);
			byte[] final = new byte[string_bytes.Length + bytes.Length];
			System.Buffer.BlockCopy(string_bytes, 0, final, 0, string_bytes.Length);
			System.Buffer.BlockCopy(bytes, 0, final, string_bytes.Length, bytes.Length);
			client.SendFrame(final);
		}
		string message;
		bool gotMessage;
		while (true)
		{
			gotMessage = client.TryReceiveFrameString(out message);
			if (gotMessage) break;
		}
		mut.ReleaseMutex();
		if (gotMessage) Debug.Log("Received Message: " + message);
	}

    public void sendBytesInMemory(string type, byte[] bytes)
    {
        mut.WaitOne();
        System.Text.Encoding encoding = System.Text.Encoding.UTF8; //or some other, but prefer some UTF is Unicode is used
        byte[] string_bytes = encoding.GetBytes(type);
        byte[] final = new byte[string_bytes.Length + bytes.Length];
        System.Buffer.BlockCopy(string_bytes, 0, final, 0, string_bytes.Length);
        System.Buffer.BlockCopy(bytes, 0, final, string_bytes.Length, bytes.Length);
        client.SendFrame(final);
        string message;
        bool gotMessage;
        while (true)
        {
            gotMessage = client.TryReceiveFrameString(out message);
            if (gotMessage) break;
        }
        mut.ReleaseMutex();
        if (gotMessage) Debug.Log("Received Message: " + message);
    }
}