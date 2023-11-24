using Unity.Mathematics;

public static class SquadTools
{
    public static float3[] GetPosition(int totalChildren,float3 clickPoz,float3 willForwardDegree,float3[] formationData, float stoppingDistance)
    {
        if (stoppingDistance != 0)
        {
            float3 direction = math.normalize(willForwardDegree - clickPoz);
            //Move
            clickPoz -= stoppingDistance * direction;
        }
        float3[] result = new float3[totalChildren];
        float targetDegree = math.atan2(willForwardDegree[2] - clickPoz[2], willForwardDegree[0] - clickPoz[0]) * 180 / math.PI;
        if (targetDegree < 0)
            targetDegree += 360;
        quaternion quaternion = quaternion.RotateY(math.radians(targetDegree + 90));
        for (int i = 0; i < totalChildren; i++)
        {
            result[i] = math.mul(quaternion, formationData[i]) + clickPoz; 
        }
        return result;
    }
}