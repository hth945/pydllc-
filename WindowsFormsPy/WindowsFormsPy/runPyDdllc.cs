using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsPy
{
    public struct QueueData
    {
        public string cmd;
        public string id;
        public object o;
    }

    static class runPyDdllc
    {
        static bool isrun = true;
        static Thread mThread;
        public static ConcurrentQueue<QueueData> inRecQueue = new ConcurrentQueue<QueueData>();
        public static ConcurrentQueue<QueueData> outRecQueue = new ConcurrentQueue<QueueData>();

        static byte[] fifoEn = new byte[20];
        static byte[][] fifoByte = new byte[fifoEn.Length][];

        static public int init()
        {
            if (mThread == null)
            {
                for (int i = 0; i < fifoByte.Length; i++)
                {
                    fifoByte[i] = new byte[3648 * 5472 * 3];
                }

                mThread = new Thread(() => // Lambda 表达式
                {
                    runPyDdllcunsafe.pyinit();

                    threadRun();
                    runPyDdllcunsafe.pyDeinit();
                    
                });
                mThread.Start();  // 开始
            }
            return 0;
        }

        static int threadRun()
        {
            QueueData b;

            while (isrun)
            {
                if (inRecQueue.TryDequeue(out b)) //清空队列
                {
                    if (b.cmd == "compute")  //串口来的数据
                    {
                        Console.WriteLine("in run :"+ b.id + "  " + (int)b.o);
                        float[] copy = new float[400];
                        float[] outData = runPyDdllcunsafe.run(fifoByte[(int)b.o], copy);
                        fifoEn[(int)b.o] = 0;
                        QueueData mpd = new QueueData();
                        mpd.cmd = "computeOK";
                        mpd.id = b.id;
                        mpd.o = outData;
                        outRecQueue.Enqueue(mpd);  //串口接收的数据发送给ui  ui发送过来的命令舍弃
                    }
                    else if (b.cmd == "exit")
                    {
                        return 0;
                    }
                }
                runPyDdllcunsafe.idle();
            }
            return 0;
        }

        static public int deinit()
        {
            QueueData mpd = new QueueData();
            mpd.cmd = "exit";
            inRecQueue.Enqueue(mpd);  //串口接收的数据发送给ui  ui发送过来的命令舍弃
            isrun = false;
            return 0;
        }

        static public int setAsynInData(byte[] imgByte,string id)
        {
            while(true)
            {
                for (int i = 0; i < fifoEn.Length; i++)
                {
                    if (fifoEn[i] == 0)
                    {
                        Console.WriteLine("ok:" + id + "  " + i);
                        imgByte.CopyTo(fifoByte[i], 0);
                        fifoEn[i] = 1;
                        QueueData mpd = new QueueData();
                        mpd.cmd = "compute";
                        mpd.id = id;
                        mpd.o = i;
                        inRecQueue.Enqueue(mpd);  //串口接收的数据发送给ui  ui发送过来的命令舍弃
                        return i;
                    }
                }
                Thread.Sleep(1);
            }
            
            Console.WriteLine("err");
            return -1;
        }

    }
}


static unsafe class runPyDdllcunsafe
{
    const string dllPath = @"..\..\..\..\..\cpydll\x64\Release\cpydll.dll";
    static public byte[] pypath = System.Text.Encoding.Unicode.GetBytes("D:\\Users\\59912\\anaconda3\\envs\\tf2\0");

    [DllImport(dllPath, EntryPoint = "pyInit2", CallingConvention = CallingConvention.Cdecl)]
    public static extern int pyInit2(IntPtr path,  int inLen, int outLen, IntPtr p);


    [DllImport(dllPath, EntryPoint = "pyInit", CallingConvention = CallingConvention.Cdecl)]
    public static extern int pyInit(IntPtr path, IntPtr inByte, int inLen, IntPtr outByte, int outLen);


    [DllImport(dllPath, EntryPoint = "pyDeinit", CallingConvention = CallingConvention.Cdecl)]
    public static extern int pyDeinit();
    [DllImport(dllPath, EntryPoint = "PyImportModule", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr PyImportModule(string s);


    [DllImport(dllPath, EntryPoint = "PyRunString", CallingConvention = CallingConvention.Cdecl)]
    public static extern int PyRunString(string s);

    //static byte[] inByte = new byte[3648 * 5472 * 3];
    //static float[] outByte = new float[100 * 4];

    //static byte* inByte;
    //static float* outByte;

    static IntPtr[] p = new IntPtr[2];
    static public int pyinit()
    {

        //byte* inByte1 = stackalloc byte[3648 * 5472 * 3];
        //inByte = inByte1;
        //float* outByte2 = stackalloc float[100 * 4];
        //outByte = outByte2;

        //IntPtr pData0 = Marshal.UnsafeAddrOfPinnedArrayElement(pypath, 0);
        ////IntPtr pData1 = Marshal.UnsafeAddrOfPinnedArrayElement(inByte, 0);
        ////IntPtr pData2 = Marshal.UnsafeAddrOfPinnedArrayElement(outByte, 0);
        ////pyInit(pData0, pData1, inByte.Length, pData2, outByte.Length);
        //pyInit(pData0, (IntPtr)inByte, 3648 * 5472 * 3, (IntPtr)outByte, 100 * 4);


        //IntPtr pData0 = Marshal.UnsafeAddrOfPinnedArrayElement(pypath, 0);
        //IntPtr pData1 = Marshal.UnsafeAddrOfPinnedArrayElement(inByte, 0);
        //IntPtr pData2 = Marshal.UnsafeAddrOfPinnedArrayElement(outByte, 0);
        //pyInit(pData0, pData1, inByte.Length, pData2, outByte.Length);

        IntPtr pData0 = Marshal.UnsafeAddrOfPinnedArrayElement(pypath, 0);
        IntPtr p1 = Marshal.UnsafeAddrOfPinnedArrayElement(p, 0);
        pyInit2(pData0, 3648 * 5472 * 3, 100 * 4 * 4,  p1);
        Console.WriteLine(p[0].ToString()+ p[1].ToString());

        PyRunString("import baseDll");

        PyRunString("baseDll.showImgName = '0_0'");
        PyRunString("baseDll.showImgFlag = 0");

        return 0;
    }

    static public float[] run(byte[] imgByte, float[] copy)
    {
        //imgByte.CopyTo(inByte, 0);
        //PyRunString("baseDll.startRunModle()");
        //outByte.CopyTo(copy, 0);

        //Marshal.Copy(imgByte, 0, (IntPtr)inByte, imgByte.Length);
        //PyRunString("baseDll.startRunModle()");
        //Marshal.Copy((IntPtr)outByte, copy, 0, imgByte.Length);

        Marshal.Copy(imgByte, 0, (IntPtr)p[0], imgByte.Length);
        PyRunString("baseDll.startRunModle()");
        Marshal.Copy((IntPtr)p[1], copy, 0, copy.Length);
        return copy;
    }

    static public int idle()
    {
        PyRunString("baseDll.cv2.waitKey(1)");
        return 0;
    }

    static public int pydeinit()
    {
        
        return 0;
    }



}
