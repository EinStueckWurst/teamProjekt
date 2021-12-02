using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour, INetEventListener
{
    [SerializeField] public UserConfiguration myUserConfig;
    [SerializeField] public List<UserConfiguration> activeUsers;
    [SerializeField] public List<UserConfiguration> passiveUsers;

    [SerializeField] public int serverPort = 9999;
    [SerializeField] public int maxActiveClients = 4; //included Host

    [SerializeField] public int numActiveClients = 1; //1 because Host is active
    [SerializeField] public int numPassiveClients = 0;

    [SerializeField] public GameObject lobbyPanel;


    NetManager netManager;

    /* Initializes the Server
     * 
     */
    public void startServer()
    {
        this.netManager = new NetManager(this);

        this.netManager.Start(serverPort);
        this.netManager.BroadcastReceiveEnabled = true;
        this.netManager.UnconnectedMessagesEnabled = true;
        this.netManager.UpdateTime = 15;
        this.netManager.ReuseAddress = true;

        this.activeUsers.Add(myUserConfig);
        Debug.Log("SERVER STARTED");

        Debug.Log("[SERVER] MY NETWORKID: " + this.myUserConfig.networkId);
    }

    /* Stops the Server
     * 
     */
    public void StopServer()
    {
        if(this.myUserConfig.role == UserRole.LOBBY_CREATOR)
        {
            if (this.netManager != null)
            {
                this.netManager.Stop();
            }
            Debug.Log("SERVER Stopped");
        }
    }

    /* Update is called once per frame
     * 
     */
    void Update()
    {
        if (this.netManager != null && this.netManager.IsRunning)
        {
            this.netManager.PollEvents();
        }
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        if(request.Data != null)
        {
            string reqDataJson = request.Data.GetString();
            if (reqDataJson != null)
            {
                //Deserialize json into UserConfigModel class
                UserConfigModel userConfigModel = JsonUtility.FromJson<UserConfigModel>(reqDataJson);

                //Check if User is already registered -- basically just check wether user is in activeUsers list or passiveUsers list
                if (!NetworkUtils.userAlreadyExists(userConfigModel, this.activeUsers) && !NetworkUtils.userAlreadyExists(userConfigModel, this.passiveUsers))
                {
                    if (userConfigModel.isActive && this.activeUsers.Count <= this.maxActiveClients)
                    {
                        //Accept Connection and trigger OnPeerConnected for the Client
                        request.Accept();
                        Debug.Log("[SERVER] Client ConnectionRequest ACCEPTED");
                    }
                    else if (!userConfigModel.isActive)
                    {
                        //Accept Connection and trigger OnPeerConnected for the Client
                        request.Accept();
                        Debug.Log("[SERVER] Client ConnectionRequest ACCEPTED");
                    }
                    else
                    {
                        request.Reject();
                        Debug.Log("[SERVER] Client ConnectionRequest REJECTED");
                    }
                }
                else
                {
                    request.Reject();
                    Debug.Log("[SERVER] Client ConnectionRequest REJECTED");
                }
            }
        }
        
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[Server] Peer has been connected with endpoint : " + peer.EndPoint.ToString());
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        string json = reader.GetString();
        TransMissionContainerModel container = JsonUtility.FromJson<TransMissionContainerModel>(json);

        switch (container.action)
        {
            //Action to add the User
            case Action.REGISTER_USER_CONFIGURATION:
                if(container.dataModel == DataModel.USER_CONFIG_MODEL)
                {
                    UserConfigModel userConfigModel = container.configModel;
                    NetworkUtils.addUser(userConfigModel, this.activeUsers, this.passiveUsers); //wird bei ConnectionReq bereits geprüft, ob der User bereits existiert
                    UpdateLobbyAfterUserAdded();
                    Debug.Log("SERVER : Amount Of Passive Users: " + this.passiveUsers.Count);
                    //UpdateAllConnectedClients();
                }
                break;
            default:
                Debug.Log("Action not detected");
                break;

        }
    }

    /** Just Activates any new Users (Index of these activated Users is the same as the index of the activeUser list)
     * 
     */ 
    void UpdateLobbyAfterUserAdded()
    {
        for(int i = 0; i < this.activeUsers.Count; i++)
        {
            Transform t = this.lobbyPanel.transform.GetChild(i);
            if(!t.gameObject.activeInHierarchy)
            {
                this.lobbyPanel.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.Broadcast)
        {
            NetDataWriter resp = new NetDataWriter();
            resp.Put(1);
            this.netManager.SendUnconnectedMessage(resp, remoteEndPoint);
        }
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }
}
