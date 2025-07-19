#define SENSOR_WHITE  2

//use library MPU9250_WE https://docs.arduino.cc/libraries/mpu9250_we/

uint8_t buff[16],buffTmp[16];
#include <MPU9250_WE.h>
#include <Wire.h>
#define MPU9250_ADDR 0x68
MPU9250_WE myMPU9250 = MPU9250_WE(MPU9250_ADDR);

void setup() {
   pinMode(SENSOR_WHITE, INPUT);
   Serial.begin(2000000L);
   Wire.setClock(400000L);
   Wire.begin(); 
   Wire.setClock(400000L);
   if(myMPU9250.init())
   {   
      myMPU9250.autoOffsets();
      myMPU9250.setAccRange(MPU9250_ACC_RANGE_2G);
      myMPU9250.setGyrRange(MPU9250_GYRO_RANGE_250);
      myMPU9250.enableAccDLPF(false);
      myMPU9250.disableGyrDLPF(MPU9250_BW_WO_DLPF_3600);
      
      myMPU9250.disableInterrupt(MPU9250_DATA_READY); 
      myMPU9250.disableInterrupt(MPU9250_WOM_INT); 
      myMPU9250.disableInterrupt(MPU9250_FIFO_OVF); 
      myMPU9250.enableAccAxes(MPU9250_ENABLE_XYZ);
      myMPU9250.enableGyrAxes(MPU9250_ENABLE_XYZ);
  }
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
