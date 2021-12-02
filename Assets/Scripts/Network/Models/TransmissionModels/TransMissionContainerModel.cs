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

    public TransMissionContainerModel(
        Action action,
        DataModel dataModel,
        UserConfigModel configModel = null,
        int NumActiveUsers = -1, //null geht nicht --- also ist es -1
        int NumPassiveUsers = -1
        )
    {
        this.action = action;
        this.dataModel = dataModel;

        if (configModel != null)
        {
            this.configModel = configModel;
        } 
        if (NumActiveUsers != -1)
        {
            this.NumActiveUsers = NumActiveUsers;
        }        
        if (NumPassiveUsers != -1)
        {
            this.NumPassiveUsers = NumPassiveUsers;
        }
    }
}
