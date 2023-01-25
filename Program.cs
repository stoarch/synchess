using System.Numerics;

using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;

using Texture2D = Raylib_CsLo.Texture;
using Rectangle = Raylib_CsLo.Rectangle;
using Color = Raylib_CsLo.Color;

using System;
using System.Drawing;

using Raylib_CsLo.InternalHelpers;
using static Raylib_CsLo.TraceLogLevel;

namespace SyncChess
{
    public class Game
    {
        const int GRID_X = 64;
        const int GRID_Y = 64;
        const int CELL_WIDTH = 64;
        const int CELL_HEIGHT = 64;

        private static int[,] grid =
        {
            { 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public static void Main(string[] args)
        {
            InitWindow(1280, 720, "BoomTris");
            SetTargetFPS(60);

            // load blue unit texture
            Texture2D blueUnitTexture = LoadTexture("resources/circle/blue.png");
            Texture2D blueUnitSelectedTexture = LoadTexture("resources/square/blue.png");
            // load red unit texture
            Texture2D redUnitTexture = LoadTexture("resources/circle/red.png");
            Texture2D redUnitSelectedTexture = LoadTexture("resources/square/red.png");

            // cell highlighted texture
            Texture2D cellHighlightedTexture = LoadTexture("resources/tile/wall-1111.png");

            var bluePos = new Vector2(0, 0);
            var redPos = new Vector2(7, 7);
            var blueCharacter = new Character(
                blueUnitTexture,
                blueUnitSelectedTexture,
                bluePos,
                0F,
                0.8F
            );
            blueCharacter.SetGrid(GRID_X, GRID_Y, CELL_WIDTH, CELL_HEIGHT);

            var redCharacter = new Character(
                redUnitTexture,
                redUnitSelectedTexture,
                redPos,
                -180F,
                0.8F
            );
            redCharacter.SetGrid(GRID_X, GRID_Y, CELL_WIDTH, CELL_HEIGHT);

            var mousePosition = new Vector2();
            List<AStarNode> path = new List<AStarNode>();

            float debugFadeTimeout = 0F;
            float pathDebugFadeTimeout = 0F;

            while (!WindowShouldClose())
            {
                //Check input//
                mousePosition = GetMousePosition();
                var mouseGridPos = new Vector2(
                    (int)(mousePosition.X - GRID_X) / CELL_WIDTH,
                    (int)(mousePosition.Y - GRID_Y) / CELL_HEIGHT
                );

                bool leftPressed = IsMouseButtonPressed(MOUSE_LEFT_BUTTON);

                blueCharacter.HandleInput(mousePosition);

                bool drawCellHighlighted = false;
                if (blueCharacter.Selected && !blueCharacter.MouseOver)
                {
                    drawCellHighlighted = true;

                    //Calculate path towards mouse grid cell
                    AStar pathfinder = new AStar();

                    path = pathfinder.GetPath(
                        grid,
                        (int)blueCharacter.Position.X,
                        (int)blueCharacter.Position.Y,
                        (int)mouseGridPos.X,
                        (int)mouseGridPos.Y
                    );

                    if (path == null)
                        TraceLog(
                            LOG_INFO,
                            "Path undefined from " + blueCharacter.Position + " to " + mouseGridPos
                        );
                }

                //If mouse clicked on cell, move Character
                if (leftPressed && path != null && path.Count > 0)
                {
                    //Display path information fading 1s
                    pathDebugFadeTimeout = 1F;

                    //Log path information
                    TraceLog(3, "Path setting:" + path.Count);

                    if (path.Count > 0)
                    {
                        TraceLog(3, "Path set to char:" + path.Count);

                        blueCharacter.SetPath(path);
                        blueCharacter.Moving = true;
                    }
                }

                //Update scene//
                float dt = GetFrameTime();

                blueCharacter.Update(dt);

                //Draw scene//
                BeginDrawing();
                ClearBackground(SKYBLUE);
                DrawFPS(10, 10);
                DrawText("SyncChess", 100, 10, 30, WHITE);

                blueCharacter.Draw();
                redCharacter.Draw();

                DrawGrid();

                if (drawCellHighlighted)
                {
                    DrawText($"Char path: {blueCharacter.GetPath()}", 10, 310, 20, WHITE);
                    if (path != null)
                    {
                        DrawText("Path length: " + path.Count, 10, 350, 20, WHITE);
                        DrawText($"Path:{path}", 10, 370, 20, WHITE);

                        if ((path?.Count > 0) && (path[path.Count - 1] != null))
                            DrawText("Path cost: " + path[path.Count - 1].G, 10, 390, 20, WHITE);
                    }

                    //Draw path
                    if (path != null)
                        foreach (var node in path)
                        {
                            DrawTexture(
                                cellHighlightedTexture,
                                GRID_X
                                    + node.X * CELL_WIDTH
                                    + CELL_WIDTH / 2
                                    - cellHighlightedTexture.width / 2,
                                GRID_Y
                                    + node.Y * CELL_HEIGHT
                                    + CELL_HEIGHT / 2
                                    - cellHighlightedTexture.height / 2,
                                WHITE
                            );

                            //Draw path text position
                            DrawText(
                                node.X + "," + node.Y,
                                GRID_X + node.X * CELL_WIDTH + CELL_WIDTH - 30,
                                GRID_Y + node.Y * CELL_HEIGHT + CELL_HEIGHT - 20,
                                20,
                                WHITE
                            );
                        }

                    //Draw highlighted texture over grid cell
                    DrawTextureEx(
                        cellHighlightedTexture,
                        new Vector2(
                            GRID_X + (CELL_WIDTH * (int)mouseGridPos.X),
                            GRID_Y + (CELL_HEIGHT * (int)mouseGridPos.Y)
                        ),
                        0,
                        CELL_WIDTH / cellHighlightedTexture.width,
                        WHITE
                    );
                }

                //Draw debug info

                //Display mouse pressed info
                //Fade out after 1 second
                if (debugFadeTimeout > 0)
                {
                    //Color fade out by delta time
                    Color fadeColor = new Color(
                        (byte)255,
                        (byte)255,
                        (byte)255,
                        (byte)(255 * debugFadeTimeout)
                    );

                    DrawText("Mouse pressed", 10, 250, 20, fadeColor);
                    debugFadeTimeout -= dt;

                    //Draw path info
                    if (pathDebugFadeTimeout > 0)
                    {
                        //Color fade out by delta time
                        Color fadePathColor = new Color(
                            (byte)255,
                            (byte)255,
                            (byte)255,
                            (byte)(255 * pathDebugFadeTimeout)
                        );

                        DrawText("Path length: " + path.Count, 10, 350, 20, fadePathColor);
                        if (path.Count > 0)
                            DrawText(
                                "Path cost: " + path[path.Count - 1].G,
                                10,
                                370,
                                20,
                                fadePathColor
                            );

                        pathDebugFadeTimeout -= dt;
                    }
                }
                //If first time pressed start fading
                else if (leftPressed)
                {
                    debugFadeTimeout = 1F;
                }

                //Draw character movement and status
                DrawText("Character movement: " + blueCharacter.Moving, 10, 50, 20, WHITE);
                DrawText("Character selected: " + blueCharacter.Selected, 10, 70, 20, WHITE);
                //Show current path node for character
                if (blueCharacter.CurrentPathNode != null)
                {
                    DrawText(
                        "Current path node: "
                            + blueCharacter.CurrentPathNode.X
                            + ","
                            + blueCharacter.CurrentPathNode.Y,
                        10,
                        90,
                        20,
                        WHITE
                    );
                }
                //Show next path node for character
                if (blueCharacter.NextPathNode != null)
                {
                    DrawText(
                        "Next path node: "
                            + blueCharacter.NextPathNode.X
                            + ","
                            + blueCharacter.NextPathNode.Y,
                        10,
                        110,
                        20,
                        WHITE
                    );
                }
                //Show character position in grid coord
                DrawText(
                    "Character pos: " + blueCharacter.Position.X + "," + blueCharacter.Position.Y,
                    300,
                    150,
                    20,
                    WHITE
                );

                //Show character pixel perfect position
                var charPos = new Vector2(
                    GRID_X + blueCharacter.Position.X * CELL_WIDTH + CELL_WIDTH / 2,
                    GRID_Y + blueCharacter.Position.Y * CELL_HEIGHT + CELL_HEIGHT / 2
                );
                DrawText(
                    "Character pixel pos: " + charPos.X + "," + charPos.Y,
                    300,
                    170,
                    20,
                    WHITE
                );

                EndDrawing();
            }

            UnloadTexture(blueUnitTexture);
            UnloadTexture(redUnitTexture);
            UnloadTexture(blueUnitSelectedTexture);
            UnloadTexture(redUnitSelectedTexture);

            CloseWindow();
        }

        private static void DrawGrid()
        {
            for (int i = 0; i <= 8; i++)
            {
                DrawLine(
                    GRID_X,
                    GRID_Y + i * CELL_HEIGHT,
                    GRID_X + 8 * CELL_WIDTH,
                    GRID_Y + i * CELL_HEIGHT,
                    BLACK
                );
                DrawLine(
                    GRID_X + i * CELL_WIDTH,
                    GRID_Y,
                    GRID_X + i * CELL_WIDTH,
                    GRID_Y + 8 * CELL_HEIGHT,
                    BLACK
                );
            }

            //Draw blocker cells
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    if (grid[x, y] == 1)
                    {
                        DrawRectangle(
                            GRID_X + x * CELL_WIDTH,
                            GRID_Y + y * CELL_HEIGHT,
                            CELL_WIDTH,
                            CELL_HEIGHT,
                            BLACK
                        );
                    }
                }
            }
        }
    } //Game
}
