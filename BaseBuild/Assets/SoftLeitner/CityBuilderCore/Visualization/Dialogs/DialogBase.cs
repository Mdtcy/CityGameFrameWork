﻿using UnityEngine;
using UnityEngine.UI;

namespace CityBuilderCore
{
    public class DialogBase : MonoBehaviour
    {
        public bool PauseGame;
        public ContentSizeFitter Fitter;

        public bool IsDialogActive { get; private set; }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            if (!IsDialogActive)
                Deactivate();
        }

        protected virtual void Update()
        {
            if (!PauseGame && IsDialogActive)
            {
                updateContent(false);
                updateLayout();
            }
        }

        public virtual void Activate()
        {
            IsDialogActive = true;
            gameObject.SetActive(true);
            updateContent(true);
            updateLayout();

            if (PauseGame)
                Dependencies.Get<IGameSpeed>().Pause();
        }

        public virtual void Deactivate()
        {
            IsDialogActive = false;
            gameObject.SetActive(false);

            if (PauseGame)
                Dependencies.Get<IGameSpeed>().Resume();
        }

        protected virtual void updateContent(bool initiate)
        {

        }

        protected virtual void updateLayout()
        {
            if (Fitter)
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Fitter.transform);
        }
    }
}