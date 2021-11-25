using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RegisterUserModel
{
    public Action action;
    public DataModel dataModel;
    public UserConfigModel userConfigModel;

    public RegisterUserModel(
        UserConfigModel userConfigModel
        )
    {
        this.action = Action.REGISTER_USER_CONFIGURATION;
        this.dataModel = DataModel.USER_CONFIGURATION;
        this.userConfigModel = userConfigModel;
    }
}
