using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BlocksManager: MonoBehaviour
    {
        public Action<Block> BlockInfoDisplayNeeded;
        
        [SerializeField] private Material glass;
        [SerializeField] private Material stone;
        [SerializeField] private Material wood;
        [SerializeField] private Material errorMaterial;
        [SerializeField] private Block blockPrefab;
        [SerializeField] private Transform[] stackTransforms;
        [SerializeField] private float interBlockSpacing;
        [SerializeField] private Transform blocksPoolParent;

        private List<Block> _blocksPool;
        private List<List<Block>> _stacks;
        private Camera _mainCamera;

        private const float BlockWidth = 0.1f;
        private const int NumBlocksPerRow = 3;
        private float BlockHeight => BlockWidth;
        private float BlockDepth => BlockWidth * NumBlocksPerRow + interBlockSpacing * (NumBlocksPerRow - 1);

        
        Ray ray;
        RaycastHit hit;
        
        private void Start()
        {
            _blocksPool = new List<Block>();
            _stacks = new List<List<Block>> { new(), new(), new()};
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (!Input.GetMouseButtonDown(1))
            {
                return;
            }

            ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag(Block.Tag))
                {
                    var block = hit.collider.GetComponent<Block>();
                    BlockInfoDisplayNeeded?.Invoke(block);
                }
            }

        }

        public void TestMyStacks()
        {
            foreach (var stack in _stacks)
            {
                foreach (var block in stack)
                {
                    if (block.BlockData.mastery == BlockType.Glass)
                    {
                        block.gameObject.SetActive(false);
                    }
                    
                    block.Rigidbody.isKinematic = false;
                    
                }
            }
        }

        public void ResetStacks()
        {
            foreach (var stack in _stacks)
            {
                foreach (var block in stack)
                {
                    ReturnToPool(block);
                    
                }
            }
            
            _stacks = new List<List<Block>>() {new(), new(), new()};

        }

        public void PlaceAllBlocks(List<BlockData> blocks)
        {
            foreach (var blockData in blocks)
            {
                var block = GetBlock(blockData);
                PlaceBlock(block, blockData);
            }
        }

        private void PlaceBlock(Block block, BlockData blockData)
        {
            var stackIndex = GradeToStackIndex(blockData.grade);
            
            if (stackIndex < 0)
            {
                Debug.Log($"Skipping item with id \"{blockData.id}\" due to unknown grade \"{blockData.grade}\".");
                ReturnToPool(block);
                return;
            }

            var stackTransform = stackTransforms[stackIndex].transform;
            block.transform.parent = stackTransform;
            
            var stackPosition = stackTransform.position;
            var blocksInStack = _stacks[stackIndex];
            var blockIndex = blocksInStack.Count;
            var isLongitudinal = ShouldPlaceLongitudinal(blockIndex);
            var columnIndex = blockIndex % NumBlocksPerRow;
            var rowIndex = blockIndex / NumBlocksPerRow;
            
            var positionY = stackPosition.y + rowIndex * BlockHeight;
            
            // The offsets in the position calculations below are because the pivot is in the middle of the block.
            if (isLongitudinal)
            {
                var xOffsetForPivot = BlockWidth / 2;
                var positionXLongitudinal = stackPosition.x + columnIndex * (interBlockSpacing + BlockWidth) + xOffsetForPivot;
                block.transform.position = new Vector3(positionXLongitudinal, positionY, stackPosition.z);
            }
            else
            {
                var xOffsetForPivot = BlockDepth / 2;
                var positionXLateral = stackPosition.x + xOffsetForPivot;
                var zOffsetForPivot = -BlockDepth / 2 + BlockWidth / 2;
                var positionZLateral = stackPosition.z + columnIndex * (interBlockSpacing + BlockWidth) + zOffsetForPivot;
                block.transform.position = new Vector3(positionXLateral, positionY, positionZLateral);
            }
            
            block.transform.rotation = isLongitudinal ?
                Quaternion.identity : Quaternion.Euler(new Vector3(0, 90, 0));
            _stacks[stackIndex].Add(block);
        }

        private void ReturnToPool(Block block)
        {
            _blocksPool.Add(block);
            block.transform.parent = blocksPoolParent;
        }

        private bool ShouldPlaceLongitudinal(int blockIndexInStack)
        {
            var rowIndex = blockIndexInStack / 3;
            return IsEven(rowIndex);
        }

        private bool IsEven(int number)
        {
            return number % 2 == 0;
        }
        

        private Block GetBlock(BlockData blockData)
        {
            Block block;
            
            if (_blocksPool.Count == 0)
            {
                block = Instantiate(blockPrefab);
            }
            else
            {
                block = _blocksPool[^1];
                _blocksPool.RemoveAt(_blocksPool.Count-1);
            }

            block.transform.localScale = new Vector3(BlockWidth, BlockHeight, BlockDepth);
            block.Rigidbody.isKinematic = true;
            block.gameObject.SetActive(true);
            block.SetBlockData(blockData, GetMaterial(blockData.mastery));
            block.name = $"Block {blockData.id} - {blockData.grade[..1]} - {blockData.mastery}";
            return block;
        }

        private int GradeToStackIndex(string grade)
        {
            if (grade.StartsWith("6"))
            {
                return 0;
            }
            
            if (grade.StartsWith("7"))
            {
                return 1;
            }

            if (grade.StartsWith("8"))
            {
                return 2;
            }

            return -1;
        }

        private Material GetMaterial(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.Glass:
                    return glass;
                case BlockType.Stone:
                    return stone;
                case BlockType.Wood:
                    return wood;
                default:
                    return errorMaterial;
            }
        }
    }
}