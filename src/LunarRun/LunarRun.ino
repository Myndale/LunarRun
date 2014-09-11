#include <SPI.h>
#include <Gamebuino.h>
#include "vector2.h"
Gamebuino gb;

extern const byte logo[] PROGMEM;
extern const byte ship[] PROGMEM;
extern const byte down_flame[] PROGMEM;
extern const byte left_flame[] PROGMEM;
extern const byte right_flame[] PROGMEM;

#define GRAVITY 0.005f
#define THRUST 0.015f
#define ROTATION_SPEED 0.03f

vector2 ship_pos;
vector2 ship_vel;
double ship_angle = 0;
float fuel = 200;
boolean grayscale_frame = false;


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
       
      if (gb.buttons.pressed(BTN_B))
        init_game();
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
}


