using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkUtils 
{
    public static UserConfiguration toUserConfiguration(UserConfigModel userConfigModel)
    {
        UserConfiguration userConfig = new UserConfiguration(
            userConfigModel.networkId,
            userConfigModel.isActive,
            userConfigModel.lightDir, 
            userConfigModel.role
            );
        return userConfig;
    }

    public static UserConfigModel toUserConfigurationModel(UserConfiguration userConfig)
    {
        UserConfigModel userConfigModel = new UserConfigModel(
            userConfig.networkId,
            userConfig.isActive,
            userConfig.lightDir,
            userConfig.role
            );

        return userConfigModel;
    }

    public static bool userAlreadyExists(UserConfigModel userConfigModel, List<UserConfiguration> userConfigurations)
    {
        foreach(UserConfiguration uc in userConfigurations)
        {
            if(uc.networkId == userConfigModel.networkId)
            {
                return true;
            }
        }

        return false;
    }

    public static void addUser(RegisterUserModel registerUserModel, List<UserConfiguration> activeUsers, List<UserConfiguration> passiveUsers)
    {
        UserConfigModel userConfigModel = registerUserModel.userConfigModel;
        UserConfiguration uc = NetworkUtils.toUserConfiguration(userConfigModel);

        if (uc.isActive)
        {
            activeUsers.Add(uc);
        }
        else
        {
            passiveUsers.Add(uc);
        }
    }
}

public enum Action
{
    REGISTER_USER_CONFIGURATION
}

public enum DataModel
{
    USER_CONFIGURATION
}

