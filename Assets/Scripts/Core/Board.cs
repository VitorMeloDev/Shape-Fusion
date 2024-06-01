using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Mathematics;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform m_emptySprite;
    public int m_heigth = 30;
    public int m_width = 10;
    public int m_header;

    Transform[,] m_grid;

    void Awake() 
    {
        m_grid = new Transform[m_width, m_heigth];
    }
    // Start is called before the first frame update
    void Start()
    {
        DrawEmptyCells();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawEmptyCells()
    {
        if(m_emptySprite != null)
        {
            for(int y = 0; y < m_heigth - m_header; y++)
            {
                for(int x = 0; x < m_width; x++)
                {
                    Transform clone;
                    clone = Instantiate(m_emptySprite, new Vector3(x,y,0), Quaternion.identity) as Transform;
                    clone.name = "Board Space ( x = " + x.ToString() + ", y = " + y.ToString () + " )";
                    clone.transform.parent = transform;
                }
            }
        }
        else
        {
            Debug.Log("WARNING! Please assign the emptySprite objetc!");
        }
    }
}