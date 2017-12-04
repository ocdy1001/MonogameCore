﻿using System;
using System.Collections.Generic;

namespace Core
{
    public static class Debug
    {
        private static float timer;
        private static bool inDebug = false;

        internal static int dynamicObjects;
        internal static int staticObjects;
        
        public static float printInterval = 1.0f;
        public static bool printErrors = true;
        public static bool drawLines = false;
        public static bool printData = false;
        public static bool printNotifications = false;
        
        public static void ReleaseMode()
        {
            inDebug = false;
            printErrors = false;
            drawLines = false;
            printData = false;
            printNotifications = false;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Engine] :: Release Mode activated!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void FullDebugMode()
        {
            inDebug = true;
            printErrors = true;
            drawLines = true;
            printData = true;
            printNotifications = true;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Engine] :: Debug Mode activated!");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void SwitchMode()
        {
            if (inDebug) ReleaseMode();
            else FullDebugMode();
        }

        internal static void PrintError(string error)
        {
            if (!printErrors) return;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] :: " + error);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void PrintDebug<T>(string msg, T val)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[Debug] :: " + msg);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(val);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void PrintDebug(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Debug] :: " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void PrintNotification(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Notification] :: " + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void PrintNotification<T>(string msg, T val)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[Notification] :: " + msg);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(val);
            Console.ForegroundColor = ConsoleColor.White;
        }

        internal static void Update(float time)
        {
            timer += time;
            if (timer >= printInterval)
                timer = 0.0f;
            else return;
            if (printData)
            {
                PrintDebug("FPS: ", Time.Fps);
                PrintDebug("Dynamics: ", dynamicObjects);
                PrintDebug("Statics: ", staticObjects);
            }
        }
    }
}