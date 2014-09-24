class vector2
{
   public:
     float x, y;
     
     inline vector2() : x(0), y() {}
     inline vector2(const float _x, const float _y) : x(_x), y(_y) {}
     
     inline friend vector2 operator +(const vector2 &v1, const vector2 &v2) {return vector2(v1.x+v2.x, v1.y+v2.y); }
     inline friend vector2 operator -(const vector2 &v1, const vector2 &v2) {return vector2(v1.x-v2.x, v1.y-v2.y); }
     inline friend float   operator *(const vector2 &v1, const vector2 &v2) {return v1.x*v2.x + v1.y*v2.y; }  // dot product
     inline friend vector2 operator *(const vector2 &v, const float s) {return vector2(v.x*s, v.y*s); }
     inline friend vector2 operator *(const float s, const vector2 &v) {return vector2(v.x*s, v.y*s); }
     inline friend vector2 operator /(const vector2 &v, const float s) {return vector2(v.x/s, v.y/s); }
     inline void operator +=(const vector2 &v) {this->x+=v.x; this->y+=v.y; }
     inline void operator -=(const vector2 &v) {this->x-=v.x; this->y-=v.y; }
     inline void operator *=(const float s) {this->x*=s; this->y*=s; }
     
     inline float length_squared() {return *this * *this; }
     inline float length() {return sqrt(this->length_squared()); }
     inline vector2 normalize() {return *this / this->length(); }
};

