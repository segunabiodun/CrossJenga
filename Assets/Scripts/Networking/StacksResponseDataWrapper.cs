using System;
using System.Collections.Generic;
using Game;

namespace Networking
{
    /// <summary>
    /// This is class is needed because with JsonUtility, we can't directly decode an array as the root level item.
    /// Only objects are acceptable.
    /// </summary>
    [Serializable]
    public class StacksResponseDataWrapper
    {
        public const string BlocksFieldKey = "blocks";
        public List<BlockData> blocks;
    }
}