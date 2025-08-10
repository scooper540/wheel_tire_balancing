using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace equilibreuse
{
    class Help
    {
        public static void FillHelp(RichTextBox richTextBox)
        {
            richTextBox.Text = String.Empty;
            richTextBox.ReadOnly = true;
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.Font = new Font("Segoe UI", 10);

            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            richTextBox.AppendText("🛠️ How to Use the Wheel Balancing Software\n");
            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);

            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            richTextBox.AppendText("🔌 Connecting the Sensor\n");
            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            richTextBox.AppendText("1. Select the appropriate COM port and click Connect.\n");
            richTextBox.AppendText("2. Wait a few seconds.\n");
            richTextBox.AppendText("3. Check that the number of samples captured in 5 seconds matches your sensor’s expected output:\n");
            richTextBox.AppendText("   - About 5000 samples for MPU9250/MPU6500\n");
            richTextBox.AppendText("   - About 11000 samples for LSM6DS3\n\n");

            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            richTextBox.AppendText("⚙️ Data Capture\n");
            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            richTextBox.AppendText("1. Place a white line on the rim of the tire and verify the sensor see it. This is the 0°. Spin the wheel and select a target RPM range, typically between 200 and 230.\n");
            richTextBox.AppendText("2. Click Start Capture once the wheel reaches the desired RPM (e.g., right after the motor is disengaged).\n");
            richTextBox.AppendText("3. Click End Capture when finished.\n");
            richTextBox.AppendText("   - A CSV file is automatically generated.\n\n");

            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            richTextBox.AppendText("📊 Data Analysis\n");
            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            richTextBox.AppendText("1. Click Analyze CSV to load a captured file.\n");
            richTextBox.AppendText("2. Use Selection Selector to choose the wheel turns to be analyzed.\n");
            richTextBox.AppendText("Display Modes:\n");
            richTextBox.AppendText(" - Compiled: Superimposes all selected turns on a single 360° angular signal.\n");
            richTextBox.AppendText(" - Single: Displays each turn individually.\n");
            richTextBox.AppendText(" - Global: Displays all turns sequentially.\n");
            richTextBox.AppendText("   - If Order Tracking Interpolate is enabled, all turns are resampled to the same number of points for consistent FFT analysis.\n");
            richTextBox.AppendText("Additional Views:\n");
            richTextBox.AppendText(" - Gyro: Displays gyroscope data.\n");
            richTextBox.AppendText(" - Analysis:\n");
            richTextBox.AppendText("   - Text: Provides summary analysis of compiled, global, turn-by-turn, and gyro data.\n");
            richTextBox.AppendText("   - Graphical: Shows where to place balancing weights based on the first five harmonics.\n");
            richTextBox.AppendText("   - Temporal: Displays statistics on peak vibration amplitudes and their angular positions.\n\n");

            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            richTextBox.AppendText("🧭 How to Balance a Wheel\n");
            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            richTextBox.AppendText("The goal is to reduce the amplitude of the first few harmonics on the X and Y axes to correct static and dynamic imbalance.\n");
            richTextBox.AppendText("Types of Imbalance:\n");
            richTextBox.AppendText(" - Static imbalance: Uneven mass distribution on a single axis (X or Y).\n");
            richTextBox.AppendText(" - Dynamic imbalance: Uneven mass on both axes; occurs when X and Y angles at the fundamental frequency are approximately 90° apart.\n");
            richTextBox.AppendText("Step-by-Step Balancing:\n");
            richTextBox.AppendText("1. Place a white line on the rim of the tire and verify the sensor see it. This is the 0°. After capturing data between 200 and 230 RPM, check if dynamic imbalance is detected (e.g., in more than 25% of turns).\n");
            richTextBox.AppendText("2. Place balancing weights on the inner and outer sides of the wheel based on the suggested angles.\n");
            richTextBox.AppendText("3. Repeat the process: capture new data and reanalyze to see if the number of turns with imbalance decreases.\n");
            richTextBox.AppendText("4. If the X or Y angle corresponds to the valve position, place a weight 180° opposite that position (especially for the first correction).\n");
            richTextBox.AppendText("5. Reduce the amplitude of the fundamental frequency until it is:\n");
            richTextBox.AppendText("   - Below 2 to 2.5 times the baseline curve (e.g., in Compiled and Turn-by-turn views).\n");
            richTextBox.AppendText("   - Note: The Global view should not be used for this, as the number of turns may vary between captures.\n");
            richTextBox.AppendText("A flat FFT curve on the fundamental and early harmonics means your wheel is properly balanced ✅\n\n");

            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            richTextBox.AppendText("⚙️ Advanced Analysis Options\n");
            richTextBox.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            richTextBox.AppendText("Resultant: Calculates √(X² + Y²)\n");
            richTextBox.AppendText("FFT: Frequency analysis (Hann window by default)\n");
            richTextBox.AppendText("SampleRate: Captured sampling rate, used for FFT calculations\n");
            richTextBox.AppendText("Lowpass filter: Applies a low-pass filter (value defined in textbox)\n");
            richTextBox.AppendText("ZeroPhase: Applies the filter in forward and reverse (zero phase shift)\n");
            richTextBox.AppendText("Limit FFT: Limits FFT display to the first X Hz\n");
            richTextBox.AppendText("Absolute values: Converts all CSV data to absolute values\n");
            richTextBox.AppendText("Sum: Sums the values instead of averaging them\n");
            richTextBox.AppendText("RemoveDC: Removes DC offset from each wheel turn\n");
            richTextBox.AppendText("Order Tracking Interpolate: Resamples selected turns to the same number of points (improves FFT accuracy)\n");
        }
    }
}
