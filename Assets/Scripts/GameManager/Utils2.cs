
using UnityEngine;

public class Utils2 : MonoBehaviour
{
    public static Utils2 Instance;
    private void Awake()
    {
        Debug.Log("Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
    }
    
    public Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2, bool asd)
    {
        var ray0 = camera.ScreenPointToRay(screenPosition1);
        Physics.Raycast(
            ray0,
            out var hit0);
        
        var ray1 = camera.ScreenPointToRay(screenPosition2);
        Physics.Raycast(
            ray1,
            out var hit1);
        
        var center = Vector3.Lerp(hit0.point, hit1.point, 0.5f);
        var size = (center - hit0.point) * 2;
        size.y += 10;
        size.x = Mathf.Abs(size.x);
        size.z = Mathf.Abs(size.z);
        Debug.Log(size);
        var bounds = new Bounds(center, size);
        // bounds.SetMinMax(min, max);
        return bounds;
    }
}
