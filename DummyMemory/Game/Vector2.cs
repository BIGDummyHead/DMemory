using System;

namespace DummyMemory.Game
{
    /// <summary>
    /// Represents a 2D space in a game and or your screen.
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// 
        /// </summary>
        public float x, y;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Calculate the distance between a vector and another
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Distance(Vector2 a, Vector2 b)
        {
            double x = Math.Pow(a.x - b.x, 2);
            double y = Math.Pow(a.y - b.y, 2);

            return (float)Math.Sqrt(x + y);
        }

        /// <summary>
        /// Calculate the distance between this Vector and another
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public float Distance(Vector2 b) => Distance(this, b);

        /// <summary>
        /// Format this vector
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{x}, {y}";

        /// <summary>
        /// Points to 0,0
        /// </summary>
        public static Vector2 Zero => new Vector2(0, 0);
    }
}
