using Facebook.Unity;
using UnityEngine;

namespace MMPandSneSignal.Scripts.Services
{
    public class FacebookService : MonoBehaviour
    {
    
        public void Initialize()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                FB.Init(() =>
                {
                    FB.ActivateApp();
                });
            }
        }
    }
}