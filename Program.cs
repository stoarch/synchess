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
        { 0, 0, 9, 0, 0, 0, 0, 0 },
        { 0, 0, 9, 2, 3, 4, 0, 0 },
        { 0, 1, 2, 3, 7, 0, 0, 0 },
        { 0, 1, 2, 9, 9, 0, 0, 0 },
        { 0, 1, 1, 7, 9, 1, 1, 0 },
        { 0, 0, 6, 3, 2, 2, 1, 0 },
        { 0, 0, 2, 3, 2, 9, 1, 0 },
        { 0, 0, 3, 3, 2, 2, 1, 0 }
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

        var bluePos1 = new Vector2(0, 0);
        var bluePos2 = new Vector2(0, 1);
        var bluePos3 = new Vector2(1, 1);

        List<Character> blueCharacters = new List<Character> {
            new Character(
                blueUnitTexture,
                blueUnitSelectedTexture,
                bluePos1,
                0F,
                0.8F
            ),
            new Character(
                blueUnitTexture,
                blueUnitSelectedTexture,
                bluePos2,
                0F,
                0.8F
            ),
            new Character(
                blueUnitTexture,
                blueUnitSelectedTexture,
                bluePos3,
                0F,
                0.8F
            )
        };

        foreach(var character in blueCharacters)
        {
            character.SetGrid(GRID_X, GRID_Y, CELL_WIDTH, CELL_HEIGHT);
        }

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

            bool drawCellHighlighted = false;
            foreach(var blueCharacter in blueCharacters)
            {
                blueCharacter.HandleInput(mousePosition);

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
            }

            //Update scene//
            float dt = GetFrameTime();

            foreach(var blueCharacter in blueCharacters)
            {
                blueCharacter.Update(dt);
            }

            //Draw scene//
            BeginDrawing();

            ClearBackground(SKYBLUE);
            DrawFPS(10, 10);
            DrawText("SyncChess", 100, 10, 30, WHITE);


            //Draw legend of grid cell weights
            for (int i = 0; i < 10; i++)
            {
                DrawRectangle(
                    700,
                    50 + i * 50,
                    30,
                    30,
                    new Color(128 + i*20, 128 + i*20, 128 + i*20, 255)
                );
                DrawText(
                    i.ToString(),
                    710,
                    50 + 10 + i * 50,
                    20,
                    new Color(0, 0, 0, 255)
                );
            }


            //Draw path cost
            if (path != null && path.Count > 0)
            {
                int pathCost = path.Sum(node => grid[node.X, node.Y]);

                DrawText(
                    "Path cost: " + pathCost,
                    700,
                    600,
                    20,
                    new Color(0, 0, 0, 255)
                );
            }

            DrawGrid();

            //Draw chars
            foreach(var blueCharacter in blueCharacters)
            {
                blueCharacter.Draw();
            }
            redCharacter.Draw();

            //Draw cell highlighted
            if (drawCellHighlighted)
            {
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

                switch(grid[x, y]) {
                case 1: {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        new Color(128, 128, 128, 255)
                    );
                    break;
                }
                case 2: {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        new Color(148, 148, 148, 255)
                    );
                    break;
                }
                case 3: {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        new Color(168, 168, 168, 255)
                    );
                    break;
                }
                case 4: {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        new Color(188, 188, 188, 255)
                    );
                    break;
                }
                case 5: {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        new Color(208, 208, 208, 255)
                    );
                    break;
                }
                case 6: {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        new Color(228, 228, 228, 255)
                    );
                    break;
                }
                case 7: {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        new Color(248, 248, 248, 255)
                    );
                    break;
                }
                case 8: {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        new Color(255, 255, 255, 255)
                    );
                    break;
                }
                case 9:
                {
                    DrawRectangle(
                        GRID_X + x * CELL_WIDTH,
                        GRID_Y + y * CELL_HEIGHT,
                        CELL_WIDTH,
                        CELL_HEIGHT,
                        BLACK
                    );
                    break;
                }
                }

                //Draw cell weight
                DrawText(
                    grid[x, y].ToString(),
                    GRID_X + x * CELL_WIDTH + 10,
                    GRID_Y + y * CELL_HEIGHT + 10,
                    20,
                    new Color(0, 0, 0, 255)
                );
            }
        }
    }
} //Game
}
