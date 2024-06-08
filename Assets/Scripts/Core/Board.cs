using System.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform m_emptySprite;
    public int m_heigth = 30;
    public int m_width = 10;
    public int m_header;

    Transform[,] m_grid;
    public int m_completedRows = 0;
    public ParticlePlayer[] m_rowGlowFX = new ParticlePlayer[4];

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

    bool IsWithinBoard(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0);
    }

    bool IsOccupied(int x, int y, Shape shape)
    {
        return(m_grid[x,y] != null && m_grid[x,y].parent != shape.transform);
    }

    public bool IsValidPosition(Shape shape)
    {
        foreach(Transform child in shape.transform)
        {
            Vector2 pos = VectorF.Round(child.position);

            if(!IsWithinBoard((int) pos.x, (int) pos.y))
            {
                return false;
            }

            if(IsOccupied((int) pos.x, (int) pos.y, shape))
            {
                return false;
            }
        }

        return true;
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

    public void StoreShapeInGrid(Shape shape)
    {
        if(shape == null)
        {
            return;
        }

        foreach(Transform child in shape.transform)
        {
            Vector2 pos = VectorF.Round(child.position);
            m_grid[(int) pos.x, (int) pos.y] = child;
        }
    }

    bool IsComplete(int y)
    {
        for(int x = 0; x < m_width; x++)
        {
            if(m_grid[x,y] == null)
            {
                return false;
            }
        }
        return true;
    }

    void ClearRow(int y)
    {
        for(int x = 0; x < m_width; x++)
        {
            if(m_grid[x,y] != null)
            {
                Destroy(m_grid[x,y].gameObject);
            }
            m_grid[x,y] = null;
        }
    }

    void ShiftOneRowDown(int y)
    {
        for (int i = 0; i < m_width; i++)
        {
            if(m_grid[i,y] != null)
            {
                m_grid[i, y-1] = m_grid[i,y];
                m_grid[i,y] = null;
                m_grid[i, y-1].position += new Vector3(0,-1,0);
            }
        }
    }

    void ShiftRowsDown(int startY)
    {
        for(int i = startY; i < m_heigth; i++)
        {
            ShiftOneRowDown(i);
        }
    }

    public IEnumerator ClearAllRows()
    {
        m_completedRows = 0;

        for(int y = 0; y < m_heigth; y++)
        {
            if(IsComplete(y))
            {
                ClearRowFX(m_completedRows, y);
                m_completedRows++;
            }
        }

        yield return new WaitForSeconds(.5f);
        for(int y = 0; y < m_heigth; y++)
        {
            if(IsComplete(y))
            {
                ClearRow(y);
                ShiftRowsDown(y+1);
                yield return new WaitForSeconds(.2f);
                y--;
            }
        }
    }

    void ClearRowFX(int id, int y)
    {
        if(m_rowGlowFX[id])
        {
            m_rowGlowFX[id].transform.position = new Vector3(5, y, -8);
            m_rowGlowFX[id].PlayParticles();
        }
    }

    public bool IsOverLimit(Shape shape)
    {
        foreach(Transform child in shape.transform)
        {
            if(child.transform.position.y >= (m_heigth - m_header - 1))
            {
                return true;
            }
        }

        return false;
    }

    
}
