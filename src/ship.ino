#include <SPI.h>
#include <Gamebuino.h>
#include "vector2.ino"
Gamebuino gb;

#define NUM_PADS 3

struct pad
{
  int pos;
  int width;
  int height;
  int multiplier;
};

vector2 ship_pos(10, 10);
vector2 ship_vel(0.5f, 0);
const vector2 gravity(0, 0.005f);
const vector2 down_thrust(0, -0.025f);
const vector2 sideways_thrust(0.01f, 0);
const float down_rate = 1.0f;
const float sideways_rate = 1.0f;
float fuel = 200;
boolean grayscale_frame = false;

pad pads[NUM_PADS];


void setup(){
  gb.begin();
  gb.setFrameRate(41);
  title_screen();  
  randomize_landscape();
}

void loop(){
  if(gb.update()){
       boolean render_down_flame = false, render_left_flame = false, render_right_flame = false;
       grayscale_frame = !grayscale_frame;
    
      if (gb.buttons.pressed(BTN_C))
      {
        title_screen();
        return;
      }      
    
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
      
      // check for collisions
      
      draw_landscape();
      draw_ship(render_down_flame, render_left_flame, render_right_flame);
      draw_fuel();
  }
}

void title_screen() {
  gb.titleScreen(F("Lunar Lander"), logo);
}

void randomize_landscape() {
  pads[0].pos = 20;
  pads[0].width = 5;
  pads[0].height = 35;
  pads[0].multiplier = 4;
  
  pads[1].pos = 30;
  pads[1].width = 8;
  pads[1].height = 32;
  pads[1].multiplier = 2;
  
  pads[2].pos = 60;
  pads[2].width = 10;
  pads[2].height = 40;
  pads[2].multiplier = 1;
}

void draw_landscape() {
  gb.display.setColor(BLACK);
  
  gb.display.drawLine(0, LCDWIDTH, pads[0].pos, pads[0].height+1);
  gb.display.drawLine(pads[0].pos+pads[0].width, pads[0].height+1, pads[1].pos, pads[1].height+1);
  gb.display.drawLine(pads[1].pos+pads[1].width, pads[1].height+1, pads[2].pos, pads[2].height+1);
  gb.display.drawLine(pads[2].pos+pads[2].width, pads[2].height+1, LCDHEIGHT, LCDWIDTH);
    
   for (int pad_num=0; pad_num<NUM_PADS; pad_num++)
   {
     pad & this_pad = pads[pad_num];
     gb.display.drawLine(this_pad.pos, this_pad.height, this_pad.pos+this_pad.width, this_pad.height);
     gb.display.drawLine(this_pad.pos, this_pad.height+1, this_pad.pos+this_pad.width, this_pad.height+1);
     gb.display.cursorX = this_pad.pos+2;
     gb.display.cursorY = this_pad.height+3;
     gb.display.print(this_pad.multiplier);
     gb.display.print("x");
   }
}

void draw_ship(boolean render_down_flame, boolean render_left_flame, boolean render_right_flame) {
  gb.display.drawBitmap(ship_pos.x-4, ship_pos.y-8, ship);
  if (grayscale_frame)
  {
    if (render_down_flame)
      gb.display.drawBitmap(ship_pos.x-4, ship_pos.y, down_flame);
    if (render_left_flame)
      gb.display.drawBitmap(ship_pos.x-4-8, ship_pos.y-8, left_flame);
    if (render_right_flame)
      gb.display.drawBitmap(ship_pos.x-4+8, ship_pos.y-8, right_flame);
  }
}

void draw_fuel() {
  gb.display.cursorX = 0;
  gb.display.cursorY = 0;
  gb.display.print("FUEL:");
  gb.display.println((int)fuel);
}

