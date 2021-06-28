#define MOTOR_A_a 3     //모터A의 +출력핀은 3번핀입니다
#define MOTOR_A_b 11    //모터A의 -출력핀은 11번핀입니다
#define MOTOR_B_a 5     //모터B의 +출력핀은 5번핀입니다
#define MOTOR_B_b 6     //모터B의 -출력핀은 6번핀입니다
#define MOTOR_SPEED 255  //모터의 기준속력입니다(0~255), 라인트레이서는 저속주행을 권장드립니다.
#define LINESENS_L 7    //왼쪽 라인센서 입력핀입니다.
#define LINESENS_R 8    //오른쪽 라인센서 입력핀입니다.

#define TRIG 9 //TRIG 핀 설정 (초음파 보내는 핀)
#define ECHO 10 //ECHO 핀 설정 (초음파 받는 핀)

unsigned char m_a_spd = 0, m_b_spd = 0; //모터의 속력을 저장하는 전역변수
boolean m_a_dir = 0, m_b_dir = 0; //모터의 방향을 결정하는 전역변수

void setup()  
{
  TCCR1B = TCCR1B & 0b11111000 | 0x05;  //저속주행이 가능하도록 모터A PWM 주파수 변경
  TCCR2B = TCCR2B & 0b11111000 | 0x07;  //저속주행이 가능하도록 모터B PWM 주파수 변경
  
  //모터 제어 핀들을 출력으로 설정합니다.
  pinMode(MOTOR_A_a, OUTPUT);
  pinMode(MOTOR_A_b, OUTPUT);
  pinMode(MOTOR_B_a, OUTPUT);
  pinMode(MOTOR_B_b, OUTPUT);

  //라인센서 핀들을 입력으로 설정합니다.
  pinMode(LINESENS_L, INPUT);
  pinMode(LINESENS_R, INPUT);

  //초음파 센서
  pinMode(TRIG, OUTPUT);
  pinMode(ECHO, INPUT);
}

void loop()
{
    int dist = get_dist(); //장애물이 없으면
    if(dist >= 30)
    {
      linetrace_val();  //입력된 데이터에 따라 모터에 입력될 변수를 조정하는 함수
      motor_drive();    //모터를 구동하는 함수
    }
    else
    {
      m_a_dir = 0;  //모터A 정방향
      m_b_dir = 0;  //모터B 정방향
      m_a_spd = 0;  //모터A의 정지
      m_b_spd = 0;  //모터B의 정지
      motor_drive();
    }
}

void linetrace_val() //입력된 데이터에 따라 모터에 입력될 변수를 조정하는 함수
{
  boolean line_l = digitalRead(LINESENS_L), line_r = digitalRead(LINESENS_R); //왼쪽과 오른쪽 라인센서의 감지값을 변수에 저장합니다.
  
  if(line_l == 0 && line_r == 0)      //라인이 감지되지 않을때 전진
  {
    m_a_dir = 0;  //모터A 정방향
    m_b_dir = 0;  //모터B 정방향
    m_a_spd = MOTOR_SPEED;  //모터A의 속력값 조정
    m_b_spd = MOTOR_SPEED;  //모터B의 속력값 조정
  }
  else if(line_l == 1 && line_r == 0) //왼쪽 센서 감지시 왼쪽으로 전진
  {
    m_a_dir = 1;  //모터A 역방향
    m_b_dir = 0;  //모터B 정방향
    m_a_spd = 0;  //모터A의 정지
    m_b_spd = constrain(MOTOR_SPEED*2
    , 0, 255);  //모터B의 속력값 조정
  }
  else if(line_l == 0 && line_r == 1)  //오른쪽 센서 감지시 오른쪽으로 전진
  {
    m_a_dir = 0;  //모터A 정방향
    m_b_dir = 1;  //모터B 역방향
    m_a_spd = constrain(MOTOR_SPEED*2, 0, 255);  //모터A의 속력값 조정
    m_b_spd = 0;  //모터B의 정지
  }
  else
  {
    m_a_dir = 0;  //모터A 정방향
    m_b_dir = 0;  //모터B 정방향
    m_a_spd = 0;  //모터A의 정지
    m_b_spd = 0;  //모터B의 정지
  }
}

void motor_drive()  //모터를 구동하는 함수
{
  if(m_a_dir == 0)
  {
    digitalWrite(MOTOR_A_a, LOW);     //모터A+ LOW
    analogWrite(MOTOR_A_b, m_a_spd);  //모터A-의 속력을 PWM 출력
  }
  else
  {
    analogWrite(MOTOR_A_a, m_a_spd);  //모터A+의 속력을 PWM 출력
    digitalWrite(MOTOR_A_b, LOW);     //모터A- LOW
  }
  if(m_b_dir == 1)
  {
    digitalWrite(MOTOR_B_a, LOW);     //모터B+ LOW
    analogWrite(MOTOR_B_b, m_b_spd);  //모터B-의 속력을 PWM 출력
  }
  else
  {
    analogWrite(MOTOR_B_a, m_b_spd);  //모터B+의 속력을 PWM 출력
    digitalWrite(MOTOR_B_b, LOW);     //모터B- LOW
  }
}

int get_dist()
{
  long duration, distance;
  
  digitalWrite(TRIG, LOW);
  delayMicroseconds(2);
  digitalWrite(TRIG, HIGH);
  delayMicroseconds(10);
  digitalWrite(TRIG, LOW);

  duration = pulseIn (ECHO, HIGH); //물체에 반사되어돌아온 초음파의 시간을 변수에 저장합

  //34000*초음파가 물체로 부터 반사되어 돌아오는시간 /1000000 / 2(왕복값이아니라 편도값이기때문에 나누기2를 함
  //초음파센서의 거리값이 위 계산값과 동일하게 Cm로 환산되는 계산공식 입니다. 수식이 간단해지도록 적용
  distance = duration * 17 / 1000; 

  //PC모니터로 초음파 거리값을 확인 하는 코드
  return distance;
}
