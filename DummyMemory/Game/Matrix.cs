namespace DummyMemory.Game
{
    /// <summary>
    ///  Used to transform a model's vertices's from world-space to view-space.
    /// </summary>
    /// <remarks>Thank you to 0XDE57 on GitHub : https://github.com/0XDE57/AssaultCubeHack/blob/master/AssaultCubeHack/structs/ </remarks>
    public struct Matrix
    {
        /// <summary>
        /// DirectX : Usually Row-Major
        /// </summary>
        /// <remarks>OpenGL: Usually Column Major</remarks>
        public float m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44;

        /// <summary>
        /// Project a 3D position in world to a 2D position on the screen.
        /// </summary>
        /// <param name="worldPos">object's 3D position in world</param>
        /// <param name="width">screen width</param>
        /// <param name="height">screen height</param>
        /// <param name="screenPos">object's 2D position on screen</param>
        /// <returns>true if object is visible by screen, false otherwise</returns>
        public bool WorldToScreen(Vector3 worldPos, int width, int height, out Vector2 screenPos)
        {
            float screenX = this.m11 * worldPos.x + this.m21 * worldPos.y + this.m31 * worldPos.z + this.m41;
            float screenY = this.m12 * worldPos.x + this.m22 * worldPos.y + this.m32 * worldPos.z + this.m42;
            float screenW = this.m14 * worldPos.x + this.m24 * worldPos.y + this.m34 * worldPos.z + this.m44;
            float camX = (float)width / 2f;
            float camY = (float)height / 2f;
            float x = camX + (camX * screenX / screenW);
            float y = camY - (camY * screenY / screenW);
            screenPos = new Vector2(x, y);
            return screenW > 0.001f;
        }
    }
}
