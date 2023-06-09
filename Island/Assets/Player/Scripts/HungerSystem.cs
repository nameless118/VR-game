using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerSystem : MonoBehaviour
{
    [SerializeField, Tooltip("������� ������ �������, ����� ������� ���� ������� �������")] int secondsPerPercentSatiety = 2;
    [SerializeField, Tooltip("��������� ������� � ���������"), Range(0, 1)] float startSatiety = 0.7f;
    [SerializeField] int secondsPerHealthPoint = 2;
    [SerializeField, Range(0, 1f)] float startHealth = 1;

    [SerializeField] Image healthBar;
    [SerializeField] Image satietyBar;

    [SerializeField] GameObject overlay;
    [SerializeField] GameObject deathScreen;

    bool isGameOver = false;
    public bool IsGameOver { 
        get { return isGameOver; }
        set { isGameOver = value; }
    }

    float satiety, health;
    public float Satiety {
        get { return satiety; }
        set { satiety = value; }
    }

    IEnumerator _StarvingProcess, _DamageFromStarvingProcess;
    private void Awake() {
        satiety = startSatiety;
        health = startHealth;
        _StarvingProcess = StarvingProcess(); _DamageFromStarvingProcess = DamageFromStarvingProcess();
        StartCoroutine(_StarvingProcess);
    }
    private void Start() {
        healthBar.fillAmount = health;
        satietyBar.fillAmount = satiety;
    }

    IEnumerator StarvingProcess() {
        yield return new WaitForSeconds(secondsPerPercentSatiety);
        while (true) {
            satiety -= 0.01f;
            yield return new WaitForSeconds(secondsPerPercentSatiety);
        }
    }
    void UpdateSatietyBar() {
        if (satiety > 0f) {
            //satiety -= Time.deltaTime * 0.1f;
            satietyBar.fillAmount = satiety;
            StopCoroutine(_DamageFromStarvingProcess);
        }
        if (satiety > 1.0f) { satiety = 1.0f; }
    }

    private void Update() {
        UpdateSatietyBar();
        UpdateHealthBar();
    }

    bool isDead = false;
    void UpdateHealthBar() {
        if (health > 1.0f) { health = 1.0f; }

        if (satiety <= 0f) {
            if (health <= 0f) {
                if (!isDead) {
                    StopCoroutine(_StarvingProcess); StopCoroutine(_DamageFromStarvingProcess); CrDmgRunning = false;
                    StartCoroutine(ShowScreenOfPlayerDeath());
                    isDead = true;
                }
            }
            else {
                if (!CrDmgRunning)
                    StartCoroutine(_DamageFromStarvingProcess);
            }
        }
        else {
            if (satiety > 0f) {
                StopCoroutine(_DamageFromStarvingProcess); CrDmgRunning = false;
            }
                
        }
        healthBar.fillAmount = health;
    }

    bool CrDmgRunning = false;
    IEnumerator DamageFromStarvingProcess() {
        //print("start");
        CrDmgRunning = true;
        yield return new WaitForSeconds(secondsPerHealthPoint);
        while (true) {
            health -= 0.01f;
            yield return new WaitForSeconds(secondsPerHealthPoint);
        }
    }

    IEnumerator ShowScreenOfPlayerDeath() {
        isGameOver = true;

        Debug.Log("Player died from starving");
        yield return null;

        Time.timeScale = 0;
        AudioListener.pause = true;

        deathScreen.SetActive(true);
        overlay.SetActive(false);
    }
}