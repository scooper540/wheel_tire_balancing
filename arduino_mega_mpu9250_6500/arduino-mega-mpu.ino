#define SENSOR_WHITE  2
#define MPU6500_ADDR 0x68

uint8_t buff[16],buffTmp[16];

#include <Wire.h>



void setup() {
   pinMode(SENSOR_WHITE, INPUT);
   Serial.begin(2000000L);
   Wire.setClock(400000L);
   Wire.begin(); 
   Wire.setClock(400000L);
   // Réveil du MPU (il démarre en mode sleep)
  Wire.beginTransmission(MPU6500_ADDR);
  Wire.write(0x6B);         // PWR_MGMT_1
  Wire.write(0x00);         // Wake up device, use internal 8 MHz clock
  Wire.endTransmission();

  delay(100); // Attendre la stabilisation

  // 1. CONFIG : activer DLPF à ~100 Hz
  Wire.beginTransmission(MPU6500_ADDR);
  Wire.write(0x1A);         // CONFIG
  Wire.write(0x03);         // DLPF_CFG = 3
  Wire.endTransmission();

  // 2. SMPLRT_DIV : pour obtenir 200 Hz
  Wire.beginTransmission(MPU6500_ADDR);
  Wire.write(0x19);         // SMPLRT_DIV
  Wire.write(4);            // (1000 / (1 + 4)) = 200 Hz
  Wire.endTransmission();

  // 3. GYRO_CONFIG : plage ±250 dps (bits [4:3] = 00)
  Wire.beginTransmission(MPU6500_ADDR);
  Wire.write(0x1B);         // GYRO_CONFIG
  Wire.write(0x00);         // ±250 dps
  Wire.endTransmission();

  // 4. ACCEL_CONFIG : plage ±2g (bits [4:3] = 00)
  Wire.beginTransmission(MPU6500_ADDR);
  Wire.write(0x1C);         // ACCEL_CONFIG
  Wire.write(0x00);         // ±2g
  Wire.endTransmission();

  // 5. ACCEL_CONFIG2 : DLPF accel à ~99 Hz (ACCEL_FCHOICE_B = 0, A_DLPF_CFG = 3)
  Wire.beginTransmission(MPU6500_ADDR);
  Wire.write(0x1D);         // ACCEL_CONFIG2
  Wire.write(0x03);         // DLPF à ~99 Hz
  Wire.endTransmission();
}

void loop() 
{
    printData();
}

int printData()
{

    // Read output registers:
    // 58 data ready
    // [59-64] Accelerometer
    // [65-66] Temperature
    // [67-72] Gyroscope
    uint8_t buff[16],buffTmp[16];
    I2Cread(MPU9250_ADDR, 58, 15, buffTmp);
    if(!(buffTmp[0]& 0x01)) return 0; //not ready
    // Accelerometer, create 16 bits values from 8 bits data
    uint16_t ms = (micros() >> 6) & 0xFFFF; // resolution 0.064 ms -> 1 = 0.064ms
    buff[0] = 0xFE | ((PINE & B00010000) > 0 ? 1:0); //porte pin 4 for D2 on mega
    *(uint16_t*)&buff[1] = ms; //fast write timestamp value on buff1 and buff2
    //move accel data and Gyro after timestamp
    for(int i = 0; i < 6;i++)
    {
      buff[3+i] = buffTmp[1+i];
      buff[9+i] = buffTmp[9+i];
    }
    buff[15] = 0x0A;
    Serial.write(buff,sizeof(buff));
    //delayMicroseconds(500);
}

void I2Cread(uint8_t address, uint8_t reg, uint8_t bytes, uint8_t* data)
{
  Wire.beginTransmission(address); // Set register address
  Wire.write(reg);
  Wire.endTransmission();

  Wire.requestFrom(address, bytes); //Read bytes amount
  
  uint8_t index = 0;
  while (Wire.available()) {
    data[index++] = Wire.read();
  }
}