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

                    // Draw blue sprite at grid cell 1,1
                    DrawTextureRec(blueUnitTexture, new Rectangle(0, 0, 64, 64), new Vector2(64, 64), WHITE);

                    // Draw red sprite at grid cell 2,7
                    DrawTextureRec(redUnitTexture, new Rectangle(0, 0, 64, 64), new Vector2(128, 448), WHITE);

                    // Draw grid of lines
                    for (int i = 0; i < 8; i++)
                    {
                        DrawLine(100 + i * 100, 100, 100 + i * 100, 700, BLACK);
                        DrawLine(100, 100 + i * 100, 700, 100 + i * 100, BLACK);
                    }
				EndDrawing();
			}

            UnloadTexture(blueUnitTexture);
            UnloadTexture(redUnitTexture);

			CloseWindow();
		}
	}
}
