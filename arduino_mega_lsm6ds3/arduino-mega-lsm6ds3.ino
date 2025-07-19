#define SENSOR_WHITE  2

//use library Amethyste_LSM6DS3 https://docs.arduino.cc/libraries/amethyste_lsm6ds3/
//modify in the library the address to 0x6B in LSM6DS3.h
//#define LSM6DS3_ID 0x6B 
//uint8_t LSM6DS3_address = 0x6B;

uint8_t buff[16];
#include <Wire.h>
#include <Amethyste_LSM6DS3.h>

// Instancie un capteur LSM6DS3
LSM6DS3 sensor;

void setup() {
    pinMode(SENSOR_WHITE, INPUT);
    Serial.begin(2000000L);
    memset(buff,0,sizeof(buff));
    Wire.setClock(1000000L);
    Wire.begin(); 
    Wire.setClock(1000000L);
    //Call .begin() to configure the IMU
    sensor.begin();
    sensor.toggleAccel(true);
    sensor.toggleGyro(true);
    sensor.setAccelDataRate(0xA0);
    sensor.setGyroDataRate(0xA0);
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
