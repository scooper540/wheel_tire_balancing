#define SENSOR_WHITE  2
#define LSM6DS3_ADDR 0x6B 
uint8_t buff[16];
#include <Wire.h>

void setup() {
    pinMode(SENSOR_WHITE, INPUT);
    Serial.begin(2000000L);
    memset(buff,0,sizeof(buff));
    Wire.setClock(1000000L);
    Wire.begin(); 
    Wire.setClock(1000000L);
    delay(100);
 // 1. Accéléromètre : ODR = 200 Hz, ±4g, BW = 50 Hz (anti-aliasing analogique)
  // CTRL1_XL: 0x5B = 0b01011011
  Wire.beginTransmission(LSM6DS3_ADDR);
  Wire.write(0x10);       // CTRL1_XL
  Wire.write(0xBF);       // ODR = 1.66 kHz, ±8g, BW = 50 Hz
  Wire.endTransmission();

  // 2. Gyroscope : ODR = 200 Hz, ±245 dps
  // CTRL2_G: 0x50 = 0b01010000
  Wire.beginTransmission(LSM6DS3_ADDR);
  Wire.write(0x11);       // CTRL2_G
  Wire.write(0x50);       // 200 Hz, ±245 dps
  Wire.endTransmission();

  // 3. Désactiver le filtre numérique LPF2 sur accéléro
  Wire.beginTransmission(LSM6DS3_ADDR);
  Wire.write(0x17);       // CTRL8_XL
  Wire.write(0x00);       // LPF2 désactivé
  Wire.endTransmission();

  // 4. Désactiver le filtre LPF1 sur gyro
  Wire.beginTransmission(LSM6DS3_ADDR);
  Wire.write(0x16);       // CTRL7_G
  Wire.write(0x00);       // LP_EN_G = 0
  Wire.endTransmission();
    Wire.setClock(1000000L);
}
void loop() 
{
    printData();
}

int printData()
{
  //read accel and gyro data
  Wire.beginTransmission(0x6B); // Set register address
  Wire.write(0x22);
  Wire.endTransmission();
  Wire.requestFrom(0x6B, 12); //Read 12 bytes amount -> gyro + accelerometer
  int i = 0;
  for(i=0;i<6;i++)
    buff[9+i] = Wire.read(); // store gyro
  for(i=0;i<6;i++)
    buff[3+i] = Wire.read(); // store accel
  
   uint16_t ms = (micros() >> 6) & 0xFFFF; // resolution 0.064 ms -> 1 = 0.064ms
   buff[0] = 0xFE | ((PINE & B00010000) > 0 ? 1:0); //port pin 4 for D2 on mega for sensor white
   *(uint16_t*)&buff[1] = ms; //fast write timestamp value on buff1 and buff2
   buff[15] = 0x0A; // end of data
   Serial.write(buff,sizeof(buff));
}