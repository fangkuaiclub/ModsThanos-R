namespace ModsThanos.CustomOption
{
    public class CustomHeaderOption : CustomOption
    {
        protected internal CustomHeaderOption(string name) : base(name, CustomOptionType.Header, 0)
        {
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
        }
    }
}