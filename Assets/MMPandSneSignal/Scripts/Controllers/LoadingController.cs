using System.Collections;
using TMPro;
using UnityEngine;

namespace MMPandSneSignal.Scripts.Controllers
{
    public class LoadingController : MonoBehaviour
    {
        //TODO Remove this shit and use some better solutions for text transition
        private readonly string[] _loadingTextStates = {
            "Loading.",
            "Loading..",
            "Loading..."
        };

        [SerializeField] private TextMeshProUGUI _loadingText;
        [SerializeField] private float _loadingTextAnimationStepTime = 1;

        public void StartLoading()
        {
            gameObject.SetActive(true);
            StartCoroutine(LoadingTextAnimation());
        }

        public void StopLoading()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }

        private IEnumerator LoadingTextAnimation()
        {
            while (true)
            {
                for (int i = 0; i < _loadingTextStates.Length; i++)
                {
                    _loadingText.text = _loadingTextStates[i];
                    yield return new WaitForSeconds(_loadingTextAnimationStepTime);
                }
            }
        }
    }
}