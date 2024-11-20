using System.Collections;
using TMPro;
using UnityEngine;

public class ErrorLog : MonoBehaviour
{
    [SerializeField, Min(0)] private float _messageVisibilityDuration = 1;
    [SerializeField] private TextMeshProUGUI _errorText;

    private float _remainingShowDuration;
    private Coroutine _errorMessageCoroutine;

    public void ShowError(string errorMessage)
    {
        _errorText.text = errorMessage;
        _remainingShowDuration = _messageVisibilityDuration;
        _errorMessageCoroutine ??= StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        while (_remainingShowDuration > 0)
        {
            _remainingShowDuration = Mathf.Max(_remainingShowDuration - Time.deltaTime, 0);
            yield return null;
        }

        _errorText.text = string.Empty;
        _errorMessageCoroutine = null;
    }
}
