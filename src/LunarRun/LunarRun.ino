#include <SPI.h>
#include <Gamebuino.h>
#include "vector2.h"
Gamebuino gb;

enum ship_mode
{
  Maneuver,
  Landing
};

extern const byte logo[] PROGMEM;

#define GRAVITY 0.0025f
#define MANUEVER_THRUST 0.01f
#define ROTATION_SPEED 0.05f
#define LANDING_THRUST 0.01f
#define SIDEWAYS_THRUST 0.005f

vector2 ship_pos;
vector2 ship_vel;
ship_mode ship_mode;
double ship_angle = 0;
float fuel = 200;
boolean grayscale_frame = false;


void setup(){
  gb.begin();
  gb.setFrameRate(41);  // magic frame rate for best gray-scale
  title_screen();  
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
  init_game();
}

void init_game() {
  ship_pos = vector2(10, 10);
  ship_vel = vector2(0, 0);
  ship_angle = 0.0f;
  ship_mode = Landing;
}


