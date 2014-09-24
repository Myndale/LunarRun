#include "levels.h"

byte rows, columns;
level * current_level = NULL;

void init_landscape(level * new_level) {
  /*
  for (int y=0; y<NUM_ROWS; y++)
    for (int x=0; x<NUM_COLUMNS; x++)
      pads_visited[y][x] = 0;
      */
  
  current_level = new_level;
  rows = pgm_read_byte(&current_level->rows);
  columns  = pgm_read_byte(&current_level->columns);
}

void draw_landscape() {
  if (ship_pos.y < 0)
    return;
  int column = (int)(ship_pos.x / 84);
  int row  = (int)(ship_pos.y / 48);
  const section * sections = (const section *)pgm_read_word(&current_level->sections);  
  const byte * landscape = (const byte *)pgm_read_word(&sections[row*columns+column]);
  if (!landscape)
    return;  // blank section
  gb.display.setColor(BLACK);
  byte x = 0, y = 0;
  int index = 0;
  while (x < 84)
  {
    byte span1 = pgm_read_byte(landscape++);
    byte span2 = pgm_read_byte(landscape++);
    byte span3 = pgm_read_byte(landscape++);
    byte span4 = ((span1 >> 6) & 0x03) | ((span2 >> 4) & 0x0c) | ((span3 >> 2) & 0x30);
    white_span(span1 & 0x3f, x, y);
    black_span(span2 & 0x3f, x, y);
    white_span(span3 & 0x3f, x, y);
    black_span(span4, x, y);
  }

/*
  // render all pads in black in preparation for collision detection  
  for (int pad_num=0; pad_num<MAX_PADS; pad_num++)
  {
    byte x = pgm_read_byte(&pads[row][column][pad_num][0]);
    if (!x)
      break;
    byte y = pgm_read_byte(&pads[row][column][pad_num][1]);
    draw_pad(x, y, 0);
  }
  
  // do collision detection against the current backbuffer image
  
  // draw animated pads again using gray-scale frames
  int pad_frame = 1 + (gb.frameCount / 10) % 3;
  byte visited = pads_visited[row][column];
  for (int pad_num=0; pad_num<MAX_PADS; pad_num++, visited>>=1)
  {
    byte x = pgm_read_byte(&pads[row][column][pad_num][0]);
    if (!x)
      break;
    byte y = pgm_read_byte(&pads[row][column][pad_num][1]);
    if (!(visited & 1))
      draw_pad(x, y, pad_frame);
  }
  */
}

void white_span(byte span, byte & x, byte & y)
{
  y += span;
  while (y >= 48)
  {
    x++;
    y -= 48;
  }
}

void black_span(byte span, byte & x, byte & y)
{
  for (int i=0; i<span; i++)
  {
    gb.display.drawPixel(x, y);
    y++;
    if (y >= 48)
    {
      x++;
      y = 0;
    }
  }
}

/*
void land(int8_t column, int8_t row, int8_t pad_num)
{
  pads_visited[row][column] |= (1 << pad_num);
}
*/

