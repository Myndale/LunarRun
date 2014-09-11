const byte mask[] PROGMEM = {
  64, 64,
  0x00,0x02,0x00,0x10,0x00,0x80,0x00,0x00,0x00,0x03,0x00,0x10,0x01,0x80,0x00,0x00,0x30,0x03,0x80,0x38,0x03,0x80,0x18,0x00,0x3C,0x03,0xC0,0x38,0x07,0x80,0x78,0x00,
  0x1F,0x03,0xE0,0x7C,0x0F,0x81,0xF0,0x00,0x1F,0x03,0xE0,0x7C,0x0F,0x81,0xF0,0x00,0x0F,0x01,0xC0,0x38,0x07,0x01,0xE0,0x00,0x0E,0x00,0x00,0x00,0x00,0x00,0xE0,0x00,
  0x00,0xC0,0x70,0x38,0x1C,0x06,0x00,0x00,0x00,0xE0,0x70,0x38,0x1C,0x0E,0x00,0x00,0x00,0x60,0x30,0x10,0x18,0x0C,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFC,0x00,0x20,0x80,0x00,0x00,0x7E,0x00,0x7E,0x00,0x71,0xC0,0x00,0x00,0xFC,0x00,
  0x3E,0x00,0x71,0xC0,0x00,0x00,0xF8,0x00,0x1E,0xC0,0x20,0x80,0x00,0x06,0xF0,0x00,0x0C,0xE0,0x51,0x40,0x00,0x0E,0x60,0x00,0x00,0xE0,0x00,0x00,0x00,0x0E,0x00,0x00,
  0x00,0x00,0x21,0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x73,0xE0,0x00,0x00,0x00,0x00,0x00,0x00,0x21,0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x80,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x0C,0x00,0x00,0x01,0x00,0x00,0x60,0x00,0x3E,0xC0,0x22,0x13,0x80,0x06,0xF8,0x00,0xFE,0xE0,0x77,0x3B,0xC0,0x0E,0xFE,0x00,
  0x3E,0xC0,0x72,0x3B,0x80,0x06,0xF8,0x00,0x0C,0x00,0x20,0x11,0x00,0x00,0x60,0x00,0x00,0x00,0x50,0x28,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xE0,0x20,0x82,0x08,0x0E,0x00,0x00,
  0x0C,0xE0,0x71,0xC7,0x1C,0x0E,0x60,0x00,0x1E,0xC0,0x71,0xC7,0x1C,0x06,0xF0,0x00,0x3E,0x00,0x00,0x82,0x08,0x00,0xF8,0x00,0x7E,0x00,0x00,0x02,0x14,0x00,0xFC,0x00,
  0xFC,0x00,0x00,0x00,0x00,0x00,0x7E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x60,0x30,0x10,0x18,0x0C,0x00,0x00,0x00,0xE0,0x70,0x38,0x1C,0x0E,0x00,0x00,0x00,0xC0,0x70,0x38,0x1C,0x06,0x00,0x00,0x0E,0x00,0x00,0x00,0x00,0x00,0xE0,0x00,
  0x0F,0x01,0xC0,0x38,0x07,0x01,0xE0,0x00,0x1F,0x03,0xE0,0x7C,0x0F,0x81,0xF0,0x00,0x1F,0x03,0xE0,0x7C,0x0F,0x81,0xF0,0x00,0x3C,0x03,0xC0,0x38,0x07,0x80,0x78,0x00,
  0x30,0x03,0x80,0x38,0x03,0x80,0x18,0x00,0x00,0x03,0x00,0x10,0x01,0x80,0x00,0x00,0x00,0x02,0x00,0x10,0x00,0x80,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
};


