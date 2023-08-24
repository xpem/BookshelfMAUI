namespace Bookshelf.Components;

public partial class BorderedNumericEntry : VerticalStackLayout
{
    public BorderedNumericEntry()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        propertyName: nameof(Text), returnType: typeof(string),
        declaringType: typeof(BorderedNumericEntry),
        defaultValue: null, defaultBindingMode: BindingMode.TwoWay);

    public string Text { get => (string)GetValue(TextProperty); set { SetValue(TextProperty, value); } }

    public static readonly BindableProperty LabelTextProp = BindableProperty.Create(
       propertyName: nameof(LabelText), returnType: typeof(string),
       declaringType: typeof(BorderedNumericEntry), defaultValue: null, defaultBindingMode: BindingMode.OneWay);

    public string LabelText { get => (string)GetValue(LabelTextProp); set { SetValue(LabelTextProp, value); } }

    public static readonly BindableProperty MaxLengthProp = BindableProperty.Create(
      propertyName: nameof(MaxLength), returnType: typeof(int),
      declaringType: typeof(BorderedNumericEntry), defaultValue: 4, defaultBindingMode: BindingMode.OneWay);

    public int MaxLength { get => (int)GetValue(MaxLengthProp); set { SetValue(MaxLengthProp, value); } }

    //public static readonly BindableProperty TextTransformProperty = BindableProperty.Create(
    //  propertyName: nameof(TextTransformValue), returnType: typeof(TextTransform), declaringType: typeof(BorderedEntry), defaultValue: TextTransform.Default, defaultBindingMode: BindingMode.OneWay);

    //public TextTransform TextTransformValue { get => (TextTransform)GetValue(TextTransformProperty); set { SetValue(TextTransformProperty, value); } }

}