using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MiniGames.SAS;

public class Slot : MonoBehaviour
{
    public GameObject emptyBlockPrefab;
    public float insertTime = 0.4f;
    public float matchTime = 0.4f;
    public float shuffleTime = 1f;

    public Vector2 offset;
    public Vector2 blockSize;
    public Transform mapContent;


    public List<int> patternSlot;

    public int matchLock = 0;
    public bool insertLock = false;
    public bool skillLock = false;

    private void Awake()
    {
        mapContent = transform.parent.Find("Map");
        var hlg = transform.GetComponent<HorizontalLayoutGroup>();
        offset = new Vector2(hlg.padding.left + blockSize.x / 2.0f, -(hlg.padding.top + blockSize.y / 2.0f));
    }

    private void Update()
    {
        if (matchLock > 0 || skillLock) return;
        MatchBlock();
    }

    public void InsertBlock(Transform block, int pattern)
    {
        // 先拿出去
        block.GetComponent<Button>().enabled = false;
        block.SetParent(transform.parent);

        // 计算该pattern应该插入的位置
        matchLock++;
        int insertIndex = GetInsertIndex(pattern);
        patternSlot.Insert(insertIndex, pattern);

        // 生成一个空位置
        Transform emptyBlock = Instantiate(emptyBlockPrefab, transform).transform;
        emptyBlock.SetSiblingIndex(insertIndex);

        // 计算位置
        Vector2 targetPos = (transform as RectTransform).anchoredPosition + offset + 
            new Vector2(insertIndex * 100, 0);

        // 使用 DOTween 将 Block 的位置移动到转换后的本地坐标
        (block as RectTransform).DOAnchorPos(targetPos, insertTime).SetEase(Ease.InOutQuad).
            OnComplete(()=>
            {
                // 动画结束的时候
                int index = emptyBlock.GetSiblingIndex();
                Destroy(emptyBlock.gameObject);
                block.SetParent(transform);
                block.SetSiblingIndex(index);
                matchLock--;
            });
    }

    private int GetInsertIndex(int pattern)
    {
        int n = patternSlot.Count;
        int insertIndex = -1;
        for(int i = 0; i < n; ++i)
        {
            if (patternSlot[i] == pattern)
            {
                if (i + 1 >= n || patternSlot[i + 1] != pattern)
                {
                    if (i - 1 < 0 || i - 2 < 0 ||
                        (!(patternSlot[i] == patternSlot[i - 1] && patternSlot[i] == patternSlot[i - 2])))
                        return i + 1;
                }
            }
        }
        if (insertIndex == -1) insertIndex = n;
        return insertIndex;
    }

    public void MatchBlock()
    {
        int n = patternSlot.Count;

        // 检查 patternSlot 中连续出现三个一样的数值
        List<int> matchIndices = new List<int>();
        for (int i = 0; i < n - 2; ++i)
        {
            if (patternSlot[i] == patternSlot[i + 1] && patternSlot[i] == patternSlot[i + 2])
            {
                // 添加匹配的三个索引
                matchIndices.Add(i);
                matchIndices.Add(i + 1);
                matchIndices.Add(i + 2);
                i += 2;
            }
        }

        // 如果没有找到匹配，则返回
        if (matchIndices.Count == 0)
            return;

        insertLock = true;

        SASAudioManager.Instance.PlayMatchEffect();

        List<GameObject> destoryList = new List<GameObject>();

        // 遍历匹配的索引，删除对应的Block
        foreach (int index in matchIndices)
        {
            Transform block = transform.GetChild(index);
            destoryList.Add(block.gameObject);
        }

        // 删除 patternSlot 中匹配的数值
        for (int i = matchIndices.Count - 1; i >= 0; --i)
        {
            patternSlot.RemoveAt(matchIndices[i]);
        }
        foreach (var go in destoryList)
        {
            Destroy(go);
        }

        insertLock = false;
    }

