using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameCycleManager : MonoBehaviour
{
    // === Настройки ===
    [Header("Game Phase Timers")]
    public float dayDurationInMinutes = 5f;
    public float discussionDurationInSeconds = 40f;
    public float votingDurationInSeconds = 10f;
    public float phaseTitleDisplayDuration = 3f;

    [Header("UI References")]
    public TextMeshProUGUI phaseTitleText; // Текст в центре (DAY 1, Discussion)
    public TextMeshProUGUI timerText;       // Таймер слева сверху
    public CanvasGroup fadePanel;
    public float fadeSpeed = 1f;

    [Header("Player & Spawn")]
    public Transform playerTransform;
    public Transform spawnPoint;

    [Header("Skybox Settings")]
    public Material[] skyboxes;

    // === Внутренние переменные ===
    private float currentTimer;
    private int dayCount = 0;
    private bool isFading = false;

    private enum GameState
    {
        Day,
        Discussion,
        Voting,
        NightTransition
    }
    private GameState currentState;

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (!isFading)
        {
            if (currentState == GameState.Day)
            {
                currentTimer -= Time.deltaTime;
                timerText.text = "Time: " + Mathf.RoundToInt(currentTimer).ToString();
            }
            else if (currentState == GameState.Discussion)
            {
                currentTimer -= Time.deltaTime;
                timerText.text = "Discussion Time: " + Mathf.RoundToInt(currentTimer).ToString();
            }
            else if (currentState == GameState.Voting)
            {
                currentTimer -= Time.deltaTime;
                timerText.text = "Decision Time: " + Mathf.RoundToInt(currentTimer).ToString();
            }
        }
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            // === ФАЗА ДНЯ ===
            dayCount++;
            currentState = GameState.Day;
            fadePanel.alpha = 0;
            timerText.enabled = true;
            
            // Текст "DAY X" появляется и затем исчезает
            phaseTitleText.text = "DAY " + dayCount;
            yield return StartCoroutine(FadeInText(phaseTitleText));
            yield return new WaitForSeconds(phaseTitleDisplayDuration);
            yield return StartCoroutine(FadeOutText(phaseTitleText));

            if (skyboxes.Length > 0 && skyboxes[0] != null)
                RenderSettings.skybox = skyboxes[0];

            currentTimer = dayDurationInMinutes * 60f;
            yield return new WaitForSeconds(currentTimer);
            yield return StartCoroutine(FadeOutText(timerText));

            // === ПЕРЕХОД К НОЧИ ===
            yield return StartCoroutine(FadeScreen(1, "NIGHT " + dayCount));
            isFading = false;
            
            if (skyboxes.Length > 1 && skyboxes[1] != null)
                RenderSettings.skybox = skyboxes[1];
            playerTransform.position = spawnPoint.position;
            
            // === ФАЗА ОБСУЖДЕНИЯ ===
            currentState = GameState.Discussion;
            yield return StartCoroutine(FadeScreen(0, "Discussion"));
            yield return StartCoroutine(FadeInText(phaseTitleText));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(FadeOutText(phaseTitleText));

            timerText.enabled = true;
            currentTimer = discussionDurationInSeconds;
            yield return new WaitForSeconds(currentTimer);
            yield return StartCoroutine(FadeOutText(timerText));

            // === ФАЗА ГОЛОСОВАНИЯ ===
            currentState = GameState.Voting;
            yield return StartCoroutine(FadeScreen(0, "Voting"));
            yield return StartCoroutine(FadeInText(phaseTitleText));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(FadeOutText(phaseTitleText));

            timerText.enabled = true;
            currentTimer = votingDurationInSeconds;
            yield return new WaitForSeconds(currentTimer);
            yield return StartCoroutine(FadeOutText(timerText));

            // === ВОЗВРАЩЕНИЕ КО ДНЮ ===
            yield return StartCoroutine(FadeScreen(1, ""));
            phaseTitleText.text = "Petya has been voted out.";
            yield return StartCoroutine(FadeInText(phaseTitleText));
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(FadeOutText(phaseTitleText));
        }
    }

    private IEnumerator FadeScreen(float targetAlpha, string textToShow)
    {
        isFading = true;
        phaseTitleText.text = textToShow;
        
        while (!Mathf.Approximately(fadePanel.alpha, targetAlpha))
        {
            fadePanel.alpha = Mathf.MoveTowards(fadePanel.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        
        isFading = false;
    }

    private IEnumerator FadeOutText(TextMeshProUGUI textToFade)
    {
        Color startColor = textToFade.color;
        float fadeDuration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            textToFade.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        textToFade.color = startColor;
        textToFade.enabled = false;
    }
    
    private IEnumerator FadeInText(TextMeshProUGUI textToFade)
    {
        Color startColor = textToFade.color;
        float fadeDuration = 1.5f;
        float elapsedTime = 0f;
        textToFade.enabled = true;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            textToFade.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
    }
}