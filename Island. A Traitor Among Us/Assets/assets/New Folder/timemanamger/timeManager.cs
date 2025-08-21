using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameCycleManager : MonoBehaviour
{
    // === Настройки ===
    [Header("Game Phase Timers")]
    [Tooltip("Длительность дневной фазы в минутах.")]
    [Range(1f, 60f)]
    public float dayDurationInMinutes = 5f;
    [Tooltip("Длительность фазы обсуждения в секундах.")]
    [Range(1f, 120f)]
    public float discussionDurationInSeconds = 40f;
    [Tooltip("Длительность фазы голосования в секундах.")]
    [Range(1f, 60f)]
    public float votingDurationInSeconds = 10f;
    [Tooltip("Время отображения текста 'DAY 1', 'Discussion' и т.д.")]
    [Range(1f, 10f)]
    public float phaseTitleDisplayDuration = 3f;

    [Header("UI References")]
    [Tooltip("Текст, который появляется в центре экрана (DAY 1, Discussion).")]
    public TextMeshProUGUI phaseTitleText;
    [Tooltip("Текст для отображения таймера.")]
    public TextMeshProUGUI timerText;
    [Tooltip("Панель (CanvasGroup) для затемнения/просветления экрана.")]
    public CanvasGroup fadePanel;
    [Tooltip("Скорость затемнения/просветления экрана.")]
    [Range(0.1f, 10f)]
    public float fadeSpeed = 1f;

    [Header("Player & Spawn")]
    [Tooltip("Трансформ игрока.")]
    public Transform playerTransform;
    [Tooltip("Точка возрождения игрока.")]
    public Transform spawnPoint;

    [Header("Skybox & Temperature Settings")]
    [Tooltip("Материал скайбокса для дня.")]
    public Material daySkybox;
    [Tooltip("Материал скайбокса для ночи.")]
    public Material nightSkybox;
    [Tooltip("Температура окружающей среды днём.")]
    public float dayTemperature = 25f;
    [Tooltip("Температура окружающей среды ночью.")]
    public float nightTemperature = -20f;
    
    // === Внутренние переменные ===
    private float currentTimer;
    private int dayCount = 0;
    private bool isFading = false;
    private TemperatureSystem temperatureSystem;

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
        if (fadePanel == null || phaseTitleText == null || timerText == null)
        {
            Debug.LogError("Один или несколько UI-элементов не подключены в инспекторе GameCycleManager!");
            return;
        }

        temperatureSystem = FindObjectOfType<TemperatureSystem>();
        if (temperatureSystem == null)
        {
            Debug.LogError("TemperatureSystem не найден в сцене!");
        }

        fadePanel.alpha = 1f;
        timerText.enabled = false;
        
        if (daySkybox != null)
        {
            RenderSettings.skybox = daySkybox;
        }
        else
        {
            Debug.LogWarning("Материал скайбокса дня не найден!");
        }

        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (isFading || timerText == null) return;

        if (currentState == GameState.Day)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0)
            {
                currentTimer = 0;
            }
            timerText.text = "Time: " + Mathf.RoundToInt(currentTimer).ToString();
        }
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            dayCount++;
            currentState = GameState.Day;
            if (temperatureSystem != null) temperatureSystem.environmentTemperature = dayTemperature;
            if (daySkybox != null) RenderSettings.skybox = daySkybox;

            // ... (Ваша существующая логика для дня)
            // Показываем текст "DAY 1"
            if (phaseTitleText != null)
            {
                phaseTitleText.text = "DAY " + dayCount;
                yield return StartCoroutine(FadeInText(phaseTitleText));
            }
            yield return new WaitForSeconds(phaseTitleDisplayDuration);
            if (phaseTitleText != null)
            {
                yield return StartCoroutine(FadeOutText(phaseTitleText));
            }

            // Просветляем экран
            if (fadePanel != null)
            {
                yield return StartCoroutine(FadeScreen(0, ""));
            }

            // Включаем таймер
            if (timerText != null)
            {
                timerText.enabled = true;
                currentTimer = dayDurationInMinutes * 60f;
                while (currentTimer > 0)
                {
                    yield return null;
                }
                yield return StartCoroutine(FadeOutText(timerText));
            }

            // Переход к ночи (затемняем экран)
            currentState = GameState.NightTransition;
            if (temperatureSystem != null) temperatureSystem.environmentTemperature = nightTemperature;
            if (nightSkybox != null) RenderSettings.skybox = nightSkybox;
            
            if (fadePanel != null)
            {
                yield return StartCoroutine(FadeScreen(1, ""));
            }
            isFading = false;
            
            // ... (Ваша существующая логика для ночи)
            // Телепортация игрока
            if (playerTransform != null && spawnPoint != null)
            {
                playerTransform.position = spawnPoint.position;
            }
            else
            {
                Debug.LogError("Player Transform или Spawn Point не подключены!");
            }

            // Фаза обсуждения
            currentState = GameState.Discussion;
            if (fadePanel != null)
            {
                yield return StartCoroutine(FadeScreen(0, "Discussion"));
            }
            if (phaseTitleText != null)
            {
                yield return StartCoroutine(FadeInText(phaseTitleText));
                yield return new WaitForSeconds(1.5f);
                yield return StartCoroutine(FadeOutText(phaseTitleText));
            }

            // Таймер обсуждения
            if (timerText != null)
            {
                timerText.enabled = true;
                currentTimer = discussionDurationInSeconds;
                while (currentTimer > 0)
                {
                    currentTimer -= Time.deltaTime;
                    yield return null;
                }
                yield return StartCoroutine(FadeOutText(timerText));
            }

            // Фаза голосования
            currentState = GameState.Voting;
            if (fadePanel != null)
            {
                yield return StartCoroutine(FadeScreen(0, "Voting"));
            }
            if (phaseTitleText != null)
            {
                yield return StartCoroutine(FadeInText(phaseTitleText));
                yield return new WaitForSeconds(1.5f);
                yield return StartCoroutine(FadeOutText(phaseTitleText));
            }

            // Таймер голосования
            if (timerText != null)
            {
                timerText.enabled = true;
                currentTimer = votingDurationInSeconds;
                while (currentTimer > 0)
                {
                    currentTimer -= Time.deltaTime;
                    yield return null;
                }
                yield return StartCoroutine(FadeOutText(timerText));
            }

            // Возвращение ко дню
            if (fadePanel != null)
            {
                yield return StartCoroutine(FadeScreen(1, ""));
            }
            if (phaseTitleText != null)
            {
                phaseTitleText.text = "Petya has been voted out.";
                yield return StartCoroutine(FadeInText(phaseTitleText));
                yield return new WaitForSeconds(3f);
                yield return StartCoroutine(FadeOutText(phaseTitleText));
            }
        }
    }
    
    // ... (Ваши существующие методы FadeScreen, FadeOutText, FadeInText)

    private IEnumerator FadeScreen(float targetAlpha, string textToShow)
    {
        if (fadePanel == null) yield break;
        isFading = true;
        
        if (phaseTitleText != null)
        {
            phaseTitleText.text = textToShow;
        }
        
        while (!Mathf.Approximately(fadePanel.alpha, targetAlpha))
        {
            fadePanel.alpha = Mathf.MoveTowards(fadePanel.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        
        isFading = false;
    }

    private IEnumerator FadeOutText(TextMeshProUGUI textToFade)
    {
        if (textToFade == null) yield break;
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
        if (textToFade == null) yield break;
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