    public void WithdrawBlock(string blockName, int pattern, int row, int col)
    {
        skillLock = true;

        int withdrawIndex = -1;

        for (int i = patternSlot.Count - 1; i >= 0; --i)
        {
            if (patternSlot[i] == pattern) withdrawIndex = i;
        }

        if (withdrawIndex == -1)
        {
            Debug.LogError("Slot withdraw 错误，未找到pattern:" + pattern);
            return;
        }

        patternSlot.RemoveAt(withdrawIndex);

        // 拿出去
        var block = transform.Find(blockName);
        int ei = block.GetSiblingIndex();
        block.SetParent(mapContent);
        // 占位
        Transform emptyBlock = Instantiate(emptyBlockPrefab, transform).transform;
        emptyBlock.SetSiblingIndex(ei);

        Vector2 targetPos = row == -1 ? 
            new Vector2((SASGameManager.Instance.blockMapdata.colNum - 1) * 50 / 2.0f + (col - 1) * 100,
                        -(SASGameManager.Instance.blockMapdata.rowNum + 2) * 50) : 
            new Vector2(col * 50, -row * 50);

        SASAudioManager.Instance.PlayWithdrawEffect();
        // 使用 DOTween 将 Block 的位置移动到转换后的本地坐标
        (block as RectTransform).DOAnchorPos(targetPos, insertTime).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                skillLock = false;
                Destroy(emptyBlock.gameObject);
                EventCenter.Broadcast(EventDefine.SAS_RefreshOffsetPosition, SASGameManager.Instance.blockMapdata);
            });



    }

    public void ShuffleBlock()
    {
        skillLock = true;
        List<Vector2> originalPositions = new List<Vector2>();
        // 获取所有子物体的原始位置
        foreach (Transform child in mapContent)
        {
            originalPositions.Add((child as RectTransform).anchoredPosition);
        }

        // 获取Map的中心点
        Vector2 mapCenter = new Vector2(25 * (SASGameManager.Instance.mapdata.colNum-1),
            -25 * (SASGameManager.Instance.mapdata.rowNum-1));

        Debug.Log("mapcenter:" + mapCenter);

        SASAudioManager.Instance.PlayShuffleEffect();
        // 创建一个序列来处理洗牌动画
        Sequence shuffleSequence = DOTween.Sequence();

        // 让所有的子物体首先集中到中心点
        foreach (RectTransform child in mapContent)
        {
            shuffleSequence.Join(child.DOAnchorPos(mapCenter, shuffleTime / 2.0f).SetEase(Ease.OutQuad));
        }

        // 在集中到中心点后，添加一个延迟然后让所有物体回到原始位置
        shuffleSequence.AppendInterval(shuffleTime / 2.0f);

        // 所有物体回到原始位置
        foreach (RectTransform child in mapContent)
        {
            shuffleSequence.Join(child.DOAnchorPos(originalPositions[child.GetSiblingIndex()], shuffleTime / 2.0f).SetEase(Ease.OutQuad));
        }

        // 动画结束后解锁技能并广播事件
        shuffleSequence.OnComplete(() =>
        {
            skillLock = false;
            EventCenter.Broadcast(EventDefine.SAS_RefreshBlockMap, SASGameManager.Instance.blockMapdata);
        });


    }

    public void Pop3Block()
    {
        skillLock = true;

        // 防止拿错，先match一下
        MatchBlock();

        SASAudioManager.Instance.PlayPopEffect();
        // 把头三个移动到特定位置
        Sequence seq = DOTween.Sequence();
        GameObject[] emptyBlocks = new GameObject[3];

        for (int i = 0; i < 3; ++i)
        {
            if (i >= patternSlot.Count) break;

            var block = transform.GetChild(i);
            block.name = string.Format(Constants.LRC_TPL, 
                SASGameManager.Instance.blockMapdata.zonePattern[i].Count-1, -1, i);
            block.SetParent(mapContent);

            // 占位
            emptyBlocks[i] = Instantiate(emptyBlockPrefab, transform);
            emptyBlocks[i].transform.SetSiblingIndex(i);

            Vector2 targetPos =
                new Vector2((SASGameManager.Instance.blockMapdata.colNum - 1) * 50 / 2.0f + (i - 1) * 100,
                -(SASGameManager.Instance.blockMapdata.rowNum + 2) * 50);
            // 使用 DOTween 将 Block 的位置移动到转换后的本地坐标
            seq.Join((block as RectTransform).DOAnchorPos(targetPos, insertTime).SetEase(Ease.InOutQuad));
        }

        seq.OnComplete(() =>
        {
            skillLock = false;
            for (int i = 2; i >= 0; --i)
            {
                if (emptyBlocks[i] != null)
                {
                    Destroy(emptyBlocks[i]);
                    patternSlot.RemoveAt(i);
                }
            }
            EventCenter.Broadcast(EventDefine.SAS_RefreshBlockMap, SASGameManager.Instance.blockMapdata);
        });

    }

    public void Clear()
    {
        patternSlot.Clear();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
