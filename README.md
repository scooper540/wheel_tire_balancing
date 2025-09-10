# 🛞 DIY Dynamic Wheel & Tire Balancing  
# 🛞 Équilibrage Dynamique DIY de Roues et Pneus

This open-source project allows you to **balance your car wheels at home** after mounting tires, without needing professional garage equipment.  
Ce projet open-source permet **d'équilibrer ses roues de voiture chez soi**, après le montage des pneus, sans avoir besoin d’un équipement professionnel.

---

## 🏗️ How I Built It  
## 🏗️ Construction du Dispositif

- I bought a **used car rear hub**  
- J’ai acheté un **moyeu arrière de voiture d’occasion**

- Mounted it securely on a **wooden base**, fixed to my garage workbench  
- Je l’ai fixé solidement sur une **base en bois**, elle-même fixée à mon établi

- Attached the **accelerometer** in the center of the hub, with:  
  - **X-axis** aligned with the wheel’s rotation axis  
  - **Y-axis** perpendicular to the wheel  
- J’ai installé l’**accéléromètre** au centre du moyeu, avec :  
  - **L’axe X** aligné avec l’axe de rotation de la roue  
  - **L’axe Y** perpendiculaire à la roue  

- Placed a **white line** on the rim of the tire  
- J’ai mis une **ligne blanche** sur la jante

- Used an **electric motor** with a belt to spin the wheel  
- Utilisé un **moteur électrique** et une courroie pour faire tourner la roue

---

## 🔌 Hardware & Connections  
## 🔌 Matériel & Connexions

- **Arduino Mega** (other models should work)  
- **Arduino Mega** (d’autres modèles devraient fonctionner)

- **Accelerometer**: MPU9250 / MPU6500 / LSM6DS3  
- **Accéléromètre** : MPU9250 / MPU6500 / LSM6DS3

- **White line sensor**: TCRT5000  
- **Capteur de ligne blanche** : TCRT5000

- Sensor on **Digital Pin D2**, accelerometer via **I2C (SDA/SCL)**  
- Capteur sur la **broche D2**, accéléromètre en **I2C (SDA/SCL)**

- Both powered by **3.3V** from Arduino  
- Alimentation des capteurs via le **3,3V** de l’Arduino

- PC connection via USB at **2 Mbit/s**  
- Connexion au PC par USB à **2 Mbit/s**

---

## 💾 Firmware & Software  
## 💾 Code et Logiciel

- Two `.ino` files included for high-speed sampling  
- Deux fichiers `.ino` sont fournis pour la lecture à haute fréquence :

  - ~5000 Hz for MPU9250/6500  
  - ~11,000 Hz for LSM6DS3

- C# software built with **Visual Studio 2017 Community**  
- Logiciel C# développé avec **Visual Studio 2017 Community**

- NuGet libraries used, compatible with **Windows 7+**  
- Bibliothèques NuGet utilisées, compatible avec **Windows 7 et plus**

---

## 🛠️ How to Use the Wheel Balancing Software  
## 🛠️ Utilisation du logiciel d'équilibrage

### 🔌 Connecting the Sensor  
### 🔌 Connexion des capteurs

1. Select correct **COM port**, click **Connect**  
2. Wait a few seconds  
3. Verify sampling rate:
   - ~5000 samples (MPU9250/6500)
   - ~11000 samples (LSM6DS3)

---

1. Sélectionner le **port COM**, cliquer sur **Connect**  
2. Attendre quelques secondes  
3. Vérifier le débit :
   - ~5000 échantillons (MPU9250/6500)
   - ~11000 échantillons (LSM6DS3)

---

### ⚙️ Data Capture  
### ⚙️ Capture des données

1. Mark the rim at **0°** with a white line  
2. Spin wheel > **250 RPM**  
3. **Remove the belt from the wheel** so it spins freely, with no friction from the motor or transmission.
4. Press **Start Capture** while wheel slows down  
5. Press **End Capture**  
   - CSV file auto-generated  

---

1. Mettre une ligne blanche à **0°**  
2. Faire tourner la roue à plus de **250 RPM**  
3. **Retirer la courroie de la roue** afin qu’elle tourne librement, sans aucun frottement du moteur ou de la transmission.
4. Appuyer sur **Start Capture** pendant que la roue ralentit  
5. Appuyer sur **End Capture**  
   - Un fichier CSV est généré automatiquement  

---

### 📊 Data Analysis  
### 📊 Analyse des données

1. Click **Analyze CSV** to load data  
2. In **Selection Selector**, click **Complete Analysis**  
3. Shows:
   - Average **imbalance angle** (X & Y)
   - Magnitude
   - **Correction angles** to apply weights
4. If X & Y angles < 5° apart → place weight at angle +180°  
5. Repeat until vibration magnitude decreases

---

