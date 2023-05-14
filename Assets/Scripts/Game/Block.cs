using UnityEngine;

namespace Game
{
    public class Block: MonoBehaviour
    {
        public const string Tag = "Block";
        
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Rigidbody blockRigidBody;
        
        // public BlockType BlockType { get; private set; }
        public BlockData BlockData { get; private set; }

        public Rigidbody Rigidbody => blockRigidBody;

        // public void SetBlockType(BlockType blockObjectType, Material material)
        // {
        //     BlockType = blockObjectType;
        //     meshRenderer.material = material;
        // }
        
        public void SetBlockData(BlockData blockData, Material material)
        {
            BlockData = blockData;
            meshRenderer.material = material;
        }
    }
}