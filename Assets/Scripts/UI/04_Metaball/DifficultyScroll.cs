/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using MiniGames.Enum;
using MiniGames.SAS;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace FancyScrollView.Example04
{
    class DifficultyScroll : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        public int cellCount = 4;

        void Start()
        {
            scrollView.OnSelectionChanged(OnSelectionChanged);

            var items = Enumerable.Range(0, cellCount)
                .Select(i => new ItemData(((Difficulty)i).ToString()))
                .ToList();

            scrollView.UpdateData(items);
            scrollView.UpdateSelection(SASGameManager.Instance.selectDiff);
            scrollView.JumpTo(SASGameManager.Instance.selectDiff);
        }

        void OnSelectionChanged(int index)
        {
            SASGameManager.Instance.ChangeDifficulty(index);
        }
    }
}
