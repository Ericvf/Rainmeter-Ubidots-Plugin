#define WIFISSID "***************"
#define PASSWORD "***************"

#include <ESP8266WiFi.h>
#include "UbidotsMicroESP8266.h"
#define TOKEN "***************"  // Put here your Ubidots TOKEN
#define VAR_TEMPERATURE "***************" // UbiDots VAR 
Ubidots client(TOKEN);

#include <OneWire.h>
#include <DallasTemperature.h>

const int TEMP_PIN = 2;
OneWire oneWire(TEMP_PIN);  // on pin 10 (a 4.7K resistor is necessary)
DallasTemperature sensors(&oneWire);

const int BLINK_PIN = 1;
void blink(int number);

void setup(void) {
  pinMode(TEMP_PIN, INPUT_PULLUP);
  pinMode(BLINK_PIN, OUTPUT);

  WiFi.hostname("BossTemperature");
  WiFi.mode(WIFI_STA);

  client.wifiConnection(WIFISSID, PASSWORD);
  sensors.begin();

  blink(3);
}

void loop(void) {
  blink(3);

  sensors.requestTemperatures();
  float temp = sensors.getTempCByIndex(0);

  client.add(VAR_TEMPERATURE, temp);
  client.sendAll(false);

  delay(1000 * 60 * 5);
}

void blink(int number){
  for (int i = 0; i < number; i++)
  {
    digitalWrite(BLINK_PIN, LOW);
    delay(500);

    digitalWrite(BLINK_PIN, HIGH);
    delay(500);
  }
}
