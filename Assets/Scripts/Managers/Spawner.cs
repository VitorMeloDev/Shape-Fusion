using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Shape[] m_allShapes;

	public Transform[] m_queuedXforms = new Transform[3];

	// the actual Shapes in the queue
	Shape[] m_queuedShapes = new Shape[3] ;

	// the scale of the Shapes currently in the queue
	public float m_queueScale = 0.5f;

	void Awake()
	{
		InitQueue();
	}

    public Shape GetRandomShape()
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

        // use the Queue
		shape = GetQueuedShape();
		shape.transform.position = transform.position;
		shape.transform.localScale = Vector3.one;
        
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

    void InitQueue()
	{
		for (int i = 0; i < m_queuedShapes.Length; i++)
		{
			m_queuedShapes[i] = null;
		}
		FillQueue();
	}

	// fill any empty spaces in the queue with random shapes
	void FillQueue()
	{
		for (int i=0; i < m_queuedShapes.Length; i++)
		{
			if (!m_queuedShapes[i])
			{
				m_queuedShapes[i] = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape; 
				m_queuedShapes[i].transform.position = m_queuedXforms[i].position + m_queuedShapes[i].m_queueOffset;

				m_queuedShapes[i].transform.localScale = new Vector3(m_queueScale,m_queueScale,m_queueScale);
			}
		}
	} 

	// returns the first shape in the queue, then handles shifting the other elements and filling the empty space
	Shape GetQueuedShape()
	{
		Shape firstShape = null;

		// designate the 0 index Shape in the queue 
		if (m_queuedShapes[0])
		{
			firstShape = m_queuedShapes[0];
		}

		// set Shapes1,2... to 0,1,... and move their positions forward in the queue
		for (int i=1; i < m_queuedShapes.Length; i++)
		{
			m_queuedShapes[i-1] = m_queuedShapes[i];
			m_queuedShapes[i-1].transform.position = m_queuedXforms[i-1].position + m_queuedShapes[i].m_queueOffset;
		}

		// set the last space to null
		m_queuedShapes[m_queuedShapes.Length - 1] = null;

		// fill the empty resulting space after shifting
		FillQueue();

		// returns either the first Shape (or null if the queue is empty)
		return firstShape;
	}
}
