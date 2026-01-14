using System.Diagnostics;

namespace MauiApp1;

public partial class TestPage : ContentPage
{
    public TestPage(SafeAreaRegions regions)
    {
        InitializeComponent();

        // Set SafeAreaEdges FIRST
        Debug.WriteLine($"[SafeAreaTest] Setting ContentLayer.SafeAreaEdges to: {regions}");
        ContentLayer.SafeAreaEdges = new SafeAreaEdges(regions);
        
        // ============================================================================
        // BUG REPRODUCTION: Setting Opacity=0 BREAKS SafeAreaEdges on Android!
        // ============================================================================
        ContentLayer.Opacity = 0;  // ← THIS BREAKS SafeAreaEdges on Android!
        ContentLayer.Scale = 0.8;
        // ============================================================================
        //
        // TO FIX THE BUG: Comment out the two lines above
        // ============================================================================
        
        // Update labels
        TitleLabel.Text = $"SafeAreaRegions: {regions}";
        DescriptionLabel.Text = GetDescription(regions);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        Debug.WriteLine($"[SafeAreaTest] ===== OnAppearing =====");
        Debug.WriteLine($"[SafeAreaTest] ContentLayer.SafeAreaEdges: {ContentLayer.SafeAreaEdges}");
        Debug.WriteLine($"[SafeAreaTest] ContentLayer Bounds BEFORE: X={ContentLayer.X:F2}, Y={ContentLayer.Y:F2}, W={ContentLayer.Width:F2}, H={ContentLayer.Height:F2}");
        Debug.WriteLine($"[SafeAreaTest] ContentLayer Padding: {ContentLayer.Padding}");
        Debug.WriteLine($"[SafeAreaTest] Platform: {DeviceInfo.Platform}");
        
        // Wait for layout
        await Task.Delay(200);
        
        Debug.WriteLine($"[SafeAreaTest] ContentLayer Bounds: X={ContentLayer.X:F2}, Y={ContentLayer.Y:F2}, W={ContentLayer.Width:F2}, H={ContentLayer.Height:F2}");
        
        // Animate in (to show the bug visually)
        await ContentLayer.FadeToAsync(1, 200);
        await ContentLayer.ScaleToAsync(1, 200);
    }

    private string GetDescription(SafeAreaRegions regions)
    {
        return regions switch
        {
            SafeAreaRegions.None => "Should go edge-to-edge (UNDER notch)",
            SafeAreaRegions.SoftInput => "Should avoid keyboard only (UNDER notch)",
            SafeAreaRegions.Container => "Should avoid notch and bars (BELOW notch)",
            SafeAreaRegions.All => "Should avoid everything (BELOW notch)",
            _ when (regions & SafeAreaRegions.Container) != 0 && (regions & SafeAreaRegions.SoftInput) != 0 
                => "Container | SoftInput (BELOW notch, above keyboard)",
            _ => $"Custom: {regions}"
        };
    }

    private async void OnBackdropTapped(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(false);
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(false);
    }
}
