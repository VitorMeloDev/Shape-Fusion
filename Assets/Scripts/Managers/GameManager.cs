using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    Board m_board;
    Spawner m_spawner;
    Shape m_currentShape;
    bool m_gameOver;
    public float m_dropInterval = 0.9f;
    float m_timeToDrop;

    float m_timeToNextKeyLeftRight;
    [Range(0.02f,1f)]
    public float m_keyRepeatRateLeftRight = 0.25f;
    float m_timeToNextKeyDown;
    [Range(0.01f,1f)]
    public float m_keyRepeatRateDown = 0.25f;
    float m_timeToNextKeyRotate;
    [Range(0.02f,1f)]
    public float m_keyRepeatRateRotate = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        m_board = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;

        if(m_spawner)
        {
            m_spawner.transform.position = VectorF.Round(m_spawner.transform.position);

            if(m_currentShape == null)
            {
                m_currentShape = m_spawner.SpawnShape();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_spawner || !m_board || !m_currentShape || m_gameOver)
        {
            return;
        }

        PlayerInput();
    }

    void PlayerInput()
    {
        if(Input.GetButton("MoveRight") && (Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveRight"))
        {
            m_currentShape.MoveRight();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            if(!m_board.IsValidPosition(m_currentShape)){m_currentShape.MoveLeft();}
        }
        else if(Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveLeft"))
        {
            m_currentShape.MoveLeft();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            if(!m_board.IsValidPosition(m_currentShape)){m_currentShape.MoveRight();}
        }
        else if(Input.GetButtonDown("Rotate") && (Time.time > m_timeToNextKeyRotate))
        {
            m_currentShape.RotateRight();
            m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
            if(!m_board.IsValidPosition(m_currentShape)){m_currentShape.RotateLeft();}
        }
        else if(Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown) || (Time.time > m_timeToDrop))
        {
            m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
            m_timeToDrop = Time.time + m_dropInterval;

            m_currentShape.MoveDown();

            if(!m_board.IsValidPosition(m_currentShape))
            {
                if(m_board.IsOverLimit(m_currentShape))
                {
                    m_currentShape.MoveUp();
                    m_gameOver = true;
                    Debug.Log("Is over limit");
                }
                else
                {
                    LandShape();
                }
                
            }
        }
    }

    private void LandShape()
    {
        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;

        m_currentShape.MoveUp();
        m_board.StoreShapeInGrid(m_currentShape);
        m_currentShape = m_spawner.SpawnShape();

        m_board.ClearAllRows();
    }
}
