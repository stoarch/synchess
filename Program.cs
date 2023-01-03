using System.Numerics;

using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;

using Texture2D = Raylib_CsLo.Texture;
using Rectangle = Raylib_CsLo.Rectangle;

using System;

using static Raylib_CsLo.Raylib;

namespace BoomTris 
{
	public class Game
	{

        const int GRID_X = 64;
        const int GRID_Y = 64;
        const int CELL_WIDTH = 64;
        const int CELL_HEIGHT = 64;

		public static void Main(string[] args)
		{
			InitWindow(1280, 720, "BoomTris");
			SetTargetFPS(60);

            // load blue unit texture
            Texture2D blueUnitTexture = LoadTexture("resources/circle/blue.png");
            // load red unit texture
            Texture2D redUnitTexture = LoadTexture("resources/circle/red.png");

			while(!WindowShouldClose())
			{
				BeginDrawing();
					ClearBackground(SKYBLUE);
					DrawFPS(10,10);
					DrawText("SyncChess", 100, 10, 30, WHITE);

                    var bluePos = new Vector2(0, 0);
                    var redPos = new Vector2(512, 512);

                    // Draw blue sprite at grid cell 1,1
                    DrawTextureEx(blueUnitTexture, new Vector2(GRID_X + bluePos.X, GRID_Y + bluePos.Y), 0F, 0.8F, WHITE);

                    // Draw red sprite at grid cell 2,7
                    DrawTextureEx(redUnitTexture, new Vector2(GRID_X + redPos.X, GRID_Y + redPos.Y), -180F, 0.8F, WHITE);

                    // Draw grid of lines
                    for (int i = 0; i < 11; i++)
                    {
                        DrawLine(GRID_X + i * CELL_WIDTH, GRID_Y, GRID_X + i * CELL_WIDTH, 700, BLACK);
                        DrawLine(GRID_X, GRID_Y + i * CELL_HEIGHT, 700, GRID_Y + i * CELL_HEIGHT, BLACK);
                    }
				EndDrawing();
			}

            UnloadTexture(blueUnitTexture);
            UnloadTexture(redUnitTexture);

			CloseWindow();
		}
	}
}
