using UnityEngine;

public enum TimeEra { Past, Present, Future }

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public TimeEra currentEra = TimeEra.Present;

    [Header("Visual & Audio Feedback")]
    [SerializeField] private AudioSource timeShiftSound;
    [SerializeField] private Animator screenEffectAnim;

    public delegate void OnTimeShift(TimeEra newEra);
    public static event OnTimeShift onTimeShift;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ShiftTime(TimeEra targetEra)
    {
        if (targetEra == currentEra) return;

        currentEra = targetEra;

        screenEffectAnim.SetTrigger("TimeShift");
        timeShiftSound.Play();

        onTimeShift?.Invoke(targetEra);
        Debug.Log("Time shifted to: " + targetEra);
    }
}