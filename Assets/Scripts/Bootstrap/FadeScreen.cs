using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeScreen : UIScreen
{
    [SerializeField] private float _fadeDuration = 1.5f;

    protected override void Awake()
    {
        base.Awake();
    }

    public void HideImmediate()
    {
        _canvasGroup.alpha = 1.0f;
        _canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(true);
    }

    public async UniTask FadeInAsync(CancellationToken cancellationToken = default)
    {
        gameObject.SetActive(true);
        _canvasGroup.blocksRaycasts = true;

        float elapsed = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < _fadeDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / _fadeDuration);
            
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        _canvasGroup.alpha = 1f;
    }

    public async UniTask FadeOutAsync(CancellationToken cancellationToken = default)
    {
        float elapsed = 0f;
        float startAlpha = _canvasGroup.alpha;

        while (elapsed < _fadeDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeDuration);
            
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}
