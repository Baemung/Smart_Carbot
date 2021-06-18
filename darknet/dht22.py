import Adafruit_DHT as dht
import time
import pymysql
import sys

Outpin=4
while(1):
    Humi, Temp = dht.read_retry(dht.DHT22, Outpin)
    Values = int(Humi), int(Temp)
    print(Values)
    time.sleep(1)