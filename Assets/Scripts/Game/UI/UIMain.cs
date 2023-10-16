using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Game.UI
{
    public class UIMain : MonoBehaviour
    {
        public static UIMain Instance;
    
        public UnityEngine.Camera mainCam = null;
        public UnityEngine.Camera uiCam = null;
        public Canvas canvas = null;
    
        public GameObject readyModeRoot;

        public List<GameObject> stepRoots = new List<GameObject>();

        public Image gameProgressImage;
        public TextMeshProUGUI gameProgressLabel;
    
        public Image gameResultProgressImage;
        public TextMeshProUGUI gameResultProgressLabel;
        public TextMeshProUGUI gameResultLabel;

        public GameObject deadInfoRoot = null;
        public UIDeadInfoItem deadInfoItemPrefab = null;

        private void Awake()
        {
            Instance = this;
            UICommon.UiCam = uiCam;
            UICommon.MainCam = mainCam;
            UICommon.Canvas = canvas;
        }

        private void Start()
        {
            GameManager.onChangeGameStep += OnChangeGameStep;
            OnChangeGameStep(GameManager.GameStep.Ready);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageDown))
                OnClickRestart();
        }

        private void OnEnable()
        {
        }
    
        private void OnDisable()
        {
        }
    
        private void OnChangeGameStep(GameManager.GameStep step)
        {
            for(int i = 0; i < stepRoots.Count; i++)
                stepRoots[i].SetActive((int)step == i);
        
            if( step == GameManager.GameStep.PlayGame)
            {
            }
        
            if (step == GameManager.GameStep.Ready)
            {
                deadInfoRoot.DestroyAllChilds();
                OnUpdateGameProgress();
            }
        
            if (step == GameManager.GameStep.Fail)
            {
            }
        
            if (step == GameManager.GameStep.Success)
            {
                SetGameResult();
            }
        }
    
        private void OnUpdateGameProgress()
        {
        }
    
        private void SetGameResult()
        {
        }
    
        public void OnClickRestart()
        {
            GameManager.Instance.SetGameStep(GameManager.GameStep.Ready);
        }
    
        public void OnClickNextLevel()
        {
            //GameManager.instance.NextLevel();
        }

        public void OnDeadMessage(string uiName)
        {
            var item = GameObject.Instantiate(deadInfoItemPrefab, deadInfoRoot.transform);
            item.Setup(uiName);
        }
    }
}
