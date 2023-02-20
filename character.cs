using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;
using Texture2D = Raylib_CsLo.Texture;
using Rectangle = Raylib_CsLo.Rectangle;
using System.Numerics;
using static Raylib_CsLo.TraceLogLevel;

namespace SyncChess
{
    public class Character
    {
        private static int uid = 0;
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
        private int[,] grid;

        public int Id { get; private set; }

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
            float scale,
            int[,] grid
        )
        {
            Id = uid;
            uid += 1;

            this.texture = texture;
            this.selectedTexture = selectedTexture;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            selected = false;
            this.grid = grid;
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
                this.gridX + position.X * cellWidth,
                this.gridY + position.Y * cellHeight,
                this.cellWidth,
                this.cellHeight
            );

            mouseOver = CheckCollisionPointRec(mousePosition, rect);

            //Display mouse over status
            if (mouseOver)
            {
                TraceLog(LOG_INFO, "Mouse over character");
                TraceLog(LOG_INFO, "Mouse position: " + mousePosition);
                TraceLog(LOG_INFO, "Character position: " + position);
                TraceLog(
                    LOG_INFO,
                    "Character rect: "
                        + rect.X
                        + ", "
                        + rect.Y
                        + ", "
                        + rect.width
                        + ", "
                        + rect.height
                );
            }

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
                DrawRectangle((int)position.X + gridX, (int)position.Y + gridY, 10, 10, RED);
            }
            //Draw the character
            if (selected)
            {
                if (mouseOver)
                {
                    //Draw rotated on center origin
                    DrawTexturePro(
                        selectedTexture,
                        new Rectangle(0, 0, selectedTexture.width, selectedTexture.height),
                        new Rectangle(
                            position.X * cellWidth + gridX + (cellWidth / 2),
                            position.Y * cellHeight + gridY + (cellHeight / 2),
                            cellWidth,
                            cellHeight
                        ),
                        new Vector2(cellWidth / 2, cellHeight / 2),
                        rotation,
                        YELLOW
                    );
                }
                else
                {
                    //Draw rotate on center origin
                    DrawTexturePro(
                        selectedTexture,
                        new Rectangle(0, 0, selectedTexture.width, selectedTexture.height),
                        new Rectangle(
                            position.X * cellWidth + gridX + (cellWidth / 2),
                            position.Y * cellHeight + gridY + (cellHeight / 2),
                            cellWidth,
                            cellHeight
                        ),
                        new Vector2(cellWidth / 2, cellHeight / 2),
                        rotation,
                        BLUE
                    );
                }
            }
            else
            {
                //Draw rotate on origin center
                DrawTexturePro(
                    texture,
                    new Rectangle(0, 0, texture.width, texture.height),
                    new Rectangle(
                        position.X * cellWidth + gridX + (cellWidth / 2),
                        position.Y * cellHeight + gridY + (cellHeight / 2),
                        cellWidth,
                        cellHeight
                    ),
                    new Vector2(cellWidth / 2, cellHeight / 2),
                    rotation,
                    WHITE
                );
            }
        }

        public void Update(float dt)
        {
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
                        if (distance > 0.1f)
                        {
                            var terrainSpeed = Math.Max(grid[target.X, target.Y], 1);
                            position += direction * speed / terrainSpeed * dt;
                            //Rotate towards next node gradually
                            rotation = Lerp(rotation, Rad2Deg(Vector2ToAngle(direction)), 0.1f);
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

        private float Vector2ToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        private float Rad2Deg(float rad)
        {
            return rad * (180.0f / (float)Math.PI);
        }

        public String toString()
        {
            return "Character: " + Id + " Position: " + position;
        }
    }
}
