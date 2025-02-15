using System.Collections;
using System.Windows.Input;

namespace Bookshelf.Utils.Behaviors
{
    /// <summary>
    /// <see cref="https://www.sandeshkarki.com/infinite-scrolling-in-xamarin-forms/"/>
    /// </summary>
    public partial class InfiniteScrollBehavior : Behavior<ListView>
    {
        public static readonly BindableProperty LoadMoreCommandProperty =
                               BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(InfiniteScrollBehavior), null);

        public ICommand LoadMoreCommand
        {
            get
            {
                return (ICommand)GetValue(LoadMoreCommandProperty);
            }
            set
            {
                SetValue(LoadMoreCommandProperty, value);
            }
        }

        public ListView AssociatedObject
        {
            get;
            private set;
        }

        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += Bindable_BindingContextChanged;

            bindable.ItemAppearing += InfiniteListView_ItemAppearing;
        }

        private void Bindable_BindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }

        protected override void OnDetachingFrom(ListView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= Bindable_BindingContextChanged;
            bindable.ItemAppearing -= InfiniteListView_ItemAppearing;
        }

        void InfiniteListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            IList items = AssociatedObject.ItemsSource as IList;

            if (items != null)
            {
                if (items.Count > 0)
                {
                    int index = items.Count - 1;
                    if (e.Item == items[index])
                    {
                        if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null)) LoadMoreCommand.Execute(null);
                    }
                }
            }
        }
    }
}
