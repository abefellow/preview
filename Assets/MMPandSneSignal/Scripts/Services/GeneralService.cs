using System.Collections.Generic;
using AffiseAttributionLib;
using AffiseAttributionLib.Modules;
using MMPandSneSignal.Scripts.Controllers;
using OneSignalSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMPandSneSignal.Scripts.Services
{
    public class GeneralService : MonoBehaviour
    {
        [SerializeField] private LoadingController _loadingController;
        [SerializeField] private FacebookService _facebookService;

        public string OneSignalID;
        public string AffiseID;
        public string AffiseKey;
        private string _deeplink;

        private void Start()
        {
            Affise
                .Settings(
                    affiseAppId: AffiseID,
                    secretKey: AffiseKey
                )
                .Start();

            OneSignal.Initialize(OneSignalID);
            _loadingController.StartLoading();
            if (Affise.IsFirstRun())
            {
                Affise.Module.GetStatus(AffiseModules.Status, onAffiseCallback);
                _facebookService.Initialize();
            }
            else
            {
                LoadGame();
            }
        }

        private void onAffiseCallback(List<AffiseKeyValue> data)
        {
            LoadGame();
        }
        
        private void LoadGame()
        {
            SceneManager.LoadScene(0);
        }
    }
}