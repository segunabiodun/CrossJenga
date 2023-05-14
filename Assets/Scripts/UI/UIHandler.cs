using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIHandler : MonoBehaviour
    {
        public Action<Transform> CameraFocusTargetChanged;
        public Action TestMyStackClicked;
        public Action ResetGameButtonClicked;

        [SerializeField] private Transform[] focusTargets;
        [SerializeField] private Button[] focusTargetButtons;
        [SerializeField] private Button testMyStackButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private RectTransform errorView;
        [SerializeField] private RectTransform loadingIndicator;
        
        [Space]
        [SerializeField] private RectTransform blockInfoView;
        [SerializeField] private TextMeshProUGUI blockInfoText;
        [SerializeField] private Button closeBlockInfoButton;

        private void Awake()
        {
            for (int i = 0; i < focusTargetButtons.Length; i++)
            {
                var index = i;
                focusTargetButtons[i].onClick.AddListener(() => { FocusTargetButtonClicked(index); });
            }

            testMyStackButton.onClick.AddListener(() => { TestMyStackClicked?.Invoke(); });
            resetButton.onClick.AddListener(HandleResetButtonClicked);
            closeBlockInfoButton.onClick.AddListener(() => { blockInfoView.gameObject.SetActive(false); });
            
            SetErrorVisible(false);
            blockInfoView.gameObject.SetActive(false);
        }

        private void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)errorView.parent);
        }

        public void DisplayBlockInfo(Block block)
        {
            var blockData = block.BlockData;
            blockInfoView.gameObject.SetActive(true);
            blockInfoText.text = $"{blockData.grade}: {blockData.domain}\n" +
                                 $"{blockData.cluster}\n" +
                                 $"{blockData.standardid}: {blockData.standarddescription}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(blockInfoView);
        }

        public void SetErrorVisible(bool isVisible)
        {
            errorView.gameObject.SetActive(isVisible);
        }

        public void SetLoadingVisibility(bool isLoading)
        {
            loadingIndicator.gameObject.SetActive(isLoading);
            resetButton.enabled = !isLoading;
            testMyStackButton.enabled = !isLoading;
        }

        private void FocusTargetButtonClicked(int index)
        {
            CameraFocusTargetChanged?.Invoke(focusTargets[index]);
        }

        private void HandleResetButtonClicked()
        {
            SetErrorVisible(false);
            ResetGameButtonClicked?.Invoke();
        }
    }
}