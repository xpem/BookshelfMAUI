namespace Bookshelf.Components;

public partial class BorderedEditor : VerticalStackLayout
{
    public BorderedEditor()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
    propertyName: nameof(Text), returnType: typeof(string), declaringType: typeof(BorderedEntry), defaultValue: null, defaultBindingMode: BindingMode.TwoWay);

    public string Text { get => (string)GetValue(TextProperty); set { SetValue(TextProperty, value); } }

    public static readonly BindableProperty EditorIsVisibleProperty = BindableProperty.Create(
    propertyName: nameof(EditorIsVisible), returnType: typeof(bool), declaringType: typeof(BorderedEntry), defaultValue: false, defaultBindingMode: BindingMode.OneWay);

    public bool EditorIsVisible { get => (bool)GetValue(EditorIsVisibleProperty); set { SetValue(EditorIsVisibleProperty, value); } }

    public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(
   propertyName: nameof(LabelText), returnType: typeof(string), declaringType: typeof(BorderedEntry), defaultValue: null, defaultBindingMode: BindingMode.OneWay);

    public string LabelText { get => (string)GetValue(LabelTextProperty); set { SetValue(LabelTextProperty, value); } }

}