const byte image1[] PROGMEM = {
  64, 64,
  0x00,0x02,0x00,0x10,0x00,0x80,0x00,0x00,0x00,0x03,0x00,0x10,0x01,0x80,0x00,0x00,0x30,0x03,0x80,0x38,0x03,0x80,0x18,0x00,0x3C,0x03,0xC0,0x38,0x07,0x80,0x78,0x00,
  0x1F,0x03,0xE0,0x7C,0x0F,0x81,0xF0,0x00,0x1F,0x03,0x60,0x6C,0x0D,0x81,0xF0,0x00,0x0D,0x01,0xC0,0x38,0x07,0x01,0x60,0x00,0x0E,0x00,0x00,0x00,0x00,0x00,0xE0,0x00,
  0x00,0xC0,0x70,0x38,0x1C,0x06,0x00,0x00,0x00,0xA0,0x50,0x28,0x14,0x0A,0x00,0x00,0x00,0x60,0x30,0x10,0x18,0x0C,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFC,0x00,0x20,0x80,0x00,0x00,0x7E,0x00,0x7E,0x00,0x51,0x40,0x00,0x00,0xFC,0x00,
  0x3A,0x00,0x71,0xC0,0x00,0x00,0xB8,0x00,0x1E,0xC0,0x20,0x80,0x00,0x06,0xF0,0x00,0x0C,0xA0,0x51,0x40,0x00,0x0A,0x60,0x00,0x00,0xE0,0x00,0x00,0x00,0x0E,0x00,0x00,
  0x00,0x00,0x21,0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x73,0x60,0x00,0x00,0x00,0x00,0x00,0x00,0x21,0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x80,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x0C,0x00,0x00,0x01,0x00,0x00,0x60,0x00,0x3E,0xC0,0x22,0x13,0x80,0x06,0xF8,0x00,0xFA,0xA0,0x57,0x2A,0xC0,0x0A,0xBE,0x00,
  0x3E,0xC0,0x72,0x3B,0x80,0x06,0xF8,0x00,0x0C,0x00,0x20,0x11,0x00,0x00,0x60,0x00,0x00,0x00,0x50,0x28,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xE0,0x20,0x82,0x08,0x0E,0x00,0x00,
  0x0C,0xA0,0x51,0x45,0x14,0x0A,0x60,0x00,0x1E,0xC0,0x71,0xC7,0x1C,0x06,0xF0,0x00,0x3A,0x00,0x00,0x82,0x08,0x00,0xB8,0x00,0x7E,0x00,0x00,0x02,0x14,0x00,0xFC,0x00,
  0xFC,0x00,0x00,0x00,0x00,0x00,0x7E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x60,0x30,0x10,0x18,0x0C,0x00,0x00,0x00,0xA0,0x50,0x28,0x14,0x0A,0x00,0x00,0x00,0xC0,0x70,0x38,0x1C,0x06,0x00,0x00,0x0E,0x00,0x00,0x00,0x00,0x00,0xE0,0x00,
  0x0D,0x01,0xC0,0x38,0x07,0x01,0x60,0x00,0x1F,0x03,0x60,0x6C,0x0D,0x81,0xF0,0x00,0x1F,0x03,0xE0,0x7C,0x0F,0x81,0xF0,0x00,0x3C,0x03,0xC0,0x38,0x07,0x80,0x78,0x00,
  0x30,0x03,0x80,0x38,0x03,0x80,0x18,0x00,0x00,0x03,0x00,0x10,0x01,0x80,0x00,0x00,0x00,0x02,0x00,0x10,0x00,0x80,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
};


const byte image2[] PROGMEM = {
  64, 64,
  0x00,0x02,0x00,0x10,0x00,0x80,0x00,0x00,0x00,0x03,0x00,0x10,0x01,0x80,0x00,0x00,0x30,0x02,0x80,0x28,0x02,0x80,0x18,0x00,0x2C,0x02,0x40,0x28,0x04,0x80,0x68,0x00,
  0x13,0x02,0x20,0x44,0x08,0x81,0x90,0x00,0x11,0x02,0x20,0x44,0x08,0x81,0x10,0x00,0x09,0x01,0xC0,0x38,0x07,0x01,0x20,0x00,0x0E,0x00,0x00,0x00,0x00,0x00,0xE0,0x00,
  0x00,0xC0,0x70,0x38,0x1C,0x06,0x00,0x00,0x00,0xA0,0x50,0x28,0x14,0x0A,0x00,0x00,0x00,0x60,0x30,0x10,0x18,0x0C,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFC,0x00,0x20,0x80,0x00,0x00,0x7E,0x00,0x42,0x00,0x51,0x40,0x00,0x00,0x84,0x00,
  0x22,0x00,0x71,0xC0,0x00,0x00,0x88,0x00,0x12,0xC0,0x20,0x80,0x00,0x06,0x90,0x00,0x0C,0xA0,0x51,0x40,0x00,0x0A,0x60,0x00,0x00,0xE0,0x00,0x00,0x00,0x0E,0x00,0x00,
  0x00,0x00,0x21,0xC0,0x00,0x00,0x00,0x00,0x00,0x00,0x52,0x20,0x00,0x00,0x00,0x00,0x00,0x00,0x21,0x40,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x80,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x0C,0x00,0x00,0x01,0x00,0x00,0x60,0x00,0x32,0xC0,0x22,0x12,0x80,0x06,0x98,0x00,0xC2,0xA0,0x55,0x2A,0x40,0x0A,0x86,0x00,
  0x32,0xC0,0x72,0x3A,0x80,0x06,0x98,0x00,0x0C,0x00,0x20,0x11,0x00,0x00,0x60,0x00,0x00,0x00,0x50,0x28,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xE0,0x20,0x82,0x08,0x0E,0x00,0x00,
  0x0C,0xA0,0x51,0x45,0x14,0x0A,0x60,0x00,0x12,0xC0,0x71,0xC7,0x1C,0x06,0x90,0x00,0x22,0x00,0x00,0x82,0x08,0x00,0x88,0x00,0x42,0x00,0x00,0x02,0x14,0x00,0x84,0x00,
  0xFC,0x00,0x00,0x00,0x00,0x00,0x7E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x60,0x30,0x10,0x18,0x0C,0x00,0x00,0x00,0xA0,0x50,0x28,0x14,0x0A,0x00,0x00,0x00,0xC0,0x70,0x38,0x1C,0x06,0x00,0x00,0x0E,0x00,0x00,0x00,0x00,0x00,0xE0,0x00,
  0x09,0x01,0xC0,0x38,0x07,0x01,0x20,0x00,0x11,0x02,0x20,0x44,0x08,0x81,0x10,0x00,0x13,0x02,0x20,0x44,0x08,0x81,0x90,0x00,0x2C,0x02,0x40,0x28,0x04,0x80,0x68,0x00,
  0x30,0x02,0x80,0x28,0x02,0x80,0x18,0x00,0x00,0x03,0x00,0x10,0x01,0x80,0x00,0x00,0x00,0x02,0x00,0x10,0x00,0x80,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
};






