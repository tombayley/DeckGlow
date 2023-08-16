using System.Windows;
using System.Windows.Controls;

namespace DeckGlow.Views.Controls
{
    public partial class BrightnessSlider : UserControl
    {

        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(int), typeof(BrightnessSlider), null);

        public int Brightness
        {
            get { return (int)GetValue(BrightnessProperty); }
            set
            {
                value = ValidateBrightness(value);
                SetValue(BrightnessProperty, value);
            }
        }

        public BrightnessSlider()
        {
            InitializeComponent();
        }

        private void BrightnessPlus_Click(object sender, RoutedEventArgs e)
        {
            Brightness++;
        }

        private void BrightnessMinus_Click(object sender, RoutedEventArgs e)
        {
            Brightness--;
        }

        private void Brightness_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void BrightnessText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;
            if (string.IsNullOrEmpty(textBox.Text)) return;

            if (!int.TryParse(textBox.Text, out int brightness)) brightness = 100;

            textBox.Text = ValidateBrightness(brightness).ToString();
        }

        private static int ValidateBrightness(int brightness)
        {
            if (brightness < 0) return 0;
            else if (brightness > 100) return 100;
            return brightness;
        }

    }
}
