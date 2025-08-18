namespace ModsThanos.CustomOption
{
    public class CustomToggleOption : CustomOption
    {
        protected internal CustomToggleOption(int id, string name, bool value = true) : base(id, name,
            CustomOptionType.Toggle,
            value)
        {
            Format = val => (bool)val ? Utility.Utils.ColorString(Palette.EnabledColor, "optionOn") : Utility.Utils.ColorString(Palette.DisabledGrey, "optionOff");
        }

        protected internal bool Get()
        {
            return (bool)Value;
        }

        protected internal void Toggle()
        {
            Set(!Get());
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var tgl = Setting.Cast<ToggleOption>();
            tgl.CheckMark.enabled = Get();
        }
    }
}