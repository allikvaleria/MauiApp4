using Microsoft.Maui.Controls;
using MauiApp4.Models;


namespace FlyoutPageNavigation;

public class StartPage : FlyoutPage
{
    private FlyoutMenuPage flyoutPage;

    public StartPage()
    {
        flyoutPage = new FlyoutMenuPage();
        flyoutPage.MenuItemSelected += OnMenuItemSelected;

        Flyout = flyoutPage;
        Detail = new NavigationPage(new StartPage());
    }

    private void OnMenuItemSelected(object? sender, FlyoutClass selectedItem)
    {
        if (selectedItem?.TargetType != null)
        {
            Detail = new NavigationPage((Page)Activator.CreateInstance(selectedItem.TargetType)!);

            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }
    }
}
