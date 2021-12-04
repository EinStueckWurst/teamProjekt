using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour, INetEventListener
{
    [SerializeField] public UserConfiguration myUserConfig;
    [SerializeField] public GameObject lobbyPanel;
    [SerializeField] public ClientDataStorage clientData;

    NetManager netManager;
    public void StartClient()
    {
        this.netManager = new NetManager(this);
        this.netManager.Start();
        this.netManager.UnconnectedMessagesEnabled = true;
        this.netManager.BroadcastReceiveEnabled = true;
        this.netManager.UpdateTime = 15;

        this.clientData.MyNetworkId = this.myUserConfig.networkId;
        Debug.Log("CLIENT Started");
        Debug.Log("[Client] MY NETWORKID: " + this.myUserConfig.networkId);
    }

    public void StopClient()
    {
        if(this.myUserConfig.role != UserRole.LOBBY_CREATOR)
        {
            if (netManager != null)
            {
                this.netManager.DisconnectAll();
                this.netManager.Stop();
            }
            Debug.Log("CLIENT Stopped ");
        }
    }

    /* Update is called once per frame
     * 
     */
    private void Update()
    {
        if (this.netManager != null && this.netManager.IsRunning)
        {
            this.netManager.PollEvents();
            if(this.netManager.ConnectedPeersCount == 0)
            {
                this.senDiscoveryRequest();
            }
            Thread.Sleep(15);
        }
    }

    public void senDiscoveryRequest()
    {
        var peer = this.netManager.FirstPeer;

        if(peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
        } 
        else
        {
            UserConfigModel userConfigModel = NetworkUtils.toUserConfigurationModel(this.myUserConfig);
            string json = JsonUtility.ToJson(userConfigModel);

            NetDataWriter writer = new NetDataWriter();
            writer.Put(json);
            this.netManager.SendBroadcast(writer, this.clientData.portToBroadcast);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {

        this.clientData.serverPeer = peer;

        //Package myUserConfig into a TransmissionContainerModel and setup Action and Datamodel
        //so on the serverside you can execute with a switch case your desired Action
        UserConfigModel userConfigModel = NetworkUtils.toUserConfigurationModel(this.myUserConfig);
        TransMissionContainerModel transMissionContainerModel = new TransMissionContainerModel(
            Action.REGISTER_USER_CONFIGURATION, 
            DataModel.USER_CONFIG_MODEL);

        transMissionContainerModel.configModel = userConfigModel;
        string json = JsonUtility.ToJson(transMissionContainerModel);
        NetDataWriter writer = new NetDataWriter();
        writer.Put(json);

        
        //Triggering OnNetworkReceive on the serverside and execute the Desired Action
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        string json = reader.GetString();
        TransMissionContainerModel container = JsonUtility.FromJson<TransMissionContainerModel>(json);

        switch(container.action)
        {
            //Actions
            case Action.INFORM_CLIENTS_ABOUT_AMOUNT_OF_USERS:
                if(container.dataModel == DataModel.NUM_ACTIVE_AND_NUM_PASSIVE_USERS)
                {
                    this.clientData.numOfActiveUsers = container.NumActiveUsers;
                    this.clientData.numOfPassiveUsers = container.NumPassiveUsers;
                    UpdateLobbyAfterUserAdded(this.clientData.numOfActiveUsers);
                    Debug.Log("[CLIENT] Num of Passive Clients " + this.clientData.numOfActiveUsers);
                }
                break;
            case Action.INFORM_CLIENTS_ABOUT_MEAN_LIGHT_AVERAGE:
                if(container.dataModel == DataModel.LIGHT_DIRECTION)
                {
                    this.clientData.meanLightDir = container.MeanLightDir;
                    Debug.Log("[CLIENT] Mean Light Dir recieved: "+ this.clientData.meanLightDir);
                }
                break;
            default:
                Debug.Log("[Client] Action not detected!");
                break;
        }
    }

    /** Activates Users that are Visualized on the Server
     * 
     */
    void UpdateLobbyAfterUserAdded(int numActiveUsers)
    {
        this.DeactivateAllUsers();
        for (int i = 0; i < numActiveUsers; i++)
        {
            Transform t = this.lobbyPanel.transform.GetChild(i);
            if (!t.gameObject.activeInHierarchy)
            {
                this.lobbyPanel.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    /** Disables all PlayerGameobjects In the Lobby
     * 
     */
    private void DeactivateAllUsers()
    {
        for (int i = 0; i < this.lobbyPanel.transform.childCount; i++)
        {
            Transform t = this.lobbyPanel.transform.GetChild(i);
            if (!t.gameObject.activeInHierarchy)
            {
                this.lobbyPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (this.netManager.ConnectedPeersCount == 0 && reader.GetInt() == 1)
        {
            //Just Debug Info to check what myCLient has
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            Debug.Log("[Client] MY USER CONFIG NET ID: " + this.myUserConfig.networkId);
            Debug.Log("[Client] MY USER CONFIG IS ACTIVE: " + this.myUserConfig.isActive);
            Debug.Log("[Client] MY USER CONFIG Light DIr: " + this.myUserConfig.lightDir);
            Debug.Log("[Client] MY USER CONFIG ROLE: " + this.myUserConfig.role);

            //Package MyUserInformation into a Json
            UserConfigModel userConfigModel = NetworkUtils.toUserConfigurationModel(this.myUserConfig);
            string json = JsonUtility.ToJson(userConfigModel);
            NetDataWriter writer = new NetDataWriter();
            writer.Put(json);

            //Sending to Server a ConnectionRequest
            this.netManager.Connect(remoteEndPoint,writer);
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

    public void OnConnectionRequest(ConnectionRequest request)
    {
    }
}
