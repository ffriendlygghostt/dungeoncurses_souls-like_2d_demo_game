using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class RestEffectController : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera defaultCam;
    public CinemachineVirtualCamera restCam;

    [Header("Скрипты для закрытия панелей")]
    public TeleportManager teleportManager;
    public RestPhrasePanel restPhrasePanel;

    [Header("Fade Image")]
    public Image fadeOverlay;

    [Header("Cursor Object")]
    public GameObject cursorObject;

    [Header("Durations")]
    public float zoomDuration = 1.2f;
    public float restDuration = 2f;

    [Header("Player Reference")]
    public GameObject playerObject;

    public void TriggerRestEffect()
    {
        if (teleportManager != null && teleportManager.panelTeleport.activeSelf)
            teleportManager.ToggleTeleportPanel();

        if (restPhrasePanel != null)
            restPhrasePanel.HidePanel();

        if (cursorObject != null)
            cursorObject.SetActive(false);

        // 👉 СОХРАНЯЕМ ПОЗИЦИЮ ОТДЫХА
        if (playerObject != null)
            PlayerRespawnManager.SetRestPosition(playerObject.transform.position);

        StartCoroutine(RestSequence());
    }


    private IEnumerator RestSequence()
    {
        if (fadeOverlay != null)
            fadeOverlay.gameObject.SetActive(true);

        yield return StartCoroutine(BlendCameras(defaultCam, restCam, zoomDuration, fadeIn: true));

        // === ВОССТАНОВЛЕНИЕ ===
        if (playerObject != null)
        {
            var healingBook = playerObject.GetComponent<HealingBook>();
            var playerHealth = playerObject.GetComponent<PlayerHealth>();
            var bleedable = playerObject.GetComponent<Bleedable>();
            var burnable = playerObject.GetComponent<Burnable>();
            var freezable = playerObject.GetComponent<Freezable>();
            var rotable = playerObject.GetComponent<Rotable>();
            var damageVisuals = playerObject.GetComponent<DamageVisuals>();

            healingBook?.RestoreAllUses();

            if (playerHealth != null && !playerHealth.IsDead &&
                playerHealth.CurrentHealth < playerObject.GetComponent<PlayerStats>().maxHealth)
            {
                playerHealth.Heal50Percent();
            }

            bleedable?.StopBleeding();
            burnable?.Extinguish();
            freezable?.RemoveFreezeP();
            freezable?.RemoveChillP();
            rotable?.StopRot();
            damageVisuals?.ClearDamageEffects();
        }


        yield return new WaitForSeconds(restDuration);

        yield return StartCoroutine(BlendCameras(restCam, defaultCam, zoomDuration, fadeIn: false));

        if (fadeOverlay != null)
            fadeOverlay.gameObject.SetActive(false);

        if (cursorObject != null)
            cursorObject.SetActive(true);
    }

    private IEnumerator BlendCameras(CinemachineVirtualCamera fromCam, CinemachineVirtualCamera toCam, float duration, bool fadeIn)
    {
        float time = 0f;

        fromCam.Priority = 5;
        toCam.Priority = 10;

        Color color = fadeOverlay.color;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (time < duration)
        {
            float t = time / duration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeOverlay.color = new Color(color.r, color.g, color.b, alpha);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        fadeOverlay.color = new Color(color.r, color.g, color.b, endAlpha);
        fromCam.Priority = 0;
        toCam.Priority = 10;
    }
}
