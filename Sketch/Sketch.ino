#include "HX711.h"
#include <Wire.h>
#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>

// #define DOUT  2 //데이터 핀
// #define CLK  3// 클럭 핀

// #define VRX A7
// #define VRY A6
// #define SW 4

// #define VIB 5

#define DOUT  3 //데이터 핀
#define CLK  4// 클럭 핀

#define VRX A7
#define VRY A6
#define SW 5

#define VIB 6

Adafruit_MPU6050 mpu;
sensors_event_t a, g, temp;

HX711 scale(DOUT, CLK);

void setup() {
    Serial.begin(115200);
    mpu.begin();
      // 가속도 측정 범위 설정 (±2G, ±4G, ±8G, ±16G 중 선택)
  mpu.setAccelerometerRange(MPU6050_RANGE_2_G);
  // 필터 대역폭 설정
  mpu.setFilterBandwidth(MPU6050_BAND_21_HZ);

      scale.set_scale(100);
    scale.tare(); //영점잡기. 현재 측정값을 0으로 둔다.
    pinMode(SW, INPUT_PULLUP);
    pinMode(VIB, OUTPUT);

    delay(2000);    
}
void loop() {
    mpu.getEvent(&a, &g, &temp);
    
    float fdata[] = {a.acceleration.x, a.acceleration.y, a.acceleration.z, g.gyro.x, g.gyro.y, g.gyro.z, -scale.get_units()};
    int32_t idata[] = {analogRead(VRX), analogRead(VRY)};

    byte packet[38];
    packet[0] = 0xAA; //Header
    memcpy(packet+1, fdata, 28);
    memcpy(packet+29, idata, 8);
    packet[37] = digitalRead(SW);
    Serial.write(packet, 38);

    if(Serial.available()) analogWrite(VIB, Serial.read());
}