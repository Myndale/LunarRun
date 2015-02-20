#include <petit_fatfs.h>

void init_landscape(/*level * new_level*/) {
  loadScreen(40);
}

void draw_landscape() {
 
}

void loadScreen(unsigned screen_num)
{
  int bytes_read;
  digitalWrite(SS, LOW);
  int err = PFFS.open_file("LUNARRUN.DAT");
  if (err == 0)
  {  
    PFFS.lseek_file(screen_num*(LCDWIDTH*LCDHEIGHT/8));
    uint8_t * buffer = gb.display.getBuffer();
    buffer[0] = 0x55;
    for (int row=0; row<LCDHEIGHT/8; row++, buffer+=LCDWIDTH)
      PFFS.read_file((char *)buffer, LCDWIDTH, &bytes_read);
  }
  else
    Serial.print("Error code "); Serial.print(err);
  digitalWrite(SS, HIGH);
}

byte rx()
{
  SPDR = 0xFF;
  loop_until_bit_is_set(SPSR, SPIF);
  return SPDR;
}

void tx(byte d)
{
  SPDR = d;
  loop_until_bit_is_set(SPSR, SPIF);
}

void spi_init()
{
  pinMode(10, OUTPUT);
  pinMode(11, OUTPUT);
  pinMode(12, INPUT);
  pinMode(13, OUTPUT);
  
  SPCR = _BV(MSTR) | _BV(SPE);      // Master mode, SPI enable, clock speed MCU_XTAL/4
}

