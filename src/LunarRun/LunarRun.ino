#include <SPI.h>
#include <Gamebuino.h>
#include <petit_fatfs.h>
#include "vector2.h"
Gamebuino gb;

#define NUM_LEVELS 111
extern const unsigned level_offsets[NUM_LEVELS] PROGMEM;

enum ship_mode
{
  Maneuver,
  Landing
};

enum ship_state
{
  Flying,
  Landed
};

enum gui_state
{
  Title,
  MainMenu,
  Playing
};

extern const byte logo[] PROGMEM;

#define GRAVITY 0.0025f
#define MANUEVER_THRUST 0.01f
#define ROTATION_SPEED 0.05f
#define LANDING_THRUST 0.01f
#define SIDEWAYS_THRUST 0.005f

gui_state gui;
vector2 ship_pos;
vector2 ship_vel;
vector2 last_ship_pos;
vector2 last_ship_vel;
ship_mode ship_mode;
ship_state ship_state;
double ship_angle = 0;
float fuel = 200;
boolean render_down_flame = false, render_left_flame = false, render_right_flame = false;       
boolean grayscale_frame = false;
boolean thrust_released = false;
unsigned current_screen;

const char strContinue[] PROGMEM = "Continue";
const char strTutorial[] PROGMEM = "Tutorial";
const char* const menu[] PROGMEM = {strContinue, strTutorial};
#define MENULENGTH (sizeof(menu) / sizeof(const char *))
  
void setup(){
  Serial.begin(9600);
  Serial.println("Starting...");

  spi_init();
  PFFS.begin(2, rx, tx);
  
  gb.begin();
  gb.setFrameRate(41);  // magic frame rate for best gray-scale
  //title_screen();
  main_menu();
}

void loop(){
  switch (gui)
  {
  case Title:
    title_screen();
    break;

  case MainMenu:
    main_menu();
    break;

  case Playing:
    update_game();
    break;
  }
}

void update_game() {
  if(gb.update()) {
    /*
    grayscale_frame = !grayscale_frame;

    if (gb.buttons.pressed(BTN_C))
    {
      main_menu();
      return;
    }

    */
    draw_ship(render_down_flame, render_left_flame, render_right_flame, grayscale_frame);
    unsigned last_screen = current_screen;
    update_ship(render_down_flame, render_left_flame, render_right_flame);
    unsigned current_screen_x = (unsigned)ship_pos.x / LCDWIDTH;
    unsigned current_screen_y = (unsigned)ship_pos.y / LCDHEIGHT;
    current_screen = (current_screen_y << 8) + current_screen_x;
    if (current_screen != last_screen)
    {
      // ship has moved to a different screen    
      init_landscape(current_screen);
    }
    //draw_landscape(); // landscape needs to be drawn first in order for collision detection to work
    do_collision_detection();
    draw_ship(render_down_flame, render_left_flame, render_right_flame, grayscale_frame);
    //draw_fuel();
    last_ship_pos = ship_pos;
    last_ship_vel = ship_vel;
  }
}

void title_screen() {
  gb.titleScreen(F("Lunar Run"), logo);
  gui = MainMenu;
}

void main_menu() {
  /*
  int result;
  if (current_level == NULL)
    result = gb.menu(menu+1, MENULENGTH-1) + 1;
  else
    result = gb.menu(menu, MENULENGTH);
  switch(result) {
  case 0: 
    gui = Playing;
    return;
  case 1:
    next_level = &tutorial;
    break;
  case 2:
    next_level = &level1;
    break;
  case 3:
    next_level = &level2;
    break;
  case 4:
    next_level = &level4;
    break;
  default:
    current_level = NULL;
    gui = Title;
    return;
  }
  */
  gui = Playing;
  init_game();
}

void init_game() {
  Serial.println("init_game");
  gb.display.persistence = true;
  
  current_screen = 0x0500;
  
  // initialize the ship position
  float x = (current_screen & 255) * 84 + 10;
  float y = (current_screen >> 8) * 48 + 24;
  ship_pos = last_ship_pos = vector2(x, y);
  ship_vel = last_ship_vel = vector2(0, 0);
  ship_angle = 0.0f;
  ship_mode = Landing;
  ship_state = Flying;
  thrust_released = false;
  
  // start with the ship already drawn
  init_landscape(current_screen);
  draw_ship(render_down_flame, render_left_flame, render_right_flame, grayscale_frame);
}

  
