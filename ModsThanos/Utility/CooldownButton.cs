using System;
using System.Collections.Generic;
using UnityEngine;
using ModsThanos.Utility;
using ModsThanos.Utility.Enumerations;
using Reactor.Utilities.Extensions;

namespace ModsThanos {
    public class CooldownButton {
        public static List<CooldownButton> buttons = new List<CooldownButton>();
        public KillButton killButton;
        private Color startColorButton = new Color(255, 255, 255);
        private Color startColorText = new Color(255, 255, 255);
        public Vector2 PositionOffset = Vector2.zero;
        public float MaxTimer = 0f;
        public string buttonText;
        public float Timer = 0f;
        public float EffectDuration = 0f;
        public bool isEffectActive;
        public bool hasEffectDuration;
        public bool enabled = true;
        public Visibility visibility;
        private string ResourceName;
        private Action OnClick;
        private Action OnEffectEnd;
        private Action OnUpdate;
        private Sprite sprite;
        private HudManager hudManager;
        private float pixelsPerUnit;
        private bool canUse;

        public CooldownButton(Action OnClick, float Cooldown, string buttonText, string ImageEmbededResourcePath, float PixelsPerUnit, Vector2 PositionOffset, Visibility visibility, HudManager hudManager, float EffectDuration, Action OnEffectEnd, Action OnUpdate) {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.OnEffectEnd = OnEffectEnd;
            this.OnUpdate = OnUpdate;
            this.PositionOffset = PositionOffset;
            this.EffectDuration = EffectDuration;
            this.visibility = visibility;
            ResourceName = ImageEmbededResourcePath;
            pixelsPerUnit = PixelsPerUnit;
            this.sprite = HelperSprite.LoadSpriteFromEmbeddedResources(ResourceName, pixelsPerUnit);
            MaxTimer = Cooldown;
            this.buttonText = ModTranslation.getString("buttonText" + buttonText);
            Timer = MaxTimer;
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start();
        }

        public CooldownButton(Action OnClick, float Cooldown, string buttonText, string ImageEmbededResourcePath, float PixelsPerUnit, Vector2 PositionOffset, Visibility visibility, HudManager hudManager, Action OnUpdate) {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.OnUpdate = OnUpdate;
            ResourceName = ImageEmbededResourcePath;
            this.pixelsPerUnit = PixelsPerUnit;
            this.PositionOffset = PositionOffset;
            this.visibility = visibility;
            this.sprite = HelperSprite.LoadSpriteFromEmbeddedResources(ResourceName, pixelsPerUnit);
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            this.buttonText = ModTranslation.getString("buttonText" + buttonText);
            hasEffectDuration = false;
            buttons.Add(this);
            Start();
        }

        private void Start() {
            killButton = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.transform.parent);
            startColorButton = killButton.graphic.color;
            startColorText = killButton.cooldownTimerText.color;
            killButton.gameObject.SetActive(true);
            killButton.graphic.enabled = true;
            killButton.graphic.sprite = sprite;
            killButton.buttonLabelText.GetComponent<TextTranslatorTMP>().Destroy();
            killButton.buttonLabelText.text = buttonText;
           PassiveButton button = killButton.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction) listener);
            void listener() {
                if (Timer < 0f && canUse) {
                    killButton.graphic.color = new Color(1f, 1f, 1f, 0.3f);
                    if (hasEffectDuration) {
                        isEffectActive = true;
                        Timer = EffectDuration;
                        killButton.cooldownTimerText.color = new Color(0, 255, 0);
                    } else {
                        Timer = MaxTimer;
                    }

                    OnClick();
                }
            }
        }

        public static void HudUpdate() {
            buttons.RemoveAll(item => item.killButton == null);
            for (int i = 0; i < buttons.Count; i++) {
                buttons[i].killButton.graphic.sprite = buttons[i].sprite;
                buttons[i].OnUpdate();
                buttons[i].Update();
            }
        }

        private void Update() {
            killButton.buttonLabelText.color = Palette.EnabledColor;

            if (killButton.transform.localPosition.x > 0f)
                killButton.transform.localPosition = new Vector3(((killButton.transform.localPosition.x - 1.3f) * -1) - 1.5f, killButton.transform.localPosition.y - 1.3f, killButton.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);
            if (Timer < 0f) {
                killButton.graphic.color = new Color(1f, 1f, 1f, 1f);
                if (isEffectActive) {
                    killButton.cooldownTimerText.color = startColorText;
                    Timer = MaxTimer;
                    isEffectActive = false;
                    OnEffectEnd();
                }
            } else {
                if (canUse && (isEffectActive || PlayerControl.LocalPlayer.CanMove))
                    Timer -= UnityEngine.Time.deltaTime;

                killButton.graphic.color = new Color(1f, 1f, 1f, 0.3f);
            }
            killButton.gameObject.SetActive(canUse);
            killButton.graphic.enabled = canUse;
            if (canUse) {
                killButton.graphic.material.SetFloat("_Desat", 0f);
                killButton.SetCoolDown(Timer, MaxTimer);
            }
        }

        public void SetCanUse(bool value) {
            this.canUse = value;
        }

        public bool GetCanUse() {
            return this.canUse;
        }
    }
}