namespace Bookshelf.Components;

public partial class BorderedEntry : VerticalStackLayout
{
    public BorderedEntry()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        propertyName: nameof(Text), returnType: typeof(string), declaringType: typeof(BorderedEntry), defaultValue: null, defaultBindingMode: BindingMode.TwoWay);

    public string Text { get => (string)GetValue(TextProperty); set { SetValue(TextProperty, value); } }

    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(
       propertyName: nameof(LabelText), returnType: typeof(string), declaringType: typeof(BorderedEntry), defaultValue: null, defaultBindingMode: BindingMode.OneWay);

    public string LabelText { get => (string)GetValue(LabelTextProperty); set { SetValue(LabelTextProperty, value); } }

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        propertyName: nameof(IsPassword), returnType: typeof(bool), declaringType: typeof(BorderedEntry), defaultValue: false, defaultBindingMode: BindingMode.OneWay);

    public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set { SetValue(IsPasswordProperty, value); } }

    public static readonly BindableProperty EnabledProperty = BindableProperty.Create(
      propertyName: nameof(Enabled), returnType: typeof(bool), declaringType: typeof(BorderedEntry), defaultValue: true, defaultBindingMode: BindingMode.OneWay);

    public bool Enabled { get => (bool)GetValue(EnabledProperty); set { SetValue(EnabledProperty, value); } }
}