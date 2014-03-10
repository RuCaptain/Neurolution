using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Neurolution.GameWorld;
using Neurolution.Graphics;

namespace Neurolution.Helpers
{
    //Various utilites
    public static class Utils
    {
        private static long _ticksStart;
        private static long _ticksTotal;
        public static readonly Random Random = new Random();
        public static long MaxTicks;
        public static string MethodName = "";
        public static string MaxMethodName = "";


        //May be useful for calculating an execution time of specified slice of code.

        public static void StartTest()
        {
            _ticksStart = DateTime.Now.Ticks;
            var stackTrace = new StackTrace();
            MethodName = stackTrace.GetFrame(1).GetMethod().Name;
        }

        public static void EndTest()
        {
            _ticksTotal = DateTime.Now.Ticks - _ticksStart;
            if (_ticksTotal > MaxTicks)
            {
                MaxTicks = _ticksTotal;
                MaxMethodName = MethodName;
            }
        }

        public static Vector2 TextureOrigin(Sprite sprite)
        {
            return new Vector2(sprite.CurrentTexture().Width/2f, sprite.CurrentTexture().Height/2f);
        }

        //Converts angle to it's vector

        public static Vector2 GetDirection(float angle, float amplifier = 1)
        {
            var direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            if (direction.Length() > 0) direction.Normalize();
            direction = new Vector2(direction.X, direction.Y) * amplifier;

            return direction;
        }

        public static float GetDistance(Vector2 point1, Vector2 point2)
        {
            return (float)Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        public static float GetAimingAngle(Vector2 position, Vector2 target)
        {
            var direction = ConvertUnits.ToDisplayUnits(target - position);
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public static float RandomSpread(float value, float spread)
        {
            return RandomRange(value - spread, value + spread);
        }

        public static float RandomRange(float minValue, float maxValue)
        {
            return minValue + (float)Random.NextDouble() * (maxValue - minValue);
        }

        public static Vector2 TileCoordToRealCoord(Tile tile)
        {
            return new Vector2(
                (float)Math.Floor((double)tile.Height * (tile.X + tile.Y)),
                (float)Math.Floor((double)tile.Height / 2 * (tile.X - tile.Y))
                );
        }

        public static Vector2 TransformCoordinates(Vector2 point)
        {
            var angle = MathHelper.ToRadians(45);
            return new Vector2(
                point.X * (float)Math.Cos(angle) - point.Y * (float)Math.Sin(angle) - GameSettings.TileHeight/2,
                (point.X * (float)Math.Sin(angle) + point.Y * (float)Math.Cos(angle))/2f
                );
        }

        public static float RenderDepth(Vector2 point, float tileHeight)
        {
            var maxDistance = GetDistance(TransformCoordinates(new Vector2(GameSettings.Borders.Right, GameSettings.Borders.Bottom)),
                TransformCoordinates(new Vector2(GameSettings.Borders.Left, GameSettings.Borders.Top)));
            var distance = GetDistance(TransformCoordinates(new Vector2(GameSettings.Borders.Right, GameSettings.Borders.Bottom)),
                new Vector2(0, point.Y + tileHeight));
            return  (distance/maxDistance);
        }

        public static float Normalize(float number, float threshold)
        {
            var value = number;
            if(value > 0)
                while (value > threshold)
                    value -= threshold;
            else
                while (value < 0)
                    value += threshold;
            return value;
        }

        public static List<string> ListFilesRecursively(string path)
        {
            return ListFilesRecursively(path, new List<string>());
        }

        private static List<string> ListFilesRecursively(string path, List<string> files)
        {
            var dirFiles = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            foreach (var dir in dirs)
                ListFilesRecursively(dir, files);
            files.AddRange(dirFiles);

            return files;
        }


        public static bool CursorInArea(Point cursor, Rectangle bounds)
        {
            return
                cursor.X >= bounds.Left &&
                cursor.X <= bounds.Right &&
                cursor.Y >= bounds.Top &&
                cursor.Y <= bounds.Bottom;
        }


        public static float Range(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
