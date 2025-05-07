using MauiApp4.View;

namespace MauiApp4;

public partial class MainPage : ContentPage
{
    public List<ContentPage> lehed = new List<ContentPage>() { new VeejalgiminePage(), new LounasookPage(), new OhtusookPage(), new VahepalaPage() };
    public List<string> tekstid = new List<string> { "VeejalgiminePage", "LounasookPage", "OhtusookPage", "VahepalaPage" };
    ScrollView sv;
    VerticalStackLayout vsl;
    public MainPage()
    {
        Title = "Avaleht";
        vsl = new VerticalStackLayout { BackgroundColor = Color.FromArgb("#FFC0CB") };
        for (int i = 0; i < tekstid.Count; i++)
        {
            Button nupp = new Button
            {
                Text = tekstid[i],
                BackgroundColor = Color.FromArgb("#EE82EE"),
                TextColor = Color.FromArgb("#FF00FF"),
                BorderWidth = 10,
                ZIndex = i,
                FontFamily = "Luckymoon 400",
                FontSize = 28
            };
            vsl.Add(nupp);
            nupp.Clicked += Lehte_avamine;
        }
        sv = new ScrollView { Content = vsl };
        Content = sv;

    }

    private async void Lehte_avamine(object? sender, EventArgs e)
    {
        Button btn = (Button)sender;
        await Navigation.PushAsync(lehed[btn.ZIndex]);
    }

    private async void Tagasi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
}