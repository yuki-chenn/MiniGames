using MiniGames.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.SAS
{
    // 保存有地图数据以及每个Block的数据
    [Serializable]
    public class BlockMapData
    {
        public int rowNum;
        public int colNum;
        public int layerNum;
        public int blockCount;

        // 选择的图案
        List<int> typeIndex;

        // 0表示该点没有block，1表示上层没有block的block,-1表示上层有block的block
        public int[,,] graph;
        // 表示这个点的图案是什么 -1什么都没有,0,1,2,...
        public int[,,] pattern;
        // 下方有三个槽
        public List<int>[] zonePattern;

        // 记录需要偏移的顶层block
        public Dictionary<string, int> offsetBlock;

        // 已经插入槽中的block
        public string[] slotName;
        public int[] slotPattern;

        // 记录操作
        public List<string> stack;


        public BlockMapData(MapData mapdata,int patternCount)
        {
            this.rowNum = mapdata.rowNum;
            this.colNum = mapdata.colNum;
            this.layerNum = mapdata.layerNum;
            this.blockCount = mapdata.blockCount;

            offsetBlock = new Dictionary<string, int>();
            stack = new List<string>();
            slotName = new string[Constants.MAX_SLOT_NUM];
            slotPattern = new int[Constants.MAX_SLOT_NUM];
            for (int i = 0; i < Constants.MAX_SLOT_NUM; ++i) slotPattern[i] = -1;
            zonePattern = new List<int>[3];
            for (int i = 0; i < 3; ++i) zonePattern[i] = new List<int>();
            

            #region 生成graph
            graph = new int[layerNum, rowNum, colNum];

            // 遍历MapData中的每一层数据
            foreach (var layerEntry in mapdata.layerData)
            {
                int layerIndex = int.Parse(layerEntry.Key);
                List<BlockData> blocks = layerEntry.Value;

                // 遍历这一层的block并更新graph
                foreach (BlockData block in blocks)
                {
                    // 确保row和col在有效范围内
                    if (block.row >= 0 && block.row < rowNum && block.col >= 0 && block.col < colNum)
                    {
                        graph[layerIndex, block.row, block.col] = 1; // 默认标记为1，表示当前层有block
                        if(block.type > 0) offsetBlock.Add(string.Format(Constants.LRC_TPL, layerIndex, block.row, block.col), block.type);
                    }
                }
            }

            UpdateOccusion();

            #endregion

            #region 生成pattern

            // 随机获取patternCount个0-15之间的随机值
            typeIndex = RandomUtil.GetRandomIndexList(patternCount, 0, Constants.MAX_TYPE_NUMS);

            int pairs = blockCount / 3;
            pattern = new int[layerNum, rowNum, colNum];
            List<int> list = new List<int>();
            for (int i = 0; i < patternCount; ++i)
            {
                int c = pairs / (patternCount-i);
                pairs -= c;

                for (int j = 0; j < c * 3; ++j) list.Add(typeIndex[i]);
            }

            RandomUtil.Shuffle(ref list);
            int index = 0;
            for (int layer = 0; layer < layerNum; layer++)
            {
                for (int row = 0; row < rowNum; row++)
                {
                    for (int col = 0; col < colNum; col++)
                    {
                        if (Math.Abs(graph[layer, row, col]) == 1)
                        {
                            pattern[layer, row, col] = list[index++];
                        }
                        else
                        {
                            pattern[layer, row, col] = -1;
                        }
                    }
                }
            }
            #endregion
        }

        private void UpdateOccusion()
        {
            // 根据上下层的block关系，进一步标记graph
            for (int layer = layerNum - 1; layer >= 0; layer--)
            {
                for (int row = 0; row < rowNum; row++)
                {
                    for (int col = 0; col < colNum; col++)
                    {
                        // 如果当前层有block
                        if (Math.Abs(graph[layer, row, col]) == 1)
                        {
                            graph[layer, row, col] = IsTop(layer, row, col) ? 1 : -1;
                        }
                    }
                }
            }
        }

        // 判断该block上面是否有block
        private bool IsTop(int layer,int row,int col)
        {
            if (graph[layer, row, col] == 0) return false;
            for (int dr = -1; dr <= 1; ++dr)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    int sr = row + dr, sc = col + dc;
                    if (sr < 0 || sr >= rowNum || sc < 0 || sc >= colNum) continue;
                    for(int li = layer + 1; li < layerNum; ++li)
                    {
                        if (Math.Abs(graph[li, sr, sc]) == 1) return false;
                    }
                }
            }
            return true;
        }

        // 加入到Slot
        public int AddToSlot(int layer,int row,int col)
        {
            if (slotPattern[Constants.MAX_SLOT_NUM - 1] != -1) return -1;

            // 更新slot值
            int insertIndex = -1;
            int p = row == -1 ? zonePattern[col][layer] : pattern[layer, row, col];

            if(p == -1)
            {
                Debug.LogError("点击了一个没有图案的block");
                return -1;
            }

            // 先看有没有相同的
            bool hasSame = false;
            for (int i = 0; i < Constants.MAX_SLOT_NUM; ++i)
            {
                if (slotPattern[i] == p)
                {
                    hasSame = true;
                    break;
                }
            }

            if (!hasSame)
            {
                // 没有相同的，插入到第一个-1处
                for (int i = 0; i < Constants.MAX_SLOT_NUM; ++i)
                {
                    if (slotPattern[i] == -1)
                    {
                        insertIndex = i;
                        break;
                    }
                }
            }
            else
            {
                // 有相同的，找到最后一个相同的
                for (int i = 0; i < Constants.MAX_SLOT_NUM - 1; ++i)
                {
                    if (slotPattern[i] == p && slotPattern[i+1] != p)
                    {
                        insertIndex = i + 1;
                        break;
                    }
                }
            }

            if(slotPattern[insertIndex] != -1)
            {
                // 把insertIndex之后的都向后移动一格
                for (int i = Constants.MAX_SLOT_NUM - 2; i >= insertIndex; --i)
                {
                    slotName[i + 1] = slotName[i];
                    slotPattern[i + 1] = slotPattern[i];
                }
            }

            // 赋值
            slotPattern[insertIndex] = p;
            slotName[insertIndex] = string.Format(Constants.LRC_TPL, layer, row, col);
            
            // 设置该位置没有
            if(row == -1)
            {
                zonePattern[col].RemoveAt(layer);
            }
            else
            {
                graph[layer, row, col] = 0;
                pattern[layer, row, col] = -1;
            }


            // 加入Stack
            stack.Add(string.Format(Constants.LRC_TPL, layer, row, col));

            // 更新遮挡关系
            UpdateOccusion();

            return p;
        }

        // Slot三消,一次调用只会消除一个
        public int Match()
        {
            // 消除slot
            for(int i = 0; i < Constants.MAX_SLOT_NUM - 2; ++i)
            {
                if (slotPattern[i] == -1) break;

                if (slotPattern[i] == slotPattern[i + 1] && slotPattern[i + 1] == slotPattern[i + 2])
                {
                    // 消除stack
                    stack.Remove(slotName[i]);
                    stack.Remove(slotName[i+1]);
                    stack.Remove(slotName[i+2]);

                    // 消除slot
                    slotPattern[i] = -1;
                    slotPattern[i+1] = -1;
                    slotPattern[i+2] = -1;
                    // 消除并前移
                    for (int j = i + 3; j < Constants.MAX_SLOT_NUM; ++j)
                    {
                        slotName[j - 3] = slotName[j];
                        slotPattern[j - 3] = slotPattern[j];
                        slotName[j] = null;
                        slotPattern[j] = -1;
                    }
                    return i;
                }
            }
            

            return -1;
        }

        // 撤回一个
        public BlockData Withdraw()
        {
            if (stack.Count == 0) return null;

            string block = stack[stack.Count - 1];
            
            string[] values = block.Split('_');
            int layer = int.Parse(values[0]);
            int row = int.Parse(values[1]);
            int col = int.Parse(values[2]);

            // stack移除
            stack.Remove(block);

            // slot移除
            int slotIndex = -1;
            for(int i = 0; i < Constants.MAX_SLOT_NUM; ++i)
            {
                if(slotName[i] == block)
                {
                    slotIndex = i;
                    break;
                }
            }

            if(slotIndex == -1)
            {
                Debug.Log("撤回时，slot没找到block:" + block);
                return null;
            }

            int p = slotPattern[slotIndex];

            slotName[slotIndex] = null;
            slotPattern[slotIndex] = -1;
            for(int i = slotIndex; i < Constants.MAX_SLOT_NUM - 1; ++i)
            {
                slotName[i] = slotName[i + 1];
                slotPattern[i] = slotPattern[i + 1];
            }

            // pattern修改
            if(row == -1)
            {
                zonePattern[col].Add(p);
            }
            else
            {
                pattern[layer, row, col] = p;
                graph[layer, row, col] = 1;
            }
            
            UpdateOccusion();

            return new BlockData
            {
                blockId = string.Format(Constants.LRC_TPL, layer, row, col),
                layer = layer,
                row = row,
                col = col,
                type = p
            };

        }

        // 洗牌所有图案
        public void Shuffle()
        {
            List<int> patternList = new List<int>();
            for (int layer = 0; layer <layerNum; layer++)
            {
                for (int row = 0; row < rowNum; row++)
                {
                    for (int col = 0; col < colNum; col++)
                    {
                        // 如果当前层有block
                        if (pattern[layer, row, col] != -1)
                        {
                            patternList.Add(pattern[layer, row, col]);
                        }
                    }
                }
            }
            for(int col = 0; col < 3; ++col)
            {
                var list = zonePattern[col];
                for(int layer = 0; layer < list.Count; ++layer)
                {
                    patternList.Add(list[layer]);
                }
            }
            RandomUtil.Shuffle(ref patternList);
            int pi = 0;
            for (int layer = 0; layer < layerNum; layer++)
            {
                for (int row = 0; row < rowNum; row++)
                {
                    for (int col = 0; col < colNum; col++)
                    {
                        // 如果当前层有block
                        if (pattern[layer, row, col] != -1)
                        {
                            pattern[layer, row, col] = patternList[pi++];
                        }
                    }
                }
            }
            for (int col = 0; col < 3; ++col)
            {
                var list = zonePattern[col];
                for (int layer = 0; layer < list.Count; ++layer)
                {
                    list[layer] = patternList[pi++];
                }
            }
        }

        // 拿出前三个
        public int Pop3()
        {
            if (slotPattern[0] == -1) return -1;

            // 先把slot的前三个放到zone中
            for(int i = 0; i < 3; ++i)
            {
                if (slotPattern[i] == -1) break;
                // 加入zone
                zonePattern[i].Add(slotPattern[i]);
                // 把stack清理掉
                stack.Remove(slotName[i]);
                slotPattern[i] = -1;
                slotName[i] = null;
            }

            // 把后面的往前移
            for(int i = 3; i < Constants.MAX_SLOT_NUM; ++i)
            {
                slotPattern[i - 3] = slotPattern[i];
                slotName[i - 3] = slotName[i];
                slotPattern[i] = -1;
                slotName[i] = null;

            }

            return 0;
        }


    }




    // 只保存地图的数据，没有图案的数据
    [Serializable]
    public class MapData
    {
        public string mapId;
        public int rowNum;
        public int colNum;
        public int layerNum;
        public int blockCount;

        public Dictionary<string, List<BlockData>> layerData = new Dictionary<string, List<BlockData>>();

        public MapData()
        {
            
        }

        public void RefreshId()
        {
            mapId = RandomUtil.RandomAlphaNumericId(12);
        }

        public void SetData(Dictionary<int, int[,]> data, int layerCount,int blockCount)
        {
            this.blockCount = blockCount;
            layerNum = layerCount;
            layerData.Clear(); // 清空现有的 layerData
            foreach (var kvp in data)
            {
                int layerIndex = kvp.Key;
                int[,] layerArray = kvp.Value;

                List<BlockData> blocks = new List<BlockData>();

                int rows = layerArray.GetLength(0);
                int cols = layerArray.GetLength(1);

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (layerArray[r,c] >= 1) // 假设 0 表示没有 block
                        {
                            BlockData block = new BlockData
                            {
                                blockId = string.Format(Constants.LRC_TPL, layerIndex, r, c),
                                layer = layerIndex,
                                row = r,
                                col = c,
                                type = layerArray[r, c] - 1
                            };
                            blocks.Add(block);
                        }
                    }
                }

                // 将这一层的数据添加到 layerData 中
                layerData[layerIndex.ToString()] = blocks;
            }
        }

    }

    // 地图上的block位置，type:0（正常）,1,2,3,4（一摞，上下左右）
    [Serializable]
    public class BlockData
    {
        public string blockId;
        public int layer;
        public int row;
        public int col;
        public int type;
    }


}
