using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyMemory.Game
{
    /// <summary>
    /// Describes a 3D world object
    /// </summary>
    public struct Vector3
    {

        /// <summary>
        /// Left - Right
        /// </summary>
        public float x;
        /// <summary>
        /// Forward - Backward
        /// </summary>
        public float z;
        /// <summary>
        /// Up - Down
        /// </summary>
        public float y;

        /// <summary>
        /// Create a position of 3D world object
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        #region Properities
        /// <summary>
        /// The length of this vector
        /// </summary>
        public float Magnitude => Length(this);
        /// <summary>
        /// This vector with a magnitude of 1
        /// </summary>
        public Vector3 Normalized => Normalize(this);
        #endregion

        #region Methods
        /// <summary>
        /// Measure the distance between two distances
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        /// <returns>Distance between two Positions.</returns>
        public static float Distance(Vector3 a, Vector3 b)
        {
            double x = Math.Pow(a.x - b.x, 2);
            double y = Math.Pow(a.y - b.y, 2);
            double z = Math.Pow(a.z - b.z, 2);

            return (float)Math.Sqrt(x + y + z);
        }
        /// <summary>
        /// Dot product of two <see cref="Vector3"/>s
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Dot(Vector3 a, Vector3 b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }

        /// <summary>
        /// Gets the length of the Vector3
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static float Length(Vector3 a)
        {
            double x = Math.Pow(a.x, 2) + Math.Pow(a.y, 2) + Math.Pow(a.z, 2);

            return (float)Math.Sqrt(x);
        }

        /// <summary>
        /// Measures the angles 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Angle(Vector3 a, Vector3 b)
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
        public static Vector3 Normalize(Vector3 a)
        {
            return a / Length(a);
        }

        /// <summary>
        /// Multiply <see cref="Vector3"/> by <see cref="Vector3"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 Scale(Vector3 a, Vector3 m)
        {
            return new Vector3(a.x * m.x, a.y * m.y, a.z * m.z);
        }

        /// <summary>
        /// Max components of Vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            float x = Math.Max(a.x, b.x);
            float y = Math.Max(a.y, b.y);
            float z = Math.Max(a.z, b.z);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Min components of Vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            float x = Math.Min(a.x, b.x);
            float y = Math.Min(a.y, b.y);
            float z = Math.Min(a.z, b.z);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Cross product of two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            float x = (a.y * b.z - a.z * b.y);
            float y = (a.z * b.x - a.x * b.z);
            float z = (a.x * b.y - a.y * b.x);

            return new Vector3(x, y, z);
        }

        #endregion

        #region Static Info
        /// <summary>
        /// 0, 0, 1
        /// </summary>
        public static readonly Vector3 forward = new Vector3(0, 0, 1);
        /// <summary>
        /// 0, 1, 0
        /// </summary>
        public static readonly Vector3 up = new Vector3(0, 1, 0);
        /// <summary>
        /// 1, 0, 0
        /// </summary>
        public static readonly Vector3 right = new Vector3(1, 0, 0);
        /// <summary>
        /// 0, 0, 0
        /// </summary>
        public static readonly Vector3 zero = new Vector3(0, 0, 0);
        /// <summary>
        /// 1, 1, 1
        /// </summary>
        public static readonly Vector3 one = new Vector3(1, 1, 1);
        #endregion

        #region Overrides

        /// <summary>
        /// Multiply <see cref="Vector3"/> by <see cref="Vector3"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 a, Vector3 m)
        {
            return Scale(a, m);
        }

        /// <summary>
        /// Divide <see cref="Vector3"/> by <see cref="Vector3"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 a, Vector3 m)
        {
            return new Vector3(a.x / m.x, a.y / m.y, a.z / m.z);
        }

        /// <summary>
        /// Add <see cref="Vector3"/> with <see cref="Vector3"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 a, Vector3 m)
        {
            return new Vector3(a.x + m.x, a.y + m.y, a.z + m.z);
        }

        /// <summary>
        /// Subtract <see cref="Vector3"/> with <see cref="Vector3"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 a, Vector3 m)
        {
            return new Vector3(a.x - m.x, a.y - m.y, a.z - m.z);
        }

        /// <summary>
        /// Multiply <see cref="Vector3"/> by <see cref="float"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 operator *(Vector3 a, float m)
        {

            return Scale(a, new Vector3(a.x * m, a.y * m, a.z * m));
        }

        /// <summary>
        /// Divide <see cref="Vector3"/> by <see cref="float"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 operator /(Vector3 a, float m)
        {
            return new Vector3(a.x / m, a.y / m, a.z / m);
        }

        /// <summary>
        /// Add <see cref="Vector3"/> with <see cref="float"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 operator +(Vector3 a, float m)
        {
            return new Vector3(a.x + m, a.y + m, a.z + m);
        }

        /// <summary>
        /// Subtract <see cref="Vector3"/> with <see cref="float"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 operator -(Vector3 a, float m)
        {
            return new Vector3(a.x - m, a.y - m, a.z - m);
        }

        /// <summary>
        /// Equal?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool operator ==(Vector3 a, Vector3 m)
        {
            return a.Equals(m);
        }

        /// <summary>
        /// Not equal?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool operator !=(Vector3 a, Vector3 m)
        {
            return !a.Equals(m);
        }

        /// <summary>
        /// Measures to see if X,Y,Z match. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <remarks>Only valid for <see cref="Vector3"/></remarks>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Vector3))
                return false;

            Vector3 a = (Vector3)obj;

            return this.x == a.x && this.y == a.y && this.z == a.z;
        }
        /// <summary>
        /// Format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.x}, {this.y}, {this.z}";
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
