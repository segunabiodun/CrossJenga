using System;

// ReSharper disable IdentifierTypo

namespace Game
{
    [Serializable]
    public class BlockData: IComparable<BlockData>
    {
        public BlockType mastery;
        public string cluster;
        public string domain;
        public string grade;
        public string id;
        public string standardid;
        public string standarddescription;

        public int CompareTo(BlockData other)
        {
            if (this.domain != other.domain)
            {
                return string.Compare(this.domain, other.domain, StringComparison.Ordinal);
            }

            if (this.cluster != other.cluster)
            {
                return string.Compare(this.cluster, other.cluster, StringComparison.Ordinal);
            }
            
            return string.Compare(this.standardid, other.standardid, StringComparison.Ordinal);
        }
    }
}