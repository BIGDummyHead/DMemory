using System;

namespace DummyMemory.Game
{
    /// <summary>
    /// Describes a 3D world object
    /// </summary>
    public struct Vector2
    {

        /// <summary>
        /// Left - Right
        /// </summary>
        public float x;
        /// <summary>
        /// Up - Down
        /// </summary>
        public float y;

        /// <summary>
        /// Create a position of 3D world object
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        #region Properities
        /// <summary>
        /// The length of this vector
        /// </summary>
        public float Magnitude => Length(this);
        /// <summary>
        /// This vector with a magnitude of 1
        /// </summary>
        public Vector2 Normalized => Normalize(this);
        #endregion

        #region Methods
        /// <summary>
        /// Measure the distance between two distances
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <returns>Distance between two Positions.</returns>
        public static float Distance(Vector2 a, Vector2 b)
        {
            double x = Math.Pow(a.x - b.x, 2);
            double y = Math.Pow(a.y - b.y, 2);

            return (float)Math.Sqrt(x + y);
        }
        /// <summary>
        /// Dot product of two <see cref="Vector2"/>s
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Dot(Vector2 a, Vector2 b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }

        /// <summary>
        /// Gets the length of the Vector3
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static float Length(Vector2 a)
        {
            double x = Math.Pow(a.x, 2) + Math.Pow(a.y, 2);

            return (float)Math.Sqrt(x);
        }

        /// <summary>
        /// Measures the angles 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Angle(Vector2 a, Vector2 b)
        {
            float dot = Dot(a, b);

            float _a = Length(a);
            float _b = Length(b);

            return dot / (_a * _b);
        }

        /// <summary>
        /// Normalizes a vector
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2 Normalize(Vector2 a)
        {
            return a / Length(a);
        }

        /// <summary>
        /// Multiply <see cref="Vector2"/> by <see cref="Vector2"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 Scale(Vector2 a, Vector2 m)
        {
            return new Vector2(a.x * m.x, a.y * m.y);
        }

        /// <summary>
        /// Max components of Vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            float x = Math.Max(a.x, b.x);
            float y = Math.Max(a.y, b.y);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Min components of Vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            float x = Math.Min(a.x, b.x);
            float y = Math.Min(a.y, b.y);

            return new Vector2(x, y);
        }

        #endregion

        #region Static Info
        /// <summary>
        /// 0, 1
        /// </summary>
        public static readonly Vector2 up = new Vector2(0, 1);
        /// <summary>
        /// 1, 0
        /// </summary>
        public static readonly Vector2 right = new Vector2(1, 0);
        /// <summary>
        /// 0, 0
        /// </summary>
        public static readonly Vector2 zero = new Vector2(0, 0);
        /// <summary>
        /// 1, 1
        /// </summary>
        public static readonly Vector2 one = new Vector2(1, 1);
        #endregion

        #region Overrides

        /// <summary>
        /// Multiply <see cref="Vector2"/> by <see cref="Vector2"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 a, Vector2 m)
        {
            return Scale(a, m);
        }

        /// <summary>
        /// Divide <see cref="Vector2"/> by <see cref="Vector2"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 a, Vector2 m)
        {
            return new Vector2(a.x / m.x, a.y / m.y);
        }

        /// <summary>
        /// Add <see cref="Vector2"/> with <see cref="Vector2"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 operator +(Vector2 a, Vector2 m)
        {
            return new Vector2(a.x + m.x, a.y + m.y);
        }

        /// <summary>
        /// Subtract <see cref="Vector2"/> with <see cref="Vector2"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 a, Vector2 m)
        {
            return new Vector2(a.x - m.x, a.y - m.y);
        }

        /// <summary>
        /// Multiply <see cref="Vector2"/> by <see cref="float"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 a, float m)
        {

            return Scale(a, new Vector2(a.x * m, a.y * m));
        }

        /// <summary>
        /// Divide <see cref="Vector2"/> by <see cref="float"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 a, float m)
        {
            return new Vector2(a.x / m, a.y / m);
        }

        /// <summary>
        /// Add <see cref="Vector2"/> with <see cref="float"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 operator +(Vector2 a, float m)
        {
            return new Vector2(a.x + m, a.y + m);
        }

        /// <summary>
        /// Subtract <see cref="Vector2"/> with <see cref="float"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 a, float m)
        {
            return new Vector2(a.x - m, a.y - m);
        }

        /// <summary>
        /// Equal?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool operator ==(Vector2 a, Vector2 m)
        {
            return a.Equals(m);
        }

        /// <summary>
        /// Not equal?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool operator !=(Vector2 a, Vector2 m)
        {
            return !a.Equals(m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        public static implicit operator Vector3(Vector2 a)
        {
            return new Vector3(a.x, a.y, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        public static implicit operator Vector2(Vector3 a)
        {
            return new Vector2(a.x, a.y);
        }

        /// <summary>
        /// Measures to see if X,Y,Z match. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>Only valid for <see cref="Vector2"/></remarks>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Vector2))
                return false;

            Vector2 a = (Vector2)obj;

            return this.x == a.x && this.y == a.y;
        }
        /// <summary>
        /// Format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.x}, {this.y}";
        }

        /// <summary>
        /// Get Hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
