using System.Numerics;

using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;

using Texture2D = Raylib_CsLo.Texture;
using Rectangle = Raylib_CsLo.Rectangle;

using System;
using System.Drawing;

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
        { 0, 0, 0, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
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

        while (!WindowShouldClose())
        {
            //Check input//
            mousePosition = GetMousePosition();
            var mouseGridPos = new Vector2(
                (int)(mousePosition.X - GRID_X) / CELL_WIDTH,
                (int)(mousePosition.Y - GRID_Y) / CELL_HEIGHT
            );

            blueCharacter.HandleInput(mousePosition);

            //Update scene//
            bool drawCellHighlighted = false;
            if (blueCharacter.Selected && !blueCharacter.MouseOver)
            {
                drawCellHighlighted = true;

                //Calculate path towards mouse grid cell
                AStar pathfinder = new AStar();

                path = pathfinder.GetPath(
                           grid,
                           (int)blueCharacter.Position.X, (int)blueCharacter.Position.Y,
                           (int)mouseGridPos.X, (int)mouseGridPos.Y
                       );
            }

            //Draw scene//
            BeginDrawing();
            ClearBackground(SKYBLUE);
            DrawFPS(10, 10);
            DrawText("SyncChess", 100, 10, 30, WHITE);

            blueCharacter.Draw();
            redCharacter.Draw();



            if (drawCellHighlighted)
            {
                //Draw path
                if(path != null)
                    foreach (var node in path)
                    {
                        DrawTexture(
                            cellHighlightedTexture,
                            GRID_X + node.X * CELL_WIDTH,
                            GRID_Y + node.Y * CELL_HEIGHT,
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

            DrawGrid();

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
    }
} //Game
}
