using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public bool m_canRotate;
	public Vector3 m_queueOffset;

    GameObject[] m_glowFX;
    public string glowTag = "GlowFX";
    
    private void Start() 
    {
        if(glowTag != "")
        {
            m_glowFX = GameObject.FindGameObjectsWithTag(glowTag);
        }
    }
    public void LandShapeFX()
    {
       int i = 0;

       foreach(Transform child in gameObject.transform)
       {
            if(m_glowFX[i])
            {
                m_glowFX[i].transform.position = new Vector3(child.position.x, child.position.y, -8f);
                ParticlePlayer particlePlayer = m_glowFX[i].GetComponent<ParticlePlayer>();

                if(particlePlayer)
                {
                    particlePlayer.PlayParticles();
                }
            }
            i++;
        }
    }

    public void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }

    public void MoveLeft()
    {
        Move(new Vector3(-1,0,0));
    }

    public void MoveRight()
    {
        Move(new Vector3(1,0,0));
    }

    public void MoveDown()
    {
        Move(new Vector3(0,-1,0));
    }

    public void MoveUp()
    {
        Move(new Vector3(0,1,0));
    }

    public void RotateRight()
    {
        if(!m_canRotate){return;}
        transform.Rotate(0,0, -90);
    }

    public void RotateLeft()
    {
        if(!m_canRotate){return;}
        transform.Rotate(0,0, 90);
    }
}
