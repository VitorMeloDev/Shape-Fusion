using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    Shape m_ghostShape = null;
    bool m_hitBottom = false;
    public Color m_color = new Color(1,1,1,0.2f);

    public void DrawGhost(Shape originalShape, Board board)
    {
        if(!m_ghostShape)
        {
            m_ghostShape = Instantiate(originalShape, originalShape.transform.position, originalShape.transform.rotation) as Shape;
            m_ghostShape.gameObject.name = "GhostShape";

            SpriteRenderer[] allRenderers = m_ghostShape.GetComponentsInChildren<SpriteRenderer>();

            foreach(SpriteRenderer renderer in allRenderers)
            {
                renderer.color = m_color;
            }
        }
        else
        {
            m_ghostShape.transform.position = originalShape.transform.position;
            m_ghostShape.transform.rotation = originalShape.transform.rotation;
        }

        m_hitBottom = false;

        while(!m_hitBottom)
        {
            m_ghostShape.MoveDown();

            if(!board.IsValidPosition(m_ghostShape))
            {
                m_ghostShape.MoveUp();
                m_hitBottom = true;
            }
        }
    }

    public void Reset()
    {
        Destroy(m_ghostShape.gameObject);
    }
}
