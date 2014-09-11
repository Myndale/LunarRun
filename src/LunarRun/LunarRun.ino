#include <SPI.h>
#include <Gamebuino.h>
#include "vector2.h"
Gamebuino gb;

extern const byte logo[] PROGMEM;
extern const byte ship[] PROGMEM;
extern const byte down_flame[] PROGMEM;
extern const byte left_flame[] PROGMEM;
extern const byte right_flame[] PROGMEM;

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
  Serial.begin(9600);
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
      
      update_ship(render_down_flame, render_left_flame, render_right_flame);
    
      draw_landscape();
      draw_ship(render_down_flame, render_left_flame, render_right_flame, grayscale_frame);
      //draw_fuel();
  }
}

void title_screen() {
  gb.titleScreen(F("Lunar Run"), logo);
}

