using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientDataStorage : MonoBehaviour
{
    public int portToBroadcast = 9999;
    public string MyNetworkId;

    //ServerPeerInformation just in case
    public NetPeer serverPeer;

    //Information pulled from Server
    public int numOfActiveUsers;
    public int numOfPassiveUsers;

    public Vector3 meanLightDir;
    public Color meanLightColor;
}
