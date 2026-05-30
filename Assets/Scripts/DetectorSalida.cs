using UnityEngine;

public class DetectorSalida : MonoBehaviour
{
    public Vector2 direccion; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            WorldGenerator wg = FindAnyObjectByType<WorldGenerator>();
            if (wg != null)
            {
                wg.IrASala(direccion);
            }
        }
    }
}