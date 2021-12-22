using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserRole
{
    LOBBY_CREATOR,
    LOBBY_JOINER,   //People who just join the lobby
    SPECTATOR,      //passive people are automatically Spectator
}

public class UserConfiguration : MonoBehaviour
{
    public string networkId;

    public bool isActive = false;
    public Vector3 lightDir = new Vector3(0, 0, 0);
    public Color lightCol = Color.black;
    public UserRole role = UserRole.SPECTATOR;

    public UserConfiguration() { }

    public UserConfiguration(
        string networkId,
        bool isActive,
        Vector3 lightDir,
        UserRole userRole
        )
    {
        this.networkId = networkId;
        this.isActive = isActive;
        this.lightDir = lightDir;
        this.role = userRole;
    }

    private void Start()
    {
        this.networkId = this.generateId();  
    }

    private string generateId()
    {
        return Guid.NewGuid().ToString("N");
    }

    public void setActive()
    {
        isActive = true;
    }
    public void setPassive()
    {
        isActive = false;
    }
    public bool getIsActive()
    {
        return isActive;
    }
    public void setLightDir(Vector3 lightDir)
    {
        this.lightDir = lightDir;
    }
    public Vector3 getLightDir()
    {
        return this.lightDir;
    }
    public void setLightCol(Color lightCol)
    {
        this.lightCol = lightCol;
    }
    public Color getLightCol()
    {
        return this.lightCol;
    }
    public void setUserToLobbyCreator()
    {
        this.role = UserRole.LOBBY_CREATOR;
    }
    public void setUserRoleToLobbyJoiner()
    {
        this.role = UserRole.LOBBY_JOINER;
    }
    public void setUserRoleToSpectator()
    {
        this.role = UserRole.SPECTATOR;
    }
}
