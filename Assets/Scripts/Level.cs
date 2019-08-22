﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class Level : MonoBehaviour
{

    SceneLoader sceneLoader;
    [SerializeField] AudioClip startGame;

    [SerializeField] AudioClip newWave;

    [SerializeField] int numberOfGhosts = 0;

    [SerializeField] int lives = 3;

    [SerializeField] GameObject[] livesImages;
    GameObject lifeImage;
    [SerializeField] Sprite blankImage;

    [SerializeField] int score = 0;

    [SerializeField] TextMeshProUGUI scoreText;

    public BonusGhost bonusGhost;
    float spawnTime = 15;
    Vector2 spawnPoint;

    [SerializeField] Ghost[] ghosts;

    [SerializeField] GameObject ghostsPrefab;

    [SerializeField] Transform ghostsSpwan;

    GameObject ghostObject;

    GhostHiveMind ghostHiveMind;


    int wave = 0;

    private void Awake()
    {
        int levelCount = FindObjectsOfType<Level>().Length;
        if (levelCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else 
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();
    
        InvokeRepeating("addBonusGhost", 0, spawnTime);

        ghosts = FindObjectsOfType<Ghost>();

        AudioSource.PlayClipAtPoint(startGame, Camera.main.transform.position);

        ghostHiveMind = FindObjectOfType<GhostHiveMind>();

        Invoke("AssignFruit", 2);

        lifeImage = livesImages[0];

    }

    void OnEnable()
    {
        
    }

    void onDisable()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerScores.dat", FileMode.Open);

        PlayerScores playerScores = new PlayerScores();
        playerScores.scores.Add(score);

        bf.Serialize(file, playerScores);
        file.Close();
    }

    public void IncreaseLives()
    {
        if (lives < 3)
        {
            livesImages[lives].GetComponent<UnityEngine.UI.Image>().sprite = lifeImage.GetComponent<UnityEngine.UI.Image>().sprite;
            lives++;

        }
    }

    public void AssignFruit()
    {
        ghostHiveMind.AssignFruit();
    }

    public int GetEndScore()
    {
        return score;
    }

    public int GetLives()
    {
        return lives;
    }

    public int GetWaveNumber()
    {
        return wave;
    }

    void Update()
    {
        if (scoreText)
        {
            UpdateScoreText();
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            CancelInvoke();
        }
    }

    public void ResetGame()
    {
        Destroy(gameObject);
    }

    public void CountGhosts()
    {
        numberOfGhosts++;
    }

    public void RemoveGhost()
    {
        numberOfGhosts--;
        if (numberOfGhosts <= 0)
        {
            NextGhostWave();
            ghosts = FindObjectsOfType<Ghost>();
            wave += 1;
            IncreaseGhostSpeed();
        }
    }

    private void IncreaseGhostSpeed()
    {
        int x = 0;
        while (x < wave)
        {
            foreach (var ghost in ghosts)
            {
                ghost.IncreaseSpeed();
            }
            x++;
        }
    }

    public void LoseLife()
    {
        lives -= 1;
        if (lives <= 0)
        {
            FindObjectOfType<Pacman>().Die();
            sceneLoader.LoadLoseScene();
        }
        livesImages[lives].GetComponent<UnityEngine.UI.Image>().sprite = blankImage;
    }

     private void NextGhostWave()
    {
        Vector3 startPosition= new Vector3(0.0f, 5.5f, 0.0f);
        ghostObject = Instantiate(ghostsPrefab, startPosition, ghostsSpwan.rotation);
        StartCoroutine(MoveToPosition(ghostObject.GetComponent<Transform>(), new Vector3(0.0f, 0.0f, 0.0f), 4));
        AudioSource.PlayClipAtPoint(newWave, Camera.main.transform.position);
        Invoke("AssignFruit", 2);
    }

    public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while(t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    public void UpdateScoreGhost()
    {
        score += 20;
    }

    public void UpdateScoreBonusGhost()
    {
        score += 150;
    }

    public void UpdateScoreFruit()
    {
        score += 30;
    }

    public void UpdateScoreCherry()
    {
        score += 100;
    }

    public void RemoveTextScoreAndLives()
    {
        Destroy(scoreText);
    }

    private void addBonusGhost()
    {
        var spawnPointLeft = new Vector2(-1.39f, 9.32f);
        var spawnPointRight = new Vector2(13.39f, 9.32f);

        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            spawnPoint = spawnPointLeft;
            bonusGhost.speed = 2.0f;
        } else
        {
            spawnPoint = spawnPointRight;
            bonusGhost.speed = -2.0f;
        }

        Instantiate(bonusGhost, spawnPoint, Quaternion.identity);
    }
}
[Serializable]
class PlayerScores
{
    public List<int> scores;
}