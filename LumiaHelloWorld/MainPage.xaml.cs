using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LumiaHelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int tapCount = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void HelloButton_Click(object sender, RoutedEventArgs e)
        {
            tapCount++;
            HelloText.Text = string.Format("Hello from your Lumia 950 XL! (tapped {0} time(s))", tapCount);
        }
    }
}
