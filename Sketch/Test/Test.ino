#include "HX711.h"
#include<Wire.h>
#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>

#define calibration_factor 100 //캘리브레이션 값
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
    Serial.begin(115200);  // 값 모니터링 위해서...
    scale.set_scale(calibration_factor);
    scale.tare(); //영점잡기. 현재 측정값을 0으로 둔다.
    mpu.begin();
    pinMode(SW, INPUT_PULLUP);
    pinMode(VIB, OUTPUT);

    delay(2000);    
}

void loop() {
    mpu.getEvent(&a, &g, &temp);
    float fdata[] = {scale.get_units(), a.acceleration.x, a.acceleration.y, a.acceleration.z, g.gyro.x, g.gyro.y, g.gyro.z};
    int32_t idata[] = {analogRead(VRX), analogRead(VRY)};

    for(int i = 0; i<7; i++){
        Serial.print(fdata[i], 2);
        Serial.print("\t");
    }
    for(int i = 0; i<2; i++){
        Serial.print(idata[i]);
        Serial.print("\t");
    }
    Serial.println(digitalRead(SW));

    if(Serial.available()) analogWrite(VIB, Serial.parseInt());
    delay(500);
}