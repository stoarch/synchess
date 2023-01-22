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
        private bool selected = false;
        private Texture2D selectedTexture;
        private int gridX;
        private int gridY;
        private int cellWidth;
        private int cellHeight;


        public Character(Texture2D texture, Texture2D selectedTexture, Vector2 position, float rotation, float scale)
        {
            this.texture = texture;
            this.selectedTexture = selectedTexture;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            selected = false;
        }

        public void SetGrid(int gridX, int gridY, int cellWidth, int cellHeight)
        {
            this.gridX = gridX;
            this.gridY = gridY;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
        }

        public void HandleInput(Vector2 mousePosition)
        {
            Rectangle rect = new Rectangle(this.gridX + position.X,
                    this.gridY + position.Y,
                    this.cellWidth,
                    this.cellHeight);

            if (IsMouseButtonPressed(MOUSE_LEFT_BUTTON)){
                if(CheckCollisionPointRec(mousePosition, rect))
                {
                    selected = true;
                }
                else
                {
                    selected = false;
                }
            }
        }

        public void Draw()
        {
            if(selected){
                DrawTextureEx(selectedTexture, new Vector2(gridX + position.X, gridY + position.Y), rotation, scale, WHITE);
            }else{
                DrawTextureEx(texture, new Vector2(gridX + position.X * cellWidth, gridY + position.Y * cellHeight), rotation, scale, WHITE);
            }
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
