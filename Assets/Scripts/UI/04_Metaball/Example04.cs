/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace FancyScrollView.Example04
{
    class Example04 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        [SerializeField] Text selectedItemInfo = default;
        public int cellCount = 4; 

        void Start()
        {
            scrollView.OnSelectionChanged(OnSelectionChanged);

            var items = Enumerable.Range(0, cellCount)
                .Select(i => new ItemData($"Cell {i}"))
                .ToList();

            scrollView.UpdateData(items);
            scrollView.UpdateSelection(0);
            scrollView.JumpTo(0);
        }

        void OnSelectionChanged(int index)
        {
            
        }
    }
}
