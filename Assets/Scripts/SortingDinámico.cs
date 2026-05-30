using UnityEngine;

public class SortingDinamico : MonoBehaviour
{
    private SpriteRenderer[] renderers;
    public Transform objetoCopa; 
    
    void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        if (objetoCopa == null)
        {
            Transform copaFound = transform.Find("copa");
            if (copaFound != null) objetoCopa = copaFound;
        }
    }

    void Update()
    {

        if (this == null || gameObject == null) return;

        if (renderers != null)
        {
            foreach (SpriteRenderer sr in renderers)
            {

                if (sr == null) continue; 

                if (sr.transform == objetoCopa)
                {
                    sr.sortingOrder = 5000;
                }
                else
                {
                    sr.sortingOrder = -(int)(transform.position.y * 10) - 1;
                }
            }
        }
    }
}