using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    Board m_board;
    Spawner m_spawner;
    Shape m_currentShape;
    SoundManager m_soundManager;
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

    public GameObject m_gameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        m_board = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        m_gameOverPanel.SetActive(false);

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
            if(!m_board.IsValidPosition(m_currentShape)){m_currentShape.MoveLeft();}else{PlaySound(m_soundManager.m_moveSound);}
        }
        else if(Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight) || Input.GetButtonDown("MoveLeft"))
        {
            m_currentShape.MoveLeft();
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            if(!m_board.IsValidPosition(m_currentShape)){m_currentShape.MoveRight();}else{PlaySound(m_soundManager.m_moveSound);}
        }
        else if(Input.GetButtonDown("Rotate") && (Time.time > m_timeToNextKeyRotate))
        {
            m_currentShape.RotateRight();
            m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
            if(!m_board.IsValidPosition(m_currentShape)){m_currentShape.RotateLeft(); PlaySound(m_soundManager.m_errorSound);}else{PlaySound(m_soundManager.m_moveSound);}
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
                    GameOver();
                }
                else
                {
                    LandShape();
                }
            }
        }
    }

    void GameOver()
    {
        PlaySound(m_soundManager.m_gameOverSound);
        PlaySound(m_soundManager.m_gameoverVocalClip);
        m_currentShape.MoveUp();
        m_gameOver = true;
        m_gameOverPanel.SetActive(true);
    }

    private void LandShape()
    {
        PlaySound(m_soundManager.m_dropSound);

        m_timeToNextKeyLeftRight = Time.time;
        m_timeToNextKeyDown = Time.time;
        m_timeToNextKeyRotate = Time.time;

        m_currentShape.MoveUp();
        m_board.StoreShapeInGrid(m_currentShape);
        m_currentShape = m_spawner.SpawnShape();

        m_board.ClearAllRows();

        if(m_board.m_completedRows > 0)
        {
            if(m_board.m_completedRows > 1)
            {
                PlaySound(m_soundManager.GetRandomAudioClip(m_soundManager.m_vocalClips));
            }
            else
            {
                PlaySound(m_soundManager.m_clearRowSound);
            }
        }
    }

    void PlaySound(AudioClip clip)
    {
        if(m_soundManager.m_fxEnabled && clip)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, m_soundManager.m_fxVolume);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
