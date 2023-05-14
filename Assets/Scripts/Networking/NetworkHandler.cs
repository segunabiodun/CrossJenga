using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class NetworkHandler
    {
        public string localStackOfBlocksData = "";
        
        private const string APIUrl = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";

        public List<BlockData> GetStacksLocal()
        {
            var jsonText = FormatForDecoding(localStackOfBlocksData);
            var blocks = JsonUtility.FromJson<StacksResponseDataWrapper>(jsonText).blocks;
            blocks.Sort();
            return blocks;
        }

        public IEnumerator GetStacksRemote(Action<List<BlockData>> successAction, Action<string> errorAction)
        {
            using var request = UnityWebRequest.Get(APIUrl);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                errorAction?.Invoke(request.error);
            }
            else
            {
                var jsonText = FormatForDecoding(request.downloadHandler.text);
                var blocks = JsonUtility.FromJson<StacksResponseDataWrapper>(jsonText).blocks;
                blocks.Sort();
                successAction.Invoke(blocks);
            }
        }

        private string FormatForDecoding(string rawResponseText)
        {
            if (rawResponseText.StartsWith("["))
            {
                return $"{{ \"{StacksResponseDataWrapper.BlocksFieldKey}\": {rawResponseText} }}";
            }

            return rawResponseText;
        }
    }
}
