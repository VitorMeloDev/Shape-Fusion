using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Shape[] m_allShapes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Shape GetRandomShape()
    {
        int i = Random.Range(0, m_allShapes.Length);
        if(m_allShapes[i])
        {
            return m_allShapes[i];
        }
        else
        {
            Debug.LogWarning("WARNING! Invalid shape index");
            return null;
        }
    }

    public Shape SpawnShape()
    {
        Shape shape = null;
        shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;

        if(shape)
        {
            return shape;
        }
        else
        {
            Debug.LogWarning("WARNING! Invalid shape index");
            return null;
        }
    }
}