using System;
using System.Collections.Generic;
using Reactor.Localization.Utilities;
using Reactor.Utilities;

namespace ModsThanos.CustomOption
{
    public class CustomOption
    {
        public static List<CustomOption> AllOptions = new List<CustomOption>();
        public readonly int ID;

        public Func<object, string> Format;
        public string Name;
        
        public StringNames StringName;

        protected internal CustomOption(int id, string name, CustomOptionType type, object defaultValue,
            Func<object, string> format = null)
        {
            ID = id;
            Name = name;
            Type = type;
            DefaultValue = Value = defaultValue;
            Format = format ?? (obj => $"{obj}");

            AllOptions.Add(this);
            Set(Value);

            StringName = CustomStringName.CreateAndRegister(name);
        }

        protected internal object Value { get; set; }
        protected internal OptionBehaviour Setting { get; set; }
        protected internal CustomOptionType Type { get; set; }
        public object DefaultValue { get; set; }

        internal static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";

        public override string ToString()
        {
            return Format(Value);
        }

        public virtual void OptionCreated()
        {
            Setting.name = Setting.gameObject.name = Name;
        }


        protected internal void Set(object value, bool SendRpc = true)
        {
            Value = value;

            if (Setting != null && AmongUsClient.Instance.AmHost && SendRpc) Coroutines.Start(RpcUpdateSetting.SendRpc(this));

            try
            {
                if (Setting is ToggleOption toggle)
                {
                    var newValue = (bool) Value;
                    toggle.oldValue = newValue;
                    if (toggle.CheckMark != null) toggle.CheckMark.enabled = newValue;
                }
                else if (Setting is NumberOption number)
                {
                    var newValue = (float) Value;

                    number.Value = number.oldValue = newValue;
                    number.ValueText.text = ToString();
                }
                else if (Setting is StringOption str)
                {
                    var newValue = (int) Value;

                    str.Value = str.oldValue = newValue;
                    str.ValueText.text = ToString();
                }
            }
            catch
            {
            }

            if (DestroyableSingleton<HudManager>.InstanceExists && Type != CustomOptionType.Header)
            {
                DestroyableSingleton<HudManager>.Instance.Notifier.
                    AddSettingsChangeMessage(StringName, ToString(), DestroyableSingleton<HudManager>.Instance.Notifier.lastMessageKey != (int)StringName);
            }
        }
    }
}