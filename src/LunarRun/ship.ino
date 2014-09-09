
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
  
  fuel = 1000;
}

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

void draw_fuel() {
  gb.display.cursorX = 0;
  gb.display.cursorY = 0;
  gb.display.print("FUEL:");
  gb.display.println((int)fuel);
}


