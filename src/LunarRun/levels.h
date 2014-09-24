#ifndef LEVELS_H
#define LEVELS_H

typedef const byte * section;

struct level
{
  const byte columns, rows;
  const section * sections;
  const float start_pos_x;
  const float start_pos_y;
};

extern level tutorial;
extern level level1;
extern level level2;
extern level level4;
extern level * current_level;

#endif LEVELS_H
