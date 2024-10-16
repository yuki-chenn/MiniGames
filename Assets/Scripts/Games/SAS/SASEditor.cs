using LitJson;
using MiniGames.SAS;
using MiniGames.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.SAS
{
    public class SASEditor : MonoBehaviour
    {
        public Slider sliderRow;
        public Slider sliderCol;
        public Text txtRow;
        public Text txtCol;
        public Text txtMapId;
        public Text txtBlockCount;
        public Button btnRefresh;
        public Button btnLock;
        public Button btnImport;
        public Button btnExport;

        public int maxRowVal;
        public int minRowVal;
        public int maxColVal;
        public int minColVal;

        public bool isLock = false;

        public GameObject gridGo;
        public GameObject pointGo;
        public GameObject showLayerItemGo;
        public GameObject editLayerItemGo;
        public GameObject blockLayerGo;
        public GameObject blockGo;
        public Transform showContent;
        public Transform selectContent;
        public Transform gridContent;
        public Transform blockContent;

        public Color[] colorArray = new Color[]
        {
        new Color(0.9f, 0.2f, 0.3f),  // ��ɫ
        new Color(0.2f, 0.7f, 0.9f),  // ǳ��ɫ
        new Color(0.1f, 0.6f, 0.3f),  // ��ɫ
        new Color(0.9f, 0.5f, 0.2f),  // ��ɫ
        new Color(0.6f, 0.3f, 0.8f),  // ��ɫ
        new Color(0.8f, 0.8f, 0.2f),  // ��ɫ
        new Color(0.3f, 0.9f, 0.6f),  // ǳ��ɫ
        new Color(0.9f, 0.4f, 0.6f),  // ��ɫ
        new Color(0.5f, 0.7f, 0.9f),  // ǳ��ɫ
        new Color(0.7f, 0.2f, 0.4f),  // �ƺ�ɫ
        new Color(0.2f, 0.5f, 0.9f),  // ��ɫ
        new Color(0.4f, 0.8f, 0.7f),  // ����ɫ
        new Color(0.9f, 0.3f, 0.7f),  // ǳ��ɫ
        new Color(0.9f, 0.7f, 0.2f),  // ���ɫ
        new Color(0.3f, 0.9f, 0.2f),  // ����ɫ
        new Color(0.7f, 0.5f, 0.9f),  // ����ɫ
        new Color(0.8f, 0.2f, 0.5f),  // ���ɫ
        new Color(0.2f, 0.8f, 0.9f),  // ǳ��ɫ
        new Color(0.5f, 0.9f, 0.4f),  // ǳ��ɫ
        new Color(0.9f, 0.8f, 0.4f),  // ǳ��ɫ
        new Color(0.6f, 0.9f, 0.3f),  // ����ɫ
        new Color(0.4f, 0.2f, 0.8f),  // ����ɫ
        new Color(0.9f, 0.6f, 0.4f),  // ��ɫ
        new Color(0.8f, 0.3f, 0.6f),  // ����ɫ
        new Color(0.2f, 0.7f, 0.8f),  // ����ɫ
        new Color(0.3f, 0.6f, 0.9f),  // ����ɫ
        new Color(0.9f, 0.4f, 0.2f),  // ǳ��ɫ
        new Color(0.5f, 0.9f, 0.8f),  // ����ɫ
        new Color(0.9f, 0.2f, 0.5f),  // õ��ɫ
        new Color(0.7f, 0.4f, 0.9f),  // ����ɫ
        };

        public MapData mapdata;
        public int selectLayer = 0;
        public int layerCount = 0;
        public int blockCount = 0;
        public Dictionary<int, int[,]> map = new Dictionary<int, int[,]>();
        public Dictionary<int, bool> isLayerShow = new Dictionary<int, bool>();

        // 1 ���� 2 3 4 5 һ�����������ң�
        public int typeMode = 1;

        private void Awake()
        {
            sliderRow.minValue = minRowVal;
            sliderRow.maxValue = maxRowVal;
            sliderCol.minValue = minColVal;
            sliderCol.maxValue = maxColVal;

            mapdata = new MapData();
            mapdata.RefreshId();
            AddLayer();
            mapdata.rowNum = minRowVal;
            mapdata.colNum = minColVal;
        }

        private void Start()
        {
            sliderRow.onValueChanged.AddListener((v) =>
            {
                mapdata.rowNum = (int)v;
                UpdateAll();
            });

            sliderCol.onValueChanged.AddListener((v) =>
            {
                mapdata.colNum = (int)v;
                UpdateAll();
            });

            btnRefresh.onClick.AddListener(() =>
            {
                mapdata.RefreshId();
                txtMapId.text = mapdata.mapId;
            });

            btnLock.onClick.AddListener(() =>
            {
                isLock = !isLock;
                sliderRow.enabled = !isLock;
                sliderCol.enabled = !isLock;
                btnLock.GetComponentInChildren<Text>().text = isLock ? "����" : "����";
            });

            btnExport.onClick.AddListener(() =>
            {
                if (blockCount % 3 != 0)
                {
                    Debug.LogError("�ܷ�����" + blockCount + "����3�ı���");
                    return;
                }
                ExportMapDataToJson();
            });

            btnImport.onClick.AddListener(() =>
            {
                LoadMapDataFromJson();
            });

            UpdateAll();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                typeMode += 1;
                if (typeMode > 5) typeMode = 1;
                Debug.Log("����ģʽ:" + typeMode);
            }
        }

        private void UpdateAll()
        {
            UpdateScroll();
            UpdateLayer();
            UpdateGrid();
            UpdateBlock();
        }

        // ���¹����б�
        private void UpdateScroll()
        {
            // show

            for (int i = 0; i < layerCount; ++i)
            {
                GameObject child = null;
                if (i < showContent.childCount)
                {
                    child = showContent.GetChild(i).gameObject;
                }
                else
                {
                    child = Instantiate(showLayerItemGo, showContent);
                }

                child.GetComponentInChildren<Text>().text = string.Format("Layer {0}", i);
                child.transform.Find("Color").GetComponent<Image>().color = colorArray[i % colorArray.Length];
                child.transform.Find("ShowLayerBtn").GetComponent<Image>().color =
                    isLayerShow.GetValueOrDefault(i, false) ? colorArray[i % colorArray.Length] : Color.white;

                var btnShow = child.transform.Find("ShowLayerBtn").GetComponent<Button>();
                int index = i;
                btnShow.onClick.RemoveAllListeners();
                btnShow.onClick.AddListener(() =>
                {
                    if (selectLayer == index) return;
                    isLayerShow[index] = !isLayerShow.GetValueOrDefault(index, false);
                    UpdateAll();
                });

                child.name = i.ToString();
                child.SetActive(true);
            }

            for (int i = layerCount; i < showContent.childCount; ++i)
            {
                showContent.GetChild(i).gameObject.SetActive(false);
            }

            // edit

            for (int i = 0; i <= layerCount; ++i)
            {
                GameObject child = null;
                if (i < selectContent.childCount)
                {
                    child = selectContent.GetChild(i).gameObject;
                }
                else
                {
                    child = Instantiate(editLayerItemGo, selectContent);
                }

                child.name = i.ToString();

                if (i == layerCount)
                {
                    // add layer
                    child.transform.Find("Select").gameObject.SetActive(false);
                    child.transform.Find("Add").gameObject.SetActive(true);
                    var addBtn = child.transform.Find("Add/AddLayerBtn").GetComponent<Button>();
                    addBtn.onClick.RemoveAllListeners();
                    addBtn.onClick.AddListener(() =>
                    {
                        AddLayer();
                        UpdateAll();
                    });
                }
                else
                {
                    // edit layer
                    child.transform.Find("Select").gameObject.SetActive(true);
                    child.transform.Find("Add").gameObject.SetActive(false);
                    // ��ť
                    child.GetComponentInChildren<Text>().text = string.Format("Layer {0}", i);
                    var editBtn = child.transform.Find("Select/EditLayerBtn").GetComponent<Button>();
                    var delBtn = child.transform.Find("Select/BtnDelete").GetComponent<Button>();

                    int index = i;
                    editBtn.onClick.RemoveAllListeners();
                    editBtn.onClick.AddListener(() =>
                    {
                        selectLayer = index;
                        isLayerShow[index] = true;
                        UpdateAll();
                    });

                    delBtn.onClick.RemoveAllListeners();
                    delBtn.onClick.AddListener(() =>
                    {
                        if (selectLayer == index) selectLayer = 0;
                        DeleteLayer(index);
                        UpdateAll();
                    });

                    child.transform.Find("Select/Selected").gameObject.SetActive(i == selectLayer);
                }

                child.SetActive(true);
            }

            for (int i = layerCount + 1; i < selectContent.childCount; ++i)
            {
                selectContent.GetChild(i).gameObject.SetActive(false);
            }

        }

        // ������ʾ��һ��ĵ�
        private void UpdateLayer()
        {
            for (int i = 0; i < layerCount; ++i)
            {
                GameObject layer = null;
                if (i < gridContent.childCount)
                {
                    layer = gridContent.GetChild(i).gameObject;
                }
                else
                {
                    layer = Instantiate(gridGo, gridContent);
                }

                layer.name = i.ToString();
                layer.SetActive(i == selectLayer);
            }

            for (int i = layerCount; i < gridContent.childCount; ++i)
            {
                gridContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void UpdateGrid()
        {
            //Debug.Log("row:" + mapdata.rowNum + ",col:" + mapdata.colNum);
            txtMapId.text = mapdata.mapId;
            txtBlockCount.text = blockCount.ToString();
            txtRow.text = mapdata.rowNum.ToString();
            txtCol.text = mapdata.colNum.ToString();

            // ���µ�
            for (int i = 0; i < layerCount; ++i) UpdateGridLayer(i);
        }

        private void UpdateGridLayer(int layer)
        {
            RectTransform grid = gridContent.GetChild(layer) as RectTransform;

            float width = 20 + (mapdata.colNum - 1) * 50;
            float height = 20 + (mapdata.rowNum - 1) * 50;
            grid.sizeDelta = new Vector2(width, height);

            var g = map[layer];

            for (int i = 0; i < mapdata.rowNum * mapdata.colNum; ++i)
            {
                GameObject go = null;

                if (i < grid.childCount)
                {
                    go = grid.GetChild(i).gameObject;
                }
                else
                {
                    go = Instantiate(pointGo, grid);
                }

                int r = i / mapdata.colNum;
                int c = i % mapdata.colNum;

                var btnPoint = go.GetComponent<Button>();
                btnPoint.onClick.RemoveAllListeners();

                if (g[r, c] == 0)
                {
                    // ��ɫ
                    go.GetComponent<Image>().color = Color.white;
                    btnPoint.onClick.AddListener(() =>
                    {
                        g[r, c] = typeMode;
                        blockCount++;
                        for (int dx = -1; dx <= 1; ++dx)
                        {
                            for (int dy = -1; dy <= 1; ++dy)
                            {
                                if (dx == 0 && dy == 0) continue;
                                int sx = r + dx, sy = c + dy;
                                if (sx < 0 || sx >= maxRowVal || sy < 0 || sy >= maxColVal) continue;
                                g[sx, sy]--;
                            }
                        }
                        UpdateAll();
                    });
                }
                else if (g[r, c] >= 1)
                {
                    // ��ɫ
                    go.GetComponent<Image>().color = g[r, c] == 1 ? Color.green : Color.yellow;
                    btnPoint.onClick.AddListener(() =>
                    {
                        g[r, c] = 0;
                        blockCount--;
                        for (int dx = -1; dx <= 1; ++dx)
                        {
                            for (int dy = -1; dy <= 1; ++dy)
                            {
                                if (dx == 0 && dy == 0) continue;
                                int sx = r + dx, sy = c + dy;
                                if (sx < 0 || sx >= maxRowVal || sy < 0 || sy >= maxColVal) continue;
                                g[sx, sy]++;
                            }
                        }
                        UpdateAll();
                    });
                }
                else if (g[r, c] < 0)
                {
                    // ��ɫ
                    go.GetComponent<Image>().color = Color.red;
                }

                go.name = string.Format("{0}-{1}", r, c);
                go.SetActive(true);
            }

            for (int i = mapdata.rowNum * mapdata.colNum; i < grid.childCount; ++i)
            {
                grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void UpdateBlock()
        {
            for (int i = 0; i < layerCount; ++i)
            {
                GameObject layer = null;
                if (i < blockContent.childCount)
                {
                    layer = blockContent.GetChild(i).gameObject;
                }
                else
                {
                    layer = Instantiate(blockLayerGo, blockContent);
                }

                layer.name = i.ToString();
                UpdateBlockLayer(i);
                layer.SetActive(isLayerShow.GetValueOrDefault(i, false));
            }

            for (int i = layerCount; i < blockContent.childCount; ++i)
            {
                blockContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void UpdateBlockLayer(int layer)
        {
            var g = map[layer];
            var blockLayer = blockContent.GetChild(layer);
            var gridLayer = gridContent.GetChild(layer);
            int bi = 0;

            for (int i = 0; i < mapdata.rowNum; ++i)
            {
                for (int j = 0; j < mapdata.colNum; ++j)
                {
                    int index = i * mapdata.colNum + j;

                    if (g[i, j] >= 1)
                    {
                        GameObject block = null;
                        if (bi < blockLayer.childCount)
                        {
                            block = blockLayer.GetChild(bi).gameObject;
                        }
                        else
                        {
                            block = Instantiate(blockGo, blockLayer);
                        }

                        bi++;
                        //Debug.Log(string.Format("{0},{1},{2}[{3}]:{4}",layer,i,j,index, gridLayer.GetChild(index).localPosition));
                        block.transform.localPosition = new Vector2(10 + j * 50, -10 - i * 50);
                        block.GetComponent<Image>().color = colorArray[layer % colorArray.Length];
                        block.SetActive(true);
                    }
                }
            }
            for (int i = bi; i < blockLayer.childCount; ++i) blockLayer.GetChild(i).gameObject.SetActive(false);
        }

        private void DeleteLayer(int layer)
        {
            if (layerCount == 1)
            {
                Debug.LogError("������һ��layer");
                return;
            }

            for (int i = layer + 1; i < layerCount; ++i)
            {
                map[i - 1] = map[i];
                isLayerShow[i - 1] = isLayerShow[i];
            }
            map.Remove(layerCount - 1);
            isLayerShow.Remove(layerCount - 1);
            layerCount--;
            if (selectLayer >= layerCount) selectLayer = layerCount - 1;
        }

        private void AddLayer()
        {
            map.Add(layerCount, new int[maxRowVal, maxColVal]);
            isLayerShow.Add(layerCount, true);
            layerCount++;
            selectLayer = layerCount - 1;
        }


        // �� MapData ����Ϊ JSON �ļ�
        public void ExportMapDataToJson()
        {
            // Ϊmapdata��ֵ
            mapdata.SetData(map, layerCount, blockCount);
            string filename = string.Format("SAS/{0}.json", mapdata.mapId);
            IOUtil.ExportJsonToSA<MapData>(mapdata, filename);
        }

        // �� JSON �ļ����� MapData
        public void LoadMapDataFromJson()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Select a file", "", "json");
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("No file selected.");
                return;
            }
            else
            {
                mapdata = IOUtil.LoadJsonFromSA<MapData>(path);
                LoadMap();

                sliderRow.value = mapdata.rowNum;
                sliderCol.value = mapdata.colNum;

                UpdateAll();
            }

#else
        Debug.LogError("not editor");
#endif

        }


        private void LoadMap()
        {
            if (mapdata == null) return;

            layerCount = mapdata.layerNum;
            blockCount = mapdata.blockCount;
            Dictionary<int, int[,]> layerDictionary = new Dictionary<int, int[,]>();
            // ��mapdata.layerData��ת����Dictionary<int, int[,]>
            foreach (var layer in mapdata.layerData)
            {
                int layerIndex = int.Parse(layer.Key);
                List<BlockData> blocks = layer.Value;

                // ����һ�� rowNum x colNum �Ķ�ά����
                int[,] blockArray = new int[maxRowVal, maxColVal];

                // �� BlockData �е�������䵽��ά������
                foreach (BlockData block in blocks)
                {
                    blockArray[block.row, block.col] = 1;
                    for (int dx = -1; dx <= 1; ++dx)
                    {
                        for (int dy = -1; dy <= 1; ++dy)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int sx = block.row + dx, sy = block.col + dy;
                            if (sx < 0 || sx >= maxRowVal || sy < 0 || sy >= maxColVal) continue;
                            blockArray[sx, sy]--;
                        }
                    }
                }

                // ���ò�Ķ�ά������ӵ��ֵ���
                layerDictionary.Add(layerIndex, blockArray);
                if (isLayerShow.ContainsKey(layerIndex)) isLayerShow[layerIndex] = true;
                else isLayerShow.Add(layerIndex, true);
            }

            map = layerDictionary;
        }



    }
}


