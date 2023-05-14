using System.Collections.Generic;
using UI;
using UnityEngine;
using Networking;

namespace Game
{
    public class GameManager: MonoBehaviour
    {
        [SerializeField] private TextAsset localStackOfBlocksData;
        [SerializeField] private BlocksManager blocksManager;
        [SerializeField] private UIHandler uiHandler;
        [SerializeField] private bool useRemoteStacksData = true;
        
        private NetworkHandler _networkHandler;

        private void Awake()
        {
            uiHandler.TestMyStackClicked += TestMyStacks;
            uiHandler.ResetGameButtonClicked += ResetLevel;
            blocksManager.BlockInfoDisplayNeeded += uiHandler.DisplayBlockInfo;
        }

        private void Start()
        {
            _networkHandler = new NetworkHandler
            {
                localStackOfBlocksData = localStackOfBlocksData.text
            };
            
            StartGame();
        }

        private void StartGame()
        {
            uiHandler.SetLoadingVisibility(true);
            
            if (useRemoteStacksData)
            {
                StartCoroutine(_networkHandler.GetStacksRemote(HandleFetchedStacks, HandleFetchStacksError));
            }
            else
            {
                var stacks = _networkHandler.GetStacksLocal();
                HandleFetchedStacks(stacks);
            }
        }

        private void HandleFetchedStacks(List<BlockData> blocks)
        {
            blocksManager.PlaceAllBlocks(blocks);
            uiHandler.SetLoadingVisibility(false);
        }

        private void HandleFetchStacksError(string error)
        {
            uiHandler.SetErrorVisible(true);
            uiHandler.SetLoadingVisibility(false);
        }

        private void TestMyStacks()
        {
            blocksManager.TestMyStacks();
        }

        private void ResetLevel()
        {
            blocksManager.ResetStacks();
            StartGame();
        }
    }
}