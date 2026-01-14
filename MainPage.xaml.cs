namespace MauiApp1;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnTestNoneClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TestPage(SafeAreaRegions.None), false);
    }

    private async void OnTestSoftInputClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TestPage(SafeAreaRegions.SoftInput), false);
    }

    private async void OnTestContainerClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TestPage(SafeAreaRegions.Container), false);
    }

    private async void OnTestContainerSoftInputClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TestPage(SafeAreaRegions.Container | SafeAreaRegions.SoftInput), false);
    }

    private async void OnTestAllClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TestPage(SafeAreaRegions.All), false);
    }
}
