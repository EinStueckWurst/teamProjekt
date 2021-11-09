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
    private bool isActive = false;
    private Vector3 lightDir = new Vector3(0, 0, 0);
    private UserRole role = UserRole.SPECTATOR;

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
