using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DestroyTrees : MonoBehaviour
{
    public float f_Radius = 5;
    private void Update()
    {
        Function_DestroyTrees(transform.position, f_Radius);
    }

    void Function_DestroyTrees(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);

        if (hitColliders.Length > 0)
        {
            //destroy some obj
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if(hitColliders[i].gameObject.tag == "DestructableEnvironment")
                {
                    Debug.Log("Destroyed: " + hitColliders[i].name);
                    Destroy(hitColliders[i].gameObject);
                }
            }
        }
    }
}
