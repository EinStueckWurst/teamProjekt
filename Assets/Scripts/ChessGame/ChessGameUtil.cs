using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChessGameUtil
{
    /** Returns floored xy Coordinates as Vector3
     * 
     */
    public static Vector3 floorToIntVector3(Vector3 vec3)
    {
        int x = Mathf.FloorToInt(vec3.x);
        int y = Mathf.FloorToInt(vec3.z);
        return new Vector3(x, 0, y);
    }
    
    /** Returns floored xy Coordinates as Vector2Int
     * 
     */
    public static Vector2Int floorToIntVector2Int(Vector3 vec3)
    {
        int x = Mathf.FloorToInt(vec3.x);
        int y = Mathf.FloorToInt(vec3.z);
        return new Vector2Int(x, y);
    }
}
