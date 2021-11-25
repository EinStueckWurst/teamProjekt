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

    [SerializeField] public int numActiveClients = 1;
    [SerializeField] public int numPassiveClients = 0;


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

        Debug.Log("[SERVER] MY USER CONFIG: " + this.myUserConfig.networkId);
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
                Debug.Log("[SERVER] RECIEVED COnfig: " + reqDataJson);

                UserConfigModel userConfigModel = JsonUtility.FromJson<UserConfigModel>(reqDataJson);

                if (!NetworkUtils.userAlreadyExists(userConfigModel, this.activeUsers) && !NetworkUtils.userAlreadyExists(userConfigModel, this.passiveUsers))
                {
                    if (userConfigModel.isActive && this.activeUsers.Count <= this.maxActiveClients)
                    {
                        request.Accept();
                        Debug.Log("[SERVER] Client ConnectionRequest ACCEPTED");
                    }
                    else if (!userConfigModel.isActive)
                    {
                        request.Accept();
                        Debug.Log("[SERVER] Client ConnectionRequest ACCEPTED");
                    }
                    else
                    {
                        request.Reject();
                        Debug.Log("[SERVER] Client ConnectionRequest REJECTED");
                    }
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
            case Action.REGISTER_USER_CONFIGURATION:
                Debug.Log("Kann man adden: Registrtriert als User");
                if(container.dataModel == DataModel.USER_CONFIGURATION)
                {
                    RegisterUserModel registerUserModel = JsonUtility.FromJson<RegisterUserModel>(json);
                    NetworkUtils.addUser(registerUserModel, this.activeUsers, this.passiveUsers); //wird bei Connection Req bereits gerüft, ob der User bereits existiert

                }
                break;
            default:
                Debug.Log("Action not detected");
                break;

        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.Broadcast)
        {
            reader.Clear();
            Debug.Log("[SERVER] Received discovery request. Send discovery response");
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




///* Adds a User
// * 
// */
//private void addUser(UserConfigModel user)
//{
//    if (!NetworkUtils.userAlreadyExists(user,this.activeUsers) && !NetworkUtils.userAlreadyExists(user, this.passiveUsers))
//    {
//        if (user.isActive && this.activeUsers.Count <= this.maxActiveClients )
//        {
//            UserConfiguration uc = NetworkUtils.toUserConfiguration(user);
//            this.activeUsers.Add(uc);
//        }
//        else if(!user.isActive)
//        {
//            UserConfiguration uc = NetworkUtils.toUserConfiguration(user);
//            this.passiveUsers.Add(uc);
//        }
//    }
//}
