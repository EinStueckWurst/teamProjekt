using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerDataStorage : MonoBehaviour
{
    public int serverPort = 9999;
    public string MyNetworkId;
    public int maxActiveClients = 4; //included Host

    public List<UserConfiguration> activeUsers;
    public List<UserConfiguration> passiveUsers;

    public Vector3 meanLightDir;
    public Color meanLightColor;
}
