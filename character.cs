using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;
using Texture2D = Raylib_CsLo.Texture;
using Rectangle = Raylib_CsLo.Rectangle;
using System.Numerics;

namespace SyncChess
{
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
        private bool mouseOver;
        private List<AStarNode> path;
        private float speed = 1.0f;

        public bool Selected => selected;
        public Vector2 Position => position;
        public bool MouseOver => mouseOver;
        public bool Moving { get; set; }

        public AStarNode CurrentPathNode
        {
            get
            {
                if (path?.Count > 0)
                {
                    return path[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public AStarNode NextPathNode
        {
            get
            {
                if (path?.Count > 1)
                {
                    return path[1];
                }
                else
                {
                    return null;
                }
            }
        }

        public Character(
            Texture2D texture,
            Texture2D selectedTexture,
            Vector2 position,
            float rotation,
            float scale
        )
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
            Rectangle rect = new Rectangle(
                this.gridX + position.X,
                this.gridY + position.Y,
                this.cellWidth,
                this.cellHeight
            );

            mouseOver = CheckCollisionPointRec(mousePosition, rect);

            if (IsMouseButtonPressed(MOUSE_LEFT_BUTTON))
            {
                if (mouseOver)
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
            //Draw flag that path is set
            if (path != null)
            {
                DrawRectangle(
                    (int)position.X + gridX,
                    (int)position.Y + gridY,
                    10,
                    10,
                    RED
                );
            }
            //Draw the character
            if (selected)
            {
                if (mouseOver)
                {
                    DrawTextureEx(
                        selectedTexture,
                        new Vector2(gridX + position.X, gridY + position.Y),
                        rotation,
                        scale,
                        YELLOW
                    );
                }
                else
                {
                    DrawTextureEx(
                        selectedTexture,
                        new Vector2(gridX + position.X, gridY + position.Y),
                        rotation,
                        scale,
                        WHITE
                    );
                }
            }
            else
            {
                DrawTextureEx(
                    texture,
                    new Vector2(gridX + position.X * cellWidth, gridY + position.Y * cellHeight),
                    rotation,
                    scale,
                    WHITE
                );
            }
        }

        public void Update(float dt)
        {
            //Display dt and Moving status
            DrawText(dt.ToString(), 10, 100, 20, WHITE);
            DrawText($"Moving: {Moving.ToString()}", 10, 120, 20, WHITE);
            //Display path information
            if (path != null)
            {
                DrawText($"Path Count: {path.Count}", 10, 140, 20, WHITE);
                if (path.Count > 0)
                {
                    DrawText($"Current Path Node: {path[0].X}, {path[0].Y}", 10, 160, 20, WHITE);
                }
                if (path.Count > 1)
                {
                    DrawText($"Next Path Node: {path[1].X}, {path[1].Y}", 10, 180, 20, WHITE);
                }
            }

            //If path present and we're moving, make one step
            if (Moving)
                if (path != null)
                {
                    if (path.Count > 0)
                    {
                        var target = path[0];
                        var targetPosition = new Vector2(target.X, target.Y);

                        var direction = Vector2.Normalize(targetPosition - position);
                        var distance = Vector2.Distance(targetPosition, position);

                        //Display direction and distance
                        DrawLineEx(
                            new Vector2(
                                gridX + position.X * cellWidth + cellWidth / 2,
                                gridY + position.Y * cellHeight + cellHeight / 2
                            ),
                            new Vector2(
                                gridX + targetPosition.X * cellWidth + cellWidth / 2,
                                gridY + targetPosition.Y * cellHeight + cellHeight / 2
                            ),
                            2,
                            RED
                        );
                        //Display position
                        DrawText($"Position: {position.X}, {position.Y}", 100, 10, 20, WHITE);
                        //Display last path nodes count
                        DrawText($"Path nodes: {path.Count}", 100, 40, 20, WHITE);

                        if (distance > 0.1f)
                        {
                            position += direction * speed * dt;
                        }
                        else
                        {
                            path.RemoveAt(0);

                            if (path.Count == 0)
                            {
                                Moving = false;
                            }

                            //Move character on last cell center
                            position = targetPosition;
                        }
                    }
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

        public List<AStarNode> GetPath()
        {
            return path;
        }
        public void SetPath(List<AStarNode> path)
        {
            this.path = path;
        }
    }
}
