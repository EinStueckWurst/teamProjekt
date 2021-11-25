using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserConfigModel
{
    public string networkId;

    public bool isActive = false;
    public Vector3 lightDir = new Vector3(0, 0, 0);
    public UserRole role = UserRole.SPECTATOR;

    public UserConfigModel(
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
}
