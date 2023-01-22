﻿using System.Numerics;

using Raylib_CsLo;
using Raylib_CsLo.InternalHelpers;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;

using Texture2D = Raylib_CsLo.Texture;
using Rectangle = Raylib_CsLo.Rectangle;

using System;

namespace SyncChess
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

            var bluePos = new Vector2(0, 0);
            var redPos = new Vector2(7, 7);
            var blueCharacter = new Character(blueUnitTexture, bluePos, 0F, 0.8F);
            var redCharacter = new Character(redUnitTexture, redPos, -180F, 0.8F);


			while(!WindowShouldClose())
			{
				BeginDrawing();
					ClearBackground(SKYBLUE);
					DrawFPS(10,10);
					DrawText("SyncChess", 100, 10, 30, WHITE);

                    blueCharacter.Draw(GRID_X, GRID_Y, CELL_WIDTH, CELL_HEIGHT);
                    redCharacter.Draw(GRID_X, GRID_Y, CELL_WIDTH, CELL_HEIGHT);

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
