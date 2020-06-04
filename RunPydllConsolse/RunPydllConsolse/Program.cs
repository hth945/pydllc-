using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RunPydllConsolse
{
    class Program
    {

        public static void bitmap2BGR24(Bitmap img, byte[] imgByte)
        {
            Bitmap bmp = img;
            if (img.PixelFormat == PixelFormat.Format24bppRgb) //格式正确 可以直接拷贝
            {
                //位图矩形
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                //以可读写的方式将图像数据锁定
                System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                //构造一个位图数组进行数据存储
                int bLength = bmp.Width * bmp.Height * 3;
                //得到图形在内存中的首地址
                IntPtr ptr = bmpdata.Scan0;
                //将被锁定的位图数据复制到该数组内
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, imgByte, 0, bLength);
                //把处理后的图像数组复制回图像
                //System.Runtime.InteropServices.Marshal.Copy(rgbVal, 0, ptr, bytes);
                //解锁位图像素
                bmp.UnlockBits(bmpdata);
            }else
            {
                //for (int i = 0; i < bmp.Width; i++)
                //{
                //    for (int j = 0; j < bmp.Height; j++)
                //    {
                //        Color pixelColor = bmp.GetPixel(i, j);
                //        imgByte[(i * bmp.Height + j) * 3] = pixelColor.R;
                //        imgByte[(i * bmp.Height + j) * 3 + 1] = pixelColor.G;
                //        imgByte[(i * bmp.Height + j) * 3 + 2] = pixelColor.B;
                //    }
                //}

                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        Color pixelColor = bmp.GetPixel(i, j);
                        imgByte[(j * bmp.Width + i) * 3] = pixelColor.B;
                        imgByte[(j * bmp.Width + i) * 3 + 1] = pixelColor.G;
                        imgByte[(j * bmp.Width + i) * 3 + 2] = pixelColor.R;
                    }
                }

            }
        }

        public static Bitmap ReadImageFile(string path)
        {
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            fs.Close();
            Bitmap bit = new Bitmap(result);
            return bit;
        }


        static void Main(string[] args)
        {


            //Bitmap bmp = ReadImageFile("0_7.bin.png");
            ////byte[] b = new byte[128];

            //bitmap2BGR24(bmp, runPyDdllc.inByte);

            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine("inByte[" + i.ToString() + "]:" + runPyDdllc.inByte[i].ToString());
            //}


            //runPyDdllc.runTest();
            byte[] imgByte = new byte[3648 * 5472 * 3];
            byte[] inByte = new byte[3648 * 5472 * 3];

            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 10; i++)
            {
                sw.Restart();
                imgByte.CopyTo(inByte, 0);
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                Console.WriteLine("DateTime costed for Shuffle function is: {0}ms", ts.TotalMilliseconds);
            }


            Console.ReadKey();

        }
    }
}
