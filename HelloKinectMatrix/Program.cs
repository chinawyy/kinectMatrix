using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace HelloKinectMatrix
{
    class Program
    {
        static void Main(string[] args)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Welcome to the kinect world");

                //选择第一个传感器
                KinectSensor _kinect = KinectSensor.KinectSensors[0];
                _kinect.DepthStream.Enable();
                _kinect.DepthFrameReady += new
                EventHandler<DepthImageFrameReadyEventArgs>(_kinect_DepthFrameReady);
                _kinect.Start();

                //按回车键退出
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                }

                //关闭kinect传感器
                _kinect.Stop();
                Console.WriteLine("Exit the kinect matrix");
            }
            else
            {
                Console.WriteLine("Please check the kinect sensor");
            }
        }


        //打印数据到控制台
        static void _kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {
                    short[] depthPixelData = new short[depthFrame.PixelDataLength];
                    depthFrame.CopyPixelDataTo(depthPixelData);
                    foreach (short pixel in depthPixelData)
                    {
                        Console.Write(pixel);
                    }
                }
            }
        }

    }
}
