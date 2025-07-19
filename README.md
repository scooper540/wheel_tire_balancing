# wheel_tire_balancing
This DIY project was created to help me balance my car tires at home after mounting them, without needing to go to a garage.

How I did it:
- I bought a used car rear hub (moyeu arrière in french)
- I mounted it securely on a wooden base, which I then fixed to my garage workbench.
- I attached the accelerometer to the hub, with:
      The X-axis perpendicular to the wheel,
      The Y-axis aligned with the wheel’s rotation axis.
- I put a white line on the rim of the tire and spin the wheel using an electric motor that drives a belt wrapped around the wheel, rotating it as if the car were moving forward.

I'm using an Arduino Mega (should work with other models) with an accelerometer (MPU9250/6500 or LSM6DS3) and a white line sensor (for example TCRT5000).
Sensor is connected on Mega on D2
Accelerometer is connected with i2c to SDA/SCL
Both powered with 3.3V from Arduino Mega.
Connection to the PC with USB (Serial) baudrate 2Mbit/s

Both ino files are provided to read the sensors at max speed (MPU has a sampling rate of around 1Khz, LSM6DS3 is faster)

----
🛠️ How to Use the Wheel Balancing Software
🔌 Connecting the Sensor
1. Select the appropriate COM port and click Connect.
2. Wait a few seconds.
3. Check that the number of samples captured in 5 seconds matches your sensor’s expected output:
   - About 5000 samples for MPU9250/MPU6500
   - About 11000 samples for LSM6DS3

⚙️ Data Capture
1. Place a white line on the rim of the tire and verify the sensor see it. This is the 0°. Spin the wheel and select a target RPM range, typically between 200 and 230.
2. Click Start Capture once the wheel reaches the desired RPM (e.g., right after the motor is disengaged).
3. Click End Capture when finished.
   - A CSV file is automatically generated.

📊 Data Analysis
1. Click Analyze CSV to load a captured file.
2. Use Selection Selector to choose the wheel turns to be analyzed.
Display Modes:
 - Compiled: Superimposes all selected turns on a single 360° angular signal.
 - Single: Displays each turn individually.
 - Global: Displays all turns sequentially.
   - If Order Tracking Interpolate is enabled, all turns are resampled to the same number of points for consistent FFT analysis.
Additional Views:
 - Gyro: Displays gyroscope data.
 - Analysis:
   - Text: Provides summary analysis of compiled, global, turn-by-turn, and gyro data.
   - Graphical: Shows where to place balancing weights based on the first five harmonics.
   - Temporal: Displays statistics on peak vibration amplitudes and their angular positions.

🧭 How to Balance a Wheel
The goal is to reduce the amplitude of the first few harmonics on the X and Y axes to correct static and dynamic imbalance.
Types of Imbalance:
 - Static imbalance: Uneven mass distribution on a single axis (X or Y).
 - Dynamic imbalance: Uneven mass on both axes; occurs when X and Y angles at the fundamental frequency are approximately 90° apart.
Step-by-Step Balancing:
1. Place a white line on the rim of the tire and verify the sensor see it. This is the 0°. After capturing data between 200 and 230 RPM, check if dynamic imbalance is detected (e.g., in more than 25% of turns).
2. Place balancing weights on the inner and outer sides of the wheel based on the suggested angles.
3. Repeat the process: capture new data and reanalyze to see if the number of turns with imbalance decreases.
4. If the X or Y angle corresponds to the valve position, place a weight 180° opposite that position (especially for the first correction).
5. Reduce the amplitude of the fundamental frequency until it is:
   - Below 2 to 2.5 times the baseline curve (e.g., in Compiled and Turn-by-turn views).
   - Note: The Global view should not be used for this, as the number of turns may vary between captures.
A flat FFT curve on the fundamental and early harmonics means your wheel is properly balanced ✅

⚙️ Advanced Analysis Options
Resultant: Calculates √(X² + Y²)
FFT: Frequency analysis (Hann window by default)
SampleRate: Captured sampling rate, used for FFT calculations
Lowpass filter: Applies a low-pass filter (value defined in textbox)
ZeroPhase: Applies the filter in forward and reverse (zero phase shift)
Limit FFT: Limits FFT display to the first X Hz
Absolute values: Converts all CSV data to absolute values
Sum: Sums the values instead of averaging them
RemoveDC: Removes DC offset from each wheel turn
Order Tracking Interpolate: Resamples selected turns to the same number of points (improves FFT accuracy)



