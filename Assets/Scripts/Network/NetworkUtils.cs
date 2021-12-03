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

    public static void addUser(UserConfigModel userConfigModel, List<UserConfiguration> activeUsers, List<UserConfiguration> passiveUsers)
    {
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

    /** Returns the mean of all light directions ---> basically 1/n * (v1 + v2 + ... + vn)
     * 
     */ 
    public static Vector3 averageLightDirections(List<UserConfiguration> activeUsers)
    {
        Vector3 res = Vector3.zero;
        for(int i = 0; i < activeUsers.Count; i++)
        {
            res += activeUsers[i].getLightDir();
        }

        res /= activeUsers.Count;

        return res;
    }
}

public enum Action
{
    REGISTER_USER_CONFIGURATION,
    INFORM_CLIENTS_ABOUT_AMOUNT_OF_USERS,
    INFORM_CLIENTS_ABOUT_MEAN_LIGHT_AVERAGE,

}

public enum DataModel
{
    USER_CONFIG_MODEL,
    NUM_ACTIVE_AND_NUM_PASSIVE_USERS,
    LIGHT_DIRECTION,
}

