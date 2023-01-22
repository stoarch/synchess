using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;
using Texture2D = Raylib_CsLo.Texture;
using Rectangle = Raylib_CsLo.Rectangle;
using System.Numerics;

namespace SyncChess {

    public class Character
    {
        private Texture2D texture;
        private Vector2 position = new Vector2();
        private float rotation;
        private float scale;

        public Character(Texture2D texture, Vector2 position, float rotation, float scale)
        {
            this.texture = texture;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public void Draw(int gridX, int gridY, int cellWidth, int cellHeight)
        {
            DrawTextureEx(texture, new Vector2(gridX + position.X * cellWidth, gridY + position.Y * cellHeight), rotation, scale, WHITE);
        }

        public void Move(Vector2 newPosition)
        {
            position = newPosition;
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}
