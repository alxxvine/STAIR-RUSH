using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Scene Transition")]
    public string sceneToLoad;
    public Button startButton;

    [Header("Animation Settings")]
    public UIAnimator uiAnimator;
    
    [Header("Button Animation Settings")]
    public float buttonJumpHeight = 50f;
    public float buttonJumpDuration = 0.25f;
    public float buttonFallDelay = 0.1f;
    public float buttonFallDistance = 400f;
    public float buttonFallDuration = 0.75f;
    public float buttonFadeDelay = 0.2f;
    public float buttonFadeDuration = 0.5f;


    [Header("Audio Settings")]
    public AudioClip clickSound;
    private AudioSource audioSource;
    
    void Start()
    {
        // Ensure there's an AudioSource component to play the sound
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Add a listener to the button's click event
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }
        else
        {
            Debug.LogError("Start Button is not assigned in the MenuManager.");
        }

        if (uiAnimator == null)
        {
            Debug.LogError("UIAnimator is not assigned in the MenuManager.");
        }
    }

    private void OnStartButtonClick()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Disable button to prevent multiple clicks
        startButton.interactable = false;
        
        // Start the animations for other UI elements
        if (uiAnimator != null)
        {
            uiAnimator.StartAnimations();
        }

        // Start the master coroutine for button animation and scene transition
        StartCoroutine(StartTransition());
    }

    private IEnumerator StartTransition()
    {
        // Calculate total time for button animation
        float postJumpAnimationTime = Mathf.Max(buttonFallDuration, buttonFadeDelay + buttonFadeDuration);
        float buttonAnimationTime = buttonJumpDuration + buttonFallDelay + postJumpAnimationTime;

        // Calculate longest animation time from UIAnimator
        float otherAnimationsTime = 0f;
        if (uiAnimator != null)
        {
            foreach (var obj in uiAnimator.animatedObjects)
            {
                float objTime = obj.delay + obj.fadeDuration;
                if (objTime > otherAnimationsTime)
                {
                    otherAnimationsTime = objTime;
                }
            }
        }

        // Start button animation in parallel
        StartCoroutine(AnimateButton());

        // Wait for the longest animation to finish
        yield return new WaitForSeconds(Mathf.Max(buttonAnimationTime, otherAnimationsTime));

        // Load the game scene
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private IEnumerator AnimateButton()
    {
        RectTransform buttonRect = startButton.GetComponent<RectTransform>();
        CanvasGroup buttonCanvasGroup = startButton.GetComponent<CanvasGroup>();
        if (buttonCanvasGroup == null)
        {
            buttonCanvasGroup = startButton.gameObject.AddComponent<CanvasGroup>();
        }
        
        Vector3 originalPos = buttonRect.anchoredPosition;
        
        // 1. Animate jump up
        float elapsedTime = 0f;
        Vector3 jumpTargetPos = originalPos + new Vector3(0, buttonJumpHeight, 0);
        while (elapsedTime < buttonJumpDuration)
        {
            elapsedTime += Time.deltaTime;
            buttonRect.anchoredPosition = Vector3.Lerp(originalPos, jumpTargetPos, elapsedTime / buttonJumpDuration);
            yield return null;
        }
        buttonRect.anchoredPosition = jumpTargetPos;

        // 2. Delay before fall
        yield return new WaitForSeconds(buttonFallDelay);

        // 3. Animate fall & fade with separate timings
        float animationTimer = 0f;
        float totalDuration = Mathf.Max(buttonFallDuration, buttonFadeDelay + buttonFadeDuration);
        
        Vector3 fallStartPos = buttonRect.anchoredPosition;
        Vector3 fallEndPos = fallStartPos - new Vector3(0, buttonFallDistance, 0);

        while (animationTimer < totalDuration)
        {
            animationTimer += Time.deltaTime;

            // Handle fall based on its own duration
            if (animationTimer <= buttonFallDuration)
            {
                float fallProgress = animationTimer / buttonFallDuration;
                buttonRect.anchoredPosition = Vector3.Lerp(fallStartPos, fallEndPos, fallProgress);
            }

            // Handle fade based on its own delay and duration
            if (animationTimer >= buttonFadeDelay)
            {
                float fadeTimer = animationTimer - buttonFadeDelay;
                if (fadeTimer <= buttonFadeDuration)
                {
                    float fadeProgress = fadeTimer / buttonFadeDuration;
                    buttonCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
                }
            }
            
            yield return null;
        }
        
        // Ensure final state is set correctly
        buttonRect.anchoredPosition = fallEndPos;
        buttonCanvasGroup.alpha = 0f;
    }
}
