using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("=== TIMER ===")]
    [SerializeField] private float startTime = 120f;
    [SerializeField] private TMP_Text timerText;

    [Header("=== LAST SECONDS ===")]
    [SerializeField] private float redColorStartTime = 5f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lastSecondsColor = Color.red;

    [Header("=== TICK SOUND ===")]
    [SerializeField] private AudioSource tickAudioSource;
    [SerializeField] private AudioClip tickSound;
    [SerializeField] private float tickStartTime = 5f;

    [Header("=== ALARM ===")]
    [SerializeField] private AudioSource alarmAudioSource;
    [SerializeField] private AudioClip alarmSound;

    [Header("=== BLINK ===")]
    [SerializeField] private float blinkDuration = 4f;
    [SerializeField] private float blinkSpeed = 0.2f;

    [Header("=== FADE ===")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 2f;

    [Header("=== SCENE ===")]
    [SerializeField] private string nextSceneName;

    private float currentTime;
    private int lastSecond = -1;

    private bool timerFinished = false;
    private bool finishStarted = false;


    private void Start()
    {
        RestartTimer();
    }


    private void Update()
    {
        if (timerFinished)
            return;

        currentTime -= Time.unscaledDeltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;

            UpdateTimerText();

            timerFinished = true;

            if (!finishStarted)
            {
                finishStarted = true;
                StartCoroutine(FinishTimer());
            }

            return;
        }

        UpdateTimerText();

        // ==========================================
        // ПОСЛЕДНИЕ 5 СЕКУНД
        // ==========================================

        if (currentTime <= redColorStartTime)
        {
            // Делаем текст красным
            if (timerText != null)
            {
                timerText.color = lastSecondsColor;
            }
        }

        // ==========================================
        // ТИКАНЬЕ
        // ==========================================

        if (currentTime <= tickStartTime)
        {
            int secondsLeft = Mathf.CeilToInt(currentTime);

            if (secondsLeft != lastSecond)
            {
                lastSecond = secondsLeft;
                PlayTick();
            }
        }
    }


    // ==========================================
    // ОБНОВЛЕНИЕ ТЕКСТА
    // ==========================================

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    // ==========================================
    // ТИК
    // ==========================================

    private void PlayTick()
    {
        if (tickAudioSource == null)
            return;

        if (tickSound == null)
            return;

        tickAudioSource.PlayOneShot(tickSound);
    }


    // ==========================================
    // ЗАВЕРШЕНИЕ ТАЙМЕРА
    // ==========================================

    private IEnumerator FinishTimer()
    {
        // Будильник
        if (alarmAudioSource != null && alarmSound != null)
        {
            alarmAudioSource.PlayOneShot(alarmSound);
        }

        // Мигание
        yield return StartCoroutine(BlinkTimer());

        // Затемнение
        yield return StartCoroutine(FadeToBlack());

        // Смена сцены
        LoadNextScene();
    }


    // ==========================================
    // МИГАНИЕ
    // ==========================================

    private IEnumerator BlinkTimer()
    {
        if (timerText == null)
            yield break;

        float elapsed = 0f;

        timerText.enabled = true;

        while (elapsed < blinkDuration)
        {
            timerText.enabled = false;

            yield return new WaitForSecondsRealtime(blinkSpeed);

            elapsed += blinkSpeed;

            timerText.enabled = true;

            yield return new WaitForSecondsRealtime(blinkSpeed);

            elapsed += blinkSpeed;
        }

        timerText.enabled = true;
    }


    // ==========================================
    // ЗАТЕМНЕНИЕ
    // ==========================================

    private IEnumerator FadeToBlack()
    {
        if (fadePanel == null)
            yield break;

        float elapsed = 0f;

        fadePanel.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = elapsed / fadeDuration;

            fadePanel.alpha = Mathf.Clamp01(progress);

            yield return null;
        }

        fadePanel.alpha = 1f;
    }


    // ==========================================
    // СМЕНА СЦЕНЫ
    // ==========================================

    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning(
                "CountdownTimer: Next Scene Name не указан!"
            );

            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }


    // ==========================================
    // ПЕРЕЗАПУСК
    // ==========================================

    public void RestartTimer()
    {
        StopAllCoroutines();

        currentTime = Mathf.Max(0f, startTime);

        lastSecond = -1;

        timerFinished = false;

        finishStarted = false;

        if (timerText != null)
        {
            timerText.enabled = true;

            // Возвращаем обычный цвет
            timerText.color = normalColor;
        }

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
        }

        UpdateTimerText();
    }
}