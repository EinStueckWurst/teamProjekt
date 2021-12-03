using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TransMissionContainerModel
{
    public Action action;
    public DataModel dataModel;
    public UserConfigModel configModel;

    public int NumActiveUsers;
    public int NumPassiveUsers;
    
    public Vector3 MeanLightDir;

    public TransMissionContainerModel(
        Action action,
        DataModel dataModel,
        UserConfigModel configModel = null
        )
    {
        this.action = action;
        this.dataModel = dataModel;

        if (configModel != null)
        {
            this.configModel = configModel;
        }
    }
}
