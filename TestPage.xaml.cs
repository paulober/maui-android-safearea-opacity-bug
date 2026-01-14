using System.Diagnostics;

namespace MauiApp1;

public partial class TestPage : ContentPage
{
    private SafeAreaRegions _initialRegions;
    
    public TestPage(SafeAreaRegions regions)
    {
        _initialRegions = regions;
        InitializeComponent();

        // Set SafeAreaEdges FIRST
        Debug.WriteLine($"[SafeAreaTest] Setting ContentLayer.SafeAreaEdges to: {regions}");
        ContentLayer.SafeAreaEdges = new SafeAreaEdges(regions);
        
        // ============================================================================
        // BUG REPRODUCTION: Setting Opacity=0 BREAKS SafeAreaEdges on Android!
        // ============================================================================
        ContentLayer.Opacity = 0;
        ContentLayer.Scale = 0.8;  // ← THIS BREAKS SafeAreaEdges on Android! in combination with the later animation
        // ============================================================================
        //
        // TO FIX THE BUG: Comment out the two lines above or the animation in OnAppearing
        // or use WORKAROUND from bellow
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
        
        // WORKAROUND but the value must be different to the one initially set for ContentLayer!
        // What we set it to first is random but it must be different to the value that was set before
        //#if ANDROID
        //ContentLayer.SafeAreaEdges = new SafeAreaEdges(_initialRegions == SafeAreaRegions.Container
        //    ? SafeAreaRegions.SoftInput : SafeAreaRegions.Container); 
        //ContentLayer.SafeAreaEdges = new SafeAreaEdges(_initialRegions); 
        //#endif
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
