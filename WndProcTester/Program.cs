// Copyright (C) 2017 Julian Bosch
// See the file LICENSE for copying permission.

using System;
using System.Linq;
using System.Text;
using MyMemory;

namespace WndProcTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var l_Process = System.Diagnostics.Process.GetProcessesByName("Wow").First();
            Console.WriteLine($"Using process : {l_Process.Id}, window {l_Process.MainWindowHandle}");

            using (RemoteProcess l_RemoteProcess = new RemoteProcess((uint)l_Process.Id))
                using (var l_WndProcExecutor = new WndProcExecutor(l_RemoteProcess, l_Process.MainWindowHandle))
                {
                    Console.WriteLine("Everything is ready, press any key to execute simple lua code in the remote process :)");
                    Console.ReadKey();
                
                    LuaTest(l_RemoteProcess, l_WndProcExecutor, "print(\"Hello world !\")");

                    Console.WriteLine("Done :) Press any key to cleanup.");
                    Console.ReadKey();
                }
        }

        /// <summary>
        /// Example that call FrameScript__ExecuteBuffer
        /// </summary>
        /// <param name="p_Process">The remote process</param>
        /// <param name="p_Executor">The WndProc executor to use</param>
        /// <param name="p_Lua">The actual LUA code</param>
        static void LuaTest(RemoteProcess p_Process, WndProcExecutor p_Executor, string p_Lua)
        {
            var l_FrameScript__ExecuteBuffer = p_Process.ModulesManager.MainModule.BaseAddress + 0xB2E28;
            var l_LuaBufferUTF8 = Encoding.UTF8.GetBytes(p_Lua);

            using (var l_RemoteBuffer = p_Process.MemoryManager.AllocateMemory((uint) l_LuaBufferUTF8.Length + 1))
            {
                l_RemoteBuffer.WriteBytes(l_LuaBufferUTF8);

                var l_Mnemonics = new string[]
                {
                    "push 0",
                    $"push {l_RemoteBuffer.Pointer}",
                    $"push {l_RemoteBuffer.Pointer}",
                    $"call {l_FrameScript__ExecuteBuffer}",
                    "add esp, 0xC",
                    "retn"
                };

                p_Executor.Call(l_Mnemonics);
            }
        }
    }
}
