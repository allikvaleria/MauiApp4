using Microsoft.Maui.Controls;
using MauiApp4.Models;
using MauiApp4.View;

namespace FlyoutPageNavigation;

public class FlyoutMenuPage : ContentPage
{
    public event EventHandler<FlyoutClass>? MenuItemSelected;

    public CollectionView collectionView;

    public FlyoutMenuPage()
    {
        Title = "Isiklik korraldaja";
        Padding = new Thickness(0, 40, 0, 0);

        var items = new List<FlyoutClass>
        {
            new() { Title = "Kontaktid", TargetType = typeof(LounasookPage) },
            new() { Title = "Tegemiste nimekiri", TargetType = typeof(OhtusookPage) },
            new() { Title = "Meeldetuletused", TargetType = typeof(VahepalaPage) },
        };

        collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            ItemsSource = items,
            ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    Padding = new Thickness(5, 10),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = 30 },
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };

                var image = new Image();
                image.SetBinding(Image.SourceProperty, "IconSource");

                var label = new Label
                {
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(20, 0, 0, 0)
                };
                label.SetBinding(Label.TextProperty, "Title");

                grid.Add(image);
                grid.Add(label, 1, 0);

                return new ViewCell { View = grid };
            })
        };

        collectionView.SelectionChanged += (s, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is FlyoutClass selectedItem)
            {
                MenuItemSelected?.Invoke(this, selectedItem);
                collectionView.SelectedItem = null;
            }
        };

        Content = collectionView;
    }
}
