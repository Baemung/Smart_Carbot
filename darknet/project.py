import time
import cv2
import pymysql
import Adafruit_DHT as dht
from color import TCS34725
from strawberry import STRAWBERRY

#Maria DB
Conn = pymysql.connect(port=3306, host='192.168.0.10', user='root', password='12341234', db='kosta', charset='utf8')
Cur = Conn.cursor()

#Strawberry
strawberry = STRAWBERRY()
QueryBerry = "insert into berrystatus values (%s, %s, %s, now())"

#DHT22
Outpin = 4
QueryDHT = "insert into sensorvalue values (%s, %s, %s, now())"

#Sector
color = TCS34725()
lastSector = 0
 
for frame in strawberry.camera.capture_continuous(strawberry.rawCapture, format="bgr", use_video_port=True):
    Ripe, unRipe = strawberry.detectAndDisplay(frame.array)
    strawberry.rawCapture.truncate(0)
    
    Sector = color.get_sector()
    print(Sector)
    if Ripe + unRipe != 0 and Sector != 0:
        Values = Ripe, unRipe, Sector
        Cur.execute(QueryBerry, Values)
        Conn.commit()
    
    if Sector != lastSector and Sector != 0:
        Hum, Temp = dht.read_retry(dht.DHT22, Outpin)
        Values = int(Hum), int(Temp), Sector
        print(Sector)
        Cur.execute(QueryDHT, Values)
        Conn.commit()
        
    lastSector = Sector
    
Conn.close()
sys.exit(0)