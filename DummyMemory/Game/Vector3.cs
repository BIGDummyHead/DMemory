using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyMemory.Game
{
    /// <summary>
    /// Used for 3D space, XYZ cords
    /// </summary>
    public struct Vector3
    {
        /// <summary>
        /// 
        /// </summary>
        public float x, y, z;

        /// <summary>
        /// 
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

        /// <summary>
        /// Calculate the distance between two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Distance between to in world Objects</returns>
        public static float Distance(Vector3 a, Vector3 b)
        {
            double x = Math.Pow( a.x - b.x, 2);
            double y = Math.Pow(a.y - b.y, 2);
            double z = Math.Pow(a.z - b.z, 2);

            return (float)Math.Sqrt(x + y + z);
        }

        /// <summary>
        /// Calculates the distance between this Vector and another.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public float Distance(Vector3 b) => Distance(this, b);

        /// <summary>
        /// Formats the Vector
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{x}, {y}, {z}";

        /// <summary>
        /// Points to 0,0,0
        /// </summary>
        public static Vector3 Zero => new Vector3(0, 0, 0);
    }
}
