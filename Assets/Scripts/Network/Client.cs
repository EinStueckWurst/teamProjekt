using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour, INetEventListener
{
    [SerializeField] public UserConfiguration myUserConfig;
    [SerializeField] public GameObject lobbyPanel;
    [SerializeField] public ClientDataStorage clientData;

    [SerializeField] public GameController gameController;
    [SerializeField] Navigation navigationController;
    [SerializeField] public Lighting lighting;

    [SerializeField] TextMeshProUGUI passiveUserCount;
    [SerializeField] TextMeshProUGUI averagedLightDir;
    [SerializeField] TextMeshProUGUI averagedLightColor;

    public NetManager netManager;
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
                this.netManager.Stop();
                this.netManager = null;
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
            //If no Connections send Discovery
            if(this.netManager.ConnectedPeersCount == 0)
            {
                this.senDiscoveryRequest();
            }
            Thread.Sleep(15);
        }
    }

    /** Discover a Server
     * 
     */ 
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
                    this.ActivateOponentIcon();
                    this.passiveUserCount.SetText($"#PassiveUsers: {this.clientData.numOfPassiveUsers}");
                    Debug.Log("[CLIENT] Num of Passive Clients " + this.clientData.numOfActiveUsers);
                }
                break;
            case Action.INFORM_CLIENTS_ABOUT_MEAN_LIGHT_AVERAGE:
                if(container.dataModel == DataModel.LIGHT_DIRECTION)
                {
                    this.clientData.meanLightDir = container.MeanLightDir;
                    this.clientData.meanLightColor = container.MeanLightColor;
                    
                    this.lighting.reorientLightDir(this.clientData.meanLightDir);
                    this.lighting.reApplyLightColor(this.clientData.meanLightColor);

                    this.averagedLightDir.SetText($"MeanLightDir: {this.clientData.meanLightDir}");
                    this.averagedLightColor.SetText($"MeanLightColor: {this.clientData.meanLightColor}");


                    Debug.Log("[CLIENT] Mean Light Dir recieved: "+ this.clientData.meanLightDir);
                }
                break;
            case Action.START_GAME:
                if(container.dataModel == DataModel.TEAM)
                {
                    this.gameController.myTeam = container.team;
                    //Start Game Here
                    this.navigationController.startGameMenu();

                    Debug.Log("[CLIENT] TEAM Assigned: "+ this.gameController.myTeam);
                }
                break;
            case Action.MAKE_MOVE:
                if(container.dataModel == DataModel.MOVE)
                {
                    Team mTeam = container.team;
                    Vector2Int mOrigin = container.originPos;
                    Vector2Int mDest = container.destinationPos;

                    Debug.Log($"[CLIENT] TEAM {mTeam} -- from : {mOrigin} ---> to: {mDest}");

                    if (this.gameController.myTeam != mTeam)
                    {
                        this.gameController.applyRecievedMove(mOrigin, mDest);
                    }

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
    void ActivateOponentIcon()
    {
        this.lobbyPanel.transform.GetChild(1).gameObject.SetActive(true);
    }

    /** Disables all PlayerIcon In the Lobby
     * 
     */
    private void DeactivateOponentIcon()
    {
        this.lobbyPanel.transform.GetChild(1).gameObject.SetActive(false);
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
        this.DeactivateOponentIcon();
        this.StopClient();
        this.navigationController.menuAnimator.SetTrigger(Triggers.VIEW_LIGHT_ORIENTATION_PANEL);
        this.gameController.ResetChess();
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

    public void sendChessPieceMove(Team myTeam, Vector2Int origin, Vector2Int dest)
    {
        TransMissionContainerModel containerModel = new TransMissionContainerModel(
            Action.MAKE_MOVE,
            DataModel.MOVE
            );

        containerModel.originPos = origin;
        containerModel.destinationPos = dest;
        containerModel.team = myTeam;

        string json = JsonUtility.ToJson(containerModel);
        NetDataWriter writer = new NetDataWriter();
        writer.Put(json);

        this.netManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
    }
}
