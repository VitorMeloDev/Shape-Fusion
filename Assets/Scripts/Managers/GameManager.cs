using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{   
    Board m_board;
    Spawner m_spawner;
    Shape m_currentShape;
    SoundManager m_soundManager;
    ScoreManager m_scoreManager;
    public Ghost m_ghost;
    bool m_gameOver;

    enum Direction {none, left, right, up, down}
    Direction m_swipeDirection = Direction.none;
    Direction m_swipeEndDirection = Direction.none;

	public float m_dropInterval = 0.1f;
	float m_dropIntervalModded;
    float m_timeToDrop;
    bool m_isPaused = false;

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
    public GameObject m_pausePanel;

    private void OnEnable() 
    {
        TouchManager.SwipeEvent += SwipeHandler;
        TouchManager.SwipeEndEvent += SwipeEndHandler;
    }

    private void OnDisable() 
    {        
        TouchManager.SwipeEvent -= SwipeHandler;
        TouchManager.SwipeEndEvent -= SwipeEndHandler;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_board = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        m_scoreManager = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        m_ghost = GameObject.FindWithTag("Ghost").GetComponent<Ghost>();
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

        m_dropIntervalModded = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level * 0.1f), 0.05f, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        if(!m_spawner || !m_board || !m_currentShape || m_gameOver || !m_scoreManager)
        {
            return;
        }

        PlayerInput();
    }

    void LateUpdate() 
    {
        if(m_ghost)
        {
            m_ghost.DrawGhost(m_currentShape, m_board);
            Debug.Log("sasdas");
        }    
    }

    void PlayerInput()
    {
        if((Input.GetButton("MoveRight") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveRight"))
        {
            MoveRight();
        }
        else if((Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveLeft"))
        {
            MoveLeft();
        }
        else if(Input.GetButtonDown("Rotate") && (Time.time > m_timeToNextKeyRotate))
        {
            Rotate();
        }
        else if((Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown)) || (Time.time > m_timeToDrop))
        {
            MoveDown();
        }
        else if((m_swipeDirection == Direction.right && Time.time > m_timeToNextKeyLeftRight || m_swipeEndDirection == Direction.right))
        {
            MoveRight();

            m_swipeDirection = Direction.none;
            m_swipeEndDirection = Direction.none;
        }
        else if((m_swipeDirection == Direction.left && Time.time > m_timeToNextKeyLeftRight || m_swipeEndDirection == Direction.left))
        {
            MoveLeft();

            m_swipeDirection = Direction.none;
            m_swipeEndDirection = Direction.none;
        }
        else if((m_swipeDirection == Direction.down && Time.time > m_timeToNextKeyDown || m_swipeEndDirection == Direction.down))
        {
            MoveDown();

            m_swipeDirection = Direction.none;
            m_swipeEndDirection = Direction.none;
        }
        else if((m_swipeDirection == Direction.up && Time.time > m_timeToNextKeyRotate || m_swipeEndDirection == Direction.up))
        {
            Rotate();

            m_swipeDirection = Direction.none;
            m_swipeEndDirection = Direction.none;
        }
        else if(Input.GetButtonDown("Pause"))
        {
            Pause();
        }
    }

    private void MoveDown()
    {
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_timeToDrop = Time.time + m_dropIntervalModded;

        m_currentShape.MoveDown();

        if (!m_board.IsValidPosition(m_currentShape))
        {
            if (m_board.IsOverLimit(m_currentShape))
            {
                GameOver();
            }
            else
            {
                LandShape();
            }
        }
    }

    private void Rotate()
    {
        m_currentShape.RotateRight();
        m_timeToNextKeyRotate = Time.time + m_keyRepeatRateRotate;
        if (!m_board.IsValidPosition(m_currentShape)) { m_currentShape.RotateLeft(); PlaySound(m_soundManager.m_errorSound); } else { PlaySound(m_soundManager.m_moveSound); }
    }

    private void MoveLeft()
    {
        m_currentShape.MoveLeft();
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        if (!m_board.IsValidPosition(m_currentShape)) { m_currentShape.MoveRight(); } else { PlaySound(m_soundManager.m_moveSound); }
    }

    private void MoveRight()
    {
        m_currentShape.MoveRight();
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        if (!m_board.IsValidPosition(m_currentShape)) { m_currentShape.MoveLeft(); } else { PlaySound(m_soundManager.m_moveSound); }
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
        if(m_currentShape)
        {
            m_currentShape.MoveUp();
            m_currentShape.LandShapeFX();
            m_board.StoreShapeInGrid(m_currentShape);
            m_currentShape = m_spawner.SpawnShape();

            m_timeToNextKeyLeftRight = Time.time;
            m_timeToNextKeyDown = Time.time;
            m_timeToNextKeyRotate = Time.time;

            m_board.StartCoroutine("ClearAllRows");
            PlaySound(m_soundManager.m_dropSound);
            
            if(m_ghost)
            {
                m_ghost.Reset();
            }

            if (m_board.m_completedRows > 0)
            {
                m_scoreManager.ScoreLines(m_board.m_completedRows);

                if (m_scoreManager.didLevelUp)
                {
                    m_dropIntervalModded = Mathf.Clamp(m_dropInterval - ((float)m_scoreManager.m_level * 0.05f), 0.05f, 1f);
                    PlaySound(m_soundManager.m_levelUpVocalClip);
                }
                else
                {
                    if (m_board.m_completedRows > 1)
                    {
                        AudioClip randomVocal = m_soundManager.GetRandomAudioClip(m_soundManager.m_vocalClips);
                        PlaySound(randomVocal);
                    }
                }

                PlaySound (m_soundManager.m_clearRowSound);
            }

            PlaySound (m_soundManager.m_clearRowSound);
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        if(m_gameOver) {return;}

        m_isPaused = !m_isPaused;
        
        m_pausePanel.SetActive(m_isPaused);

        Time.timeScale = (m_isPaused) ? 0 : 1;
    }

    void SwipeHandler(Vector2 swipeMovement)
    {
        m_swipeDirection = GetDirection(swipeMovement);
    }

    void SwipeEndHandler(Vector2 swipeMovement)
    {
        m_swipeEndDirection = GetDirection(swipeMovement);
    }   

    Direction GetDirection(Vector2 swipeMovement)
    {
        Direction swipeDir = Direction.none;

        if(Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
        {
            swipeDir = (swipeMovement.x >=0) ? Direction.right : Direction.left;
        }
        else
        {
            swipeDir = (swipeMovement.y >= 0) ? Direction.up : Direction.down;
        }

        return swipeDir;
    }
}
