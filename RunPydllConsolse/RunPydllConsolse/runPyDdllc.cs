using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RunPydllConsolse
{
    static class runPyDdllc
    {

        const string dllPath = @"..\..\..\..\..\cpydll\x64\Release\cpydll.dll";


        [DllImport(dllPath, EntryPoint = "pyInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int pyInit(IntPtr path, IntPtr inByte, int inLen, IntPtr outByte, int outLen);
        [DllImport(dllPath, EntryPoint = "pyDeinit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int pyDeinit();
        [DllImport(dllPath, EntryPoint = "PyImportModule", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr PyImportModule(string s);
        [DllImport(dllPath, EntryPoint = "PyRunString", CallingConvention = CallingConvention.Cdecl)]
        public static extern int PyRunString(string s);

        static public byte[] pypath = System.Text.Encoding.Unicode.GetBytes("D:\\Users\\59912\\anaconda3\\envs\\tf2\0");
        static public byte[] inByte = new byte[3648 * 5472 * 3];

        //static public byte[] outByte = new byte[101 * 4];
        static public float[] outByte = new float[101*4];

        static public int init()
        {
            IntPtr pData0 = Marshal.UnsafeAddrOfPinnedArrayElement(pypath, 0);
            IntPtr pData1 = Marshal.UnsafeAddrOfPinnedArrayElement(inByte, 0);
            IntPtr pData2 = Marshal.UnsafeAddrOfPinnedArrayElement(outByte, 0);
            pyInit(pData0, pData1, inByte.Length, pData2, outByte.Length * 4);

            PyRunString("import baseDll");
            return 0;
        }

        static public int deinit()
        {
            pyDeinit();
            return 0;
        }


        static public float[] run(byte[] imgByte)
        {
            if (imgByte.Length != inByte.Length)
                return null;
            imgByte.CopyTo(inByte, 0);

            PyRunString("baseDll.startRunModle()");

            float[] copy = new float[outByte.Length];
            outByte.CopyTo(copy, 0);
            return copy;
        }


        //static public int runTest()
        //{
        //    IntPtr pData0 = Marshal.UnsafeAddrOfPinnedArrayElement(pypath, 0);
        //    IntPtr pData1 = Marshal.UnsafeAddrOfPinnedArrayElement(inByte, 0);
        //    IntPtr pData2 = Marshal.UnsafeAddrOfPinnedArrayElement(outByte, 0);
        //    pyInit(pData0, pData1, inByte.Length, pData2, outByte.Length*4);

        //    PyRunString("print('123')");
        //    PyRunString("import baseDll");

        //    //inByte[0] = 3;
        //    //PyRunString("baseDll.test()");
        //    //Console.WriteLine("\r\n outByte[0]:" + outByte[0].ToString());

        //    Stopwatch sw = new Stopwatch();
        //    for (int i = 0; i < 10; i++)
        //    {
        //        sw.Restart();
        //        PyRunString("baseDll.startRunModle()");
        //        sw.Stop();
        //        TimeSpan ts = sw.Elapsed;
        //        Console.WriteLine("DateTime costed for Shuffle function is: {0}ms", ts.TotalMilliseconds);
        //    }


        //    for (int i = 0; i < 10; i++)
        //    {
        //        Console.WriteLine("outByte["+i.ToString()+"]:" + outByte[i].ToString());
        //    }

        //    pyDeinit();
        //    return 0;
        //}
    }
}


