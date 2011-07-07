#define LEDPIN 7
#define SWPIN 6
#
byte bitmap[] = {
0b00000000,
0b01111111,
0b01000001,
0b01000001,
0b00111110,
0b00000000,
0b01111110,
0b00001001,
0b00001001,
0b01111110,
0b00000000,
0b01111111,
0b00000110,
0b00011000,
0b01111111,
0b00000000,
0b10000000
};

unsigned char pos = 0;
unsigned char i = 0;

void setup() {
   for(char pin = LEDPIN; pin < LEDPIN + 7; pin++) {
     pinMode(pin, OUTPUT);
   }
   pinMode(SWPIN, INPUT);
   digitalWrite(SWPIN, HIGH);
}

void loop() {
   if(digitalRead(SWPIN) == LOW) {
     delay(6);
     pos = 0;
     while(bitmap[++pos] != 0b10000000) {
       for(i = 0; i < 7; i++) {
         digitalWrite(LEDPIN + 6 - i, (bitmap[pos] >> i) & 0b00000001);
       }
       delay(1);
     }
     for(i = LEDPIN; i < LEDPIN + 7; i++) {
       digitalWrite(i, LOW);
     }
   }
}