void update_ship(boolean & render_down_flame, boolean & render_left_flame, boolean & render_right_flame)
{
  // apply thrusters
  if ((fuel > 0) && (gb.buttons.repeat(BTN_DOWN, 1) || gb.buttons.repeat(BTN_A, 1)))
    ship_vel += down_thrust, fuel -= down_rate, render_down_flame = true;
  if ((fuel > 0) && gb.buttons.repeat(BTN_LEFT, 1))
    ship_vel -= sideways_thrust, fuel -= sideways_rate, render_right_flame = true;
  if ((fuel > 0) && gb.buttons.repeat(BTN_RIGHT, 1))
    ship_vel += sideways_thrust, fuel -= sideways_rate, render_left_flame = true;
  if (fuel < 0)
    fuel = 0;

  // apply gravity
  ship_vel += gravity;
  
  // update the ship's position
  ship_pos += ship_vel;
  
  // clamp to landscape boundry
  if (ship_pos.x < 0)
  {
    ship_pos.x = -ship_pos.x;
    ship_vel.x = -ship_vel.x;
  }
  if (ship_pos.x >= NUM_COLUMNS*LCDWIDTH)
  {
    ship_pos.x = 2*NUM_COLUMNS*LCDWIDTH - ship_pos.x;
    ship_vel.x = -ship_vel.x;
  }
  if (ship_pos.y >= NUM_ROWS*LCDHEIGHT)
  {
    ship_pos.x = 2*NUM_ROWS*LCDHEIGHT - ship_pos.y;
    ship_vel.y = -ship_vel.y;
  }
  
  fuel = 1000;
}

/*
void draw_ship(boolean render_down_flame, boolean render_left_flame, boolean render_right_flame) {
  int shipx = (int)ship_pos.x % 84;
  int shipy = (int)ship_pos.y % 48;
   
  gb.display.drawBitmap(shipx-4, shipy-8, ship);
  if (grayscale_frame)
  {
    if (render_down_flame)
      gb.display.drawBitmap(shipx-4, shipy, down_flame);
    if (render_left_flame)
      gb.display.drawBitmap(shipx-4-8, shipy-8, left_flame);
    if (render_right_flame)
      gb.display.drawBitmap(shipx-4+8, shipy-8, right_flame);
  }
}
*/

void draw_ship(boolean render_down_flame, boolean render_left_flame, boolean render_right_flame, boolean grayscale_frame) {
  
  float ship_x = ship_pos.x;
  float ship_y = ship_pos.y;
  while (ship_y < 0)
    ship_y += LCDHEIGHT;
  
  int8_t src_x = 25;
  int8_t src_y = 44;
  int8_t dst_w = 5;
  int8_t dst_h = 11;
  int8_t dst_x = (int8_t)((int)ship_x % LCDWIDTH - 2);
  int8_t dst_y = (int8_t)((int)ship_y % LCDHEIGHT - 2);
  
  
  gb.display.setColor(WHITE);
  drawBitmap(dst_x, dst_y, dst_w, dst_h, src_x, src_y, mask);
  gb.display.setColor(BLACK);
  if (grayscale_frame)
    drawBitmap(dst_x, dst_y, dst_w, dst_h, src_x, src_y, image1);
  else
    drawBitmap(dst_x, dst_y, dst_w, dst_h, src_x, src_y, image2);
}

void draw_fuel() {
  gb.display.cursorX = 0;
  gb.display.cursorY = 0;
  gb.display.print("FUEL:");
  gb.display.println((int)fuel);
}


