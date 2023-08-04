﻿// Simple Scroll-Snap - https://assetstore.unity.com/packages/tools/gui/simple-scroll-snap-140884
// Copyright (c) Daniel Lochner

using System;
using UnityEngine;
using UnityEngine.UI;

namespace DanielLochner.Assets.SimpleScrollSnap
{
    public class DynamicContent : MonoBehaviour
    {
        #region Fields
        [SerializeField] private GameObject panelPrefab;
        [SerializeField] private Toggle togglePrefab;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private InputField addInputField, removeInputField;
        public SimpleScrollSnap scrollSnap;

        private float toggleWidth;
        #endregion

        #region Methods
        private void Awake()
        {
            toggleWidth = (togglePrefab.transform as RectTransform).sizeDelta.x * (Screen.width / 2048f); ;
        }

        public void Add(int index)
        {
            // Pagination
            Toggle toggle = Instantiate(togglePrefab, scrollSnap.Pagination.GetComponent<RectTransform>());
            toggle.group = toggleGroup;

            // Panel
            scrollSnap.Add(panelPrefab, index);
        }
        public void AddAtIndex()
        {
            Add(Convert.ToInt32(addInputField.text));
        }
        public void AddToFront()
        {
            Add(0);
        }
        public void AddToBack()
        {
            Add(scrollSnap.NumberOfPanels);
        }

        public void Remove(int index)
        {
            if (scrollSnap.NumberOfPanels > 0)
            {
                // Pagination
                DestroyImmediate(scrollSnap.Pagination.transform.GetChild(scrollSnap.NumberOfPanels - 1).gameObject);
                //scrollSnap.Pagination.transform.position += new Vector3(toggleWidth / 2f, 0, 0);

                // Panel
                scrollSnap.Remove(index);
            }
        }
        public void RemoveAtIndex()
        {
            Remove(Convert.ToInt32(removeInputField.text));
        }
        public void RemoveFromFront()
        {
            Remove(0);
        }
        public void RemoveFromBack()
        {
            if (scrollSnap.NumberOfPanels > 0)
            {
                Remove(scrollSnap.NumberOfPanels - 1);
            }
            else
            {
                Remove(0);
            }
        }
        #endregion
    }
}