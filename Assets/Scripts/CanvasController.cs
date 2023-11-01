using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    static public CanvasController Instance { get; private set; }

    public TMP_Text timeText;
    public TMP_Text scoreText;
    public TMP_Text bulletRemainText;
    public Slider healthBar;
    public GameObject sight;

    public float health { get; private set; }
    public float time { get; private set; }
    public float score { get; private set; }


    private void Start()
    {
        Instance = this;
    }

    public void SetHealth(float percent)
    {
        this.health = percent;
        this.healthBar.value = this.health;
    }

    public void AddScore(float value)
    {
        this.score += value;
        this.scoreText.text = "Score: " + this.score;
    }

    public void SetBulletRemainText(string text)
    {
        this.bulletRemainText.text = text;
    }

    public void SetVisibleSight(bool visible)
    {  
        this.sight.SetActive(visible);
    }

    private void Update()
    {
        this.time += Time.deltaTime;
        this.timeText.text = String.Format("Time: {0:00}:{1:00}", this.time / 60, this.time % 60);
    }
}
