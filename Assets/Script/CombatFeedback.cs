using System.Collections;
using UnityEngine;

public static class CombatFeedback
{
    private static bool isHitStopping;

    public static void HitStop(MonoBehaviour owner, float duration)
    {
        if (isHitStopping || owner == null || !owner.isActiveAndEnabled) return;
        owner.StartCoroutine(HitStopCoroutine(duration));
    }

    private static IEnumerator HitStopCoroutine(float duration)
    {
        isHitStopping = true;

        float previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = previousTimeScale;
        isHitStopping = false;
    }
}
