using UnityEngine;

public class Profundidad2D : MonoBehaviour
{
    void Start()
    {
        
        UnityEngine.Rendering.GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        UnityEngine.Rendering.GraphicsSettings.transparencySortAxis = new Vector3(0, 1, 0);
    }
}