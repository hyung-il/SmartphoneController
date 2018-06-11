using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;




public class NetworkManager : MonoBehaviour
{
    public const short RotationMsgId = 888;
    public const short SelectMsgId = 999;
    public const short RecenterMsgId = 777;

    public Text networkStatus;

    public bool isServer;
    public string ip;
    public int port;

    NetworkClient myClient;

    private bool isAtStartup = true;

    float prevTime;

    NetworkServerSimple server;

    GameObject cube;
    GameObject ball;


    // Inverse rotation for recentering
    private Quaternion rotation;
    private Quaternion inverseInitialRotation = Quaternion.identity;

    // Use this for initialization
    void Start()
    {
        cube = GameObject.Find("Cube");
        server = new NetworkServerSimple();
        
        if (isServer)
        {
            SetupServer();
        }

        Physics.gravity = new Vector3(0, 0, 0);
        ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.localScale = Vector3.one * 0.1f;
        ball.transform.SetParent(cube.transform);
        ball.transform.localPosition = new Vector3(0, 0, 0.5f);
        ball.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        server.Update();
        cube.transform.rotation = inverseInitialRotation * rotation;
        if (Input.GetKeyDown(KeyCode.Space))
            Recenter();
        if (Input.GetKeyDown(KeyCode.S))
            OnSelect(null);
    }

    private void Recenter()
    {
        inverseInitialRotation = Quaternion.Inverse(rotation);
    }

    private void Recenter(NetworkMessage netMsg)
    {
        Recenter();
    }

    public void SetupServer()
    {
        server.Listen(4444);
        server.RegisterHandler(MsgType.Connect, OnClientConnected);
        server.RegisterHandler(RotationMsgId, OnGetRotation);
        server.RegisterHandler(SelectMsgId, OnSelect);
        server.RegisterHandler(RecenterMsgId, Recenter);
        //NetworkServer.Listen(4444);
        //NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
        //NetworkServer.RegisterHandler(RotationMsgId, OnGetRotation);
        isAtStartup = false;
        Debug.Log("Server Setup");
    }
    
   
    // Called when server is connected by client
    public void OnClientConnected(NetworkMessage netMsg)
    {
        Debug.Log("Client connected");
        
    }

    public void OnGetRotation(NetworkMessage netMsg)
    {
        RotationMessage msg = netMsg.ReadMessage<RotationMessage>();

        rotation.x = -msg.rot.x;
        rotation.y = -msg.rot.z;
        rotation.z = -msg.rot.y;
        rotation.w = msg.rot.w;

        //Debug.Log((Time.time - prevTime) + "s");
        //prevTime = Time.time;
    }

    public void OnSelect(NetworkMessage netMsg)
    {
        //GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //ball.transform.localScale = Vector3.one * 0.1f;
        ball.transform.SetParent(cube.transform);
        ball.transform.localPosition = new Vector3(0, 0, 0.5f);
        //ball.AddComponent<Rigidbody>();
        ball.transform.SetParent(null);

        ball.transform.localScale = Vector3.one * 0.1f;

        ball.GetComponent<Rigidbody>().velocity = (cube.transform.forward * 10);
        
    }
}

class RotationMessage : MessageBase
{
    public Quaternion rot;
    
}

class PressMessage : MessageBase
{
    public bool isPressed;
}