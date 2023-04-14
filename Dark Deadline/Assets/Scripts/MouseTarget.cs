using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTarget : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

       if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
       {
           transform.position = raycastHit.point;
       }
        
    }
}
