using UnityEngine;
using System.Collections;
using System.Reflection;

[DefaultExecutionOrder(-50)]
public class StartupJumpGate : MonoBehaviour
{
    [Header("References")]
    [Tooltip("PlayerJump component to gate at startup. If null, will search scene.")]
    [SerializeField] private PlayerJump playerJump;

    [Tooltip("Optional reference to StairSpawner. If null, will search scene.")]
    [SerializeField] private MonoBehaviour stairSpawner; // keep loose to avoid direct dependency in case of rename

    [Header("Timing")]
    [Tooltip("Extra safety buffer added to the computed wait time (seconds).")]
    [SerializeField] private float extraBufferSeconds = 0.05f;

    private void Awake()
    {
        if (playerJump == null)
        {
            playerJump = FindFirstObjectByType<PlayerJump>();
        }
        if (playerJump != null)
        {
            playerJump.enabled = false;
        }

        if (stairSpawner == null)
        {
            stairSpawner = (MonoBehaviour)FindFirstObjectByType<StairSpawner>();
        }
    }

    private void Start()
    {
        float totalWait = CalcStairsWaitSeconds();
        StartCoroutine(ReenableJumpAfter(totalWait + Mathf.Max(0f, extraBufferSeconds)));
    }

    private IEnumerator ReenableJumpAfter(float seconds)
    {
        if (seconds > 0f)
        {
            yield return new WaitForSeconds(seconds);
        }
        if (playerJump != null)
        {
            playerJump.enabled = true;
        }
    }

    private float CalcStairsWaitSeconds()
    {
        if (stairSpawner == null)
        {
            return 0f;
        }

        // Use reflection to read private serialized fields without modifying StairSpawner
        try
        {
            System.Type spType = stairSpawner.GetType();
            FieldInfo fUp = spType.GetField("initialStairsUp", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo fDown = spType.GetField("initialStairsDown", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo fDelay = spType.GetField("delayBetweenSteps", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo fDur = spType.GetField("appearDuration", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fUp == null || fDown == null || fDelay == null || fDur == null)
            {
                return 0f;
            }

            int up = (int)fUp.GetValue(stairSpawner);
            int down = (int)fDown.GetValue(stairSpawner);
            float delay = (float)fDelay.GetValue(stairSpawner);
            float dur = (float)fDur.GetValue(stairSpawner);

            int total = Mathf.Max(0, up + down);
            float sequenceTime = total > 0 ? Mathf.Max(0f, (total - 1) * delay) : 0f;
            return sequenceTime + Mathf.Max(0f, dur);
        }
        catch
        {
            return 0f;
        }
    }

}


