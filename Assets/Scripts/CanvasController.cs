using System;
using System.Collections;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    static public CanvasController Instance { get; private set; }

    private sealed class ReloadInfo
    {
        Image reload;
        TMP_Text text;

        public ReloadInfo(GameObject reloadImage)
        {
            this.reload = reloadImage.GetComponent<Image>();
            Transform textObj = reloadImage.transform.GetChild(1);
            this.text = textObj.GetComponent<TMP_Text>();
        }

        public void SetProgress(float progress)
        {
            this.reload.fillAmount = progress;
            this.text.text = (int)(progress * 100) + "%";
        }

        public void Reset()
        {
            SetProgress(1);
        }
    }


    public TMP_Text timeText;
    public TMP_Text scoreText;
    public TMP_Text bulletRemainText;
    public Slider healthBar;
    public GameObject sight;
    public GameObject reloadObject;
    public GameObject suggestReloadText;

    public float health { get; private set; }
    public float time { get; private set; }
    public float score { get; private set; }
    public bool suggestReloadTextVisible => this.suggestReloadText.activeInHierarchy;
    ReloadInfo reloadInfo;
    bool stopReload = false;


    private void Start()
    {
        Instance = this;
        this.reloadInfo = new ReloadInfo(this.reloadObject);
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
        if (this.sight.activeInHierarchy != visible)
        {
            this.sight.SetActive(visible);
        }
    }

    public void SetVisibleReload(bool visible)
    {
        if (this.reloadObject.activeInHierarchy != visible)
        {
            this.reloadObject.SetActive(visible);
        }
    }

    public IEnumerator CoroutineStartReload(float duration)
    {
        this.stopReload = false;
        this.SetVisibleReload(true);

        this.reloadInfo.Reset();
        float timePerTick = duration / 101.0f;

        for (int percent = 100; percent >= 0; percent--)
        {
            if (this.stopReload)
            {
                break;
            }

            this.reloadInfo.SetProgress(percent / 100.0f);
            yield return new WaitForSeconds(timePerTick);
        }

        this.SetVisibleReload(false);
    }

    public void ForceStopReload()
    {
        this.stopReload = true;
    }

    public void SetVisibleSuggestReloadText(bool visible)
    {
        if (this.suggestReloadText.activeInHierarchy != visible)
        {
            this.suggestReloadText.SetActive(visible);
        }
    }

    private void Update()
    {
        this.time += Time.deltaTime;
        this.timeText.text = String.Format("Time: {0:00}:{1:00}", this.time / 60, this.time % 60);
    }
}