1. Cliquer sur **Analyze CSV**  
2. Dans **Selection Selector**, cliquer sur **Complete Analysis**  
3. Affiche :
   - Angle moyen du **balourd** (X & Y)
   - Amplitude
   - **Angles de correction** où placer les masses  
4. Si X et Y < 5° → placer la masse à **+180°**  
5. Répéter jusqu’à réduire l’amplitude

---

## 🧭 How to Balance a Wheel  
## 🧭 Comment équilibrer une roue

### Goal / Objectif

- Reduce the **fundamental harmonic** on X & Y using a **passband filter**  
- Réduire l’**harmonique fondamentale** sur X et Y avec un **filtre passe-bande**

### Types of Imbalance / Types de balourd

- **Static imbalance**: single-axis mass offset  
- **Balourd statique** : déséquilibre sur un seul axe  
- **Dynamic imbalance**: both axes (X and Y)  
- **Balourd dynamique** : déséquilibre sur les deux axes  

### Steps / Étapes

1. White line at 0°, spin to 200–250 RPM  
2. Capture & analyze  
3. Apply weights based on results  
4. Re-test to confirm improvement

---

1. Ligne blanche à 0°, tourner à 200–250 RPM  
2. Capturer et analyser  
3. Placer les masses selon les angles  
4. Tester à nouveau pour valider

---

## 🧭 Calibration for Mass Estimation  
## 🧭 Calibration pour estimer la masse

1. Ensure sensor is **centered** on the bearing  
2. On a balanced wheel, add a known weight (e.g., 100g) at 0°, 90°, 180°, 270°  
3. Check if angle is correctly detected  
4. Adjust static offset if needed  
5. Enter known mass and observed magnitude in calibration panel

---

1. Capteur bien **centré** sur le roulement  
2. Mettre une masse connue (ex: 100g) à 0°, 90°, 180°, 270°  
3. Vérifier que l’angle est bien détecté  
4. Corriger l’offset statique si besoin  
5. Renseigner masse et magnitude observée dans le panneau de calibration

---

## ⚙️ Analysis Settings  
## ⚙️ Réglages d'analyse

- **Angle**: Best → Global FFT or Global Lock-in  
  - Check **Clockwise** if wheel spins clockwise  
- **Magnitude**: Several modes  
- **Filter**: Butterworth recommended  
  - Secondary filter (smoothing, IQ, etc.)

---

- **Angle** : Recommandé → Global FFT ou Lock-in  
  - Cocher **Clockwise** si roue dans le sens horaire  
- **Magnitude** : Plusieurs modes  
- **Filtre** : Butterworth conseillé  
  - Filtre secondaire disponible (lissage, IQ…)

---

## ✅ Recommended Settings  
## ✅ Paramètres recommandés

| Setting               | Value         | Valeur      |
|-----------------------|---------------|-------------|
| Filter                | Butterworth   | Butterworth |
| RemoveDC              | Enabled       | Activé      |
| Gain                  | 10000         | 10000       |
| FFT Window            | BlackmanNuttal| Idem        |
| Angle Detection       | Global Lock-in| Idem        |
| Magnitude             | Global / # turns | Idem    |
| X Offset              | 170           | 170         |
| Y Offset              | 260           | 260         |

> ⚠️ Offsets depend on your setup  
> ⚠️ Les offsets dépendent de la position du capteur

---

## 🗺️ Legend / Légende

- **Compiled**: Overlay of selected turns  
- **Compiled** : Superposition des tours sélectionnés

- **Single**: One turn per graph  
- **Single** : Un tour par graphe

- **Global**: All turns sequentially  
- **Global** : Tous les tours en séquence

- **Gyro**: Shows gyroscope  
- **Gyro** : Affiche le gyroscope

- **Graphical Analysis**: Suggests mass placement  
- **Graphique** : Suggestion de placement des masses

---

## ⚙️ Options Overview / Vue d'ensemble des options

| Option                    | Description 🇬🇧 / 🇫🇷                              |
|---------------------------|-------------------------------------------------|
| Resultant                 | √(X² + Y²)                                       |
| FFT                       | Frequency analysis (BlackmanNuttal window)      |
| SampleRate                | Informational sampling rate                     |
| Lowpass / Passband Filter | Apply filter on raw signal                      |
| Limit FFT                 | Limit FFT range                                 |
| Absolute Values           | Convert CSV data to absolute values             |
| Sum                       | Sum instead of averaging                        |
| RemoveDC                  | Remove DC offset from turns                     |
| dB                        | Display FFT in dB                               |
| Order Tracking Interpolate| Resample for better FFT                         |
| Gain                      | Visual multiplier                               |
| Clockwise Rotating        | Enable if wheel spins clockwise                 |

---

## 📬 Contributions Welcome  
## 📬 Contributions Bienvenues

This is a personal DIY project. Feel free to suggest improvements or contribute!  
C’est un projet DIY personnel. N’hésitez pas à proposer des améliorations ou contribuer !

