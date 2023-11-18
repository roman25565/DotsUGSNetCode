using Unity.Mathematics;
using UnityEngine;

public static class SquadTools
{
    public static float3[] getPosition(int totalChildren,float3 clickPoz,float3 willForwardDegree,float3[] formationData)
    {
        float3[] result = new float3[totalChildren];
        float targetDegree = math.atan2(clickPoz[2] - willForwardDegree[2] , clickPoz[0] - willForwardDegree[0]) * 180 / math.PI;
        if (targetDegree < 0) 
            targetDegree += 360;
        quaternion quaternion = quaternion.RotateY(targetDegree);
        Debug.Log(targetDegree);
        Debug.Log(quaternion);
        for (int i = 0; i < totalChildren; i++)
        {
            result[i] = math.mul(quaternion, formationData[i]); 
        }
        return result;
    }
}