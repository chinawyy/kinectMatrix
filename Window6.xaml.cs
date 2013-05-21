using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Kinect;

namespace KinectHubDemo
{
    //这个部分主要是基于语音识别中的页面翻转
    /// <summary>
    /// Window6.xaml 的交互逻辑
    /// </summary>
    public partial class Window6 : Window
    {

        private KinectSensor _kinect;

        //前景用户图片对象
        private WriteableBitmap pptmanBitmap;
        private Int32Rect pptmanRect;
        private int pptmanBitmapStride;

        //深度图像帧数组，彩色图像帧数组
        private short[] DepthPixelData;
        private byte[] ColorPixelData;
        private bool isWindowsClosing = false;//默认创口不关闭

        //语音识别变量：置信度处理,后面中我设置为0.75
        private SpeechRecognitionEngine _wyy;

        //对kinect的初始化处理
        private void startKinect()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _kinect = KinectSensor.KinectSensors[0];//默认选取第一个跟踪对象
                if (_kinect == null) return;

                //采用深度摄像头和彩色摄像头,调节分辨率
                _kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                _kinect.ColorStream.Enable(ColorImageFormat.RgbResolution1280x960Fps12);
                _kinect.SkeletonStream.Enable();

                //根据深度图像定义前景图片对象,指定WriteableBitmap对象高度、宽度及格式，一次性创建内存，之后只需更新像素即可
                DepthImageStream depthStream = _kinect.DepthStream;
                pptmanBitmap = new WriteableBitmap(depthStream.FrameWidth, depthStream.FrameHeight, 96, 96, PixelFormats.Bgra32, null);
                pptmanRect = new Int32Rect(0, 0, (int)Math.Ceiling(pptmanBitmap.Width), (int)Math.Ceiling(pptmanBitmap.Height));
                pptmanBitmapStride = depthStream.FrameWidth * 4;
                pptmanImage.Source = pptmanBitmap;

                DepthPixelData = new short[_kinect.DepthStream.FramePixelDataLength];
                ColorPixelData = new byte[_kinect.ColorStream.FramePixelDataLength];

                //同步深度图像和彩色图像事件
                _kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_kinect_AllFramesReady);
                _kinect.Start();

                //语音命令导播切换城市
                //PPTChooserViaVoice();
            }
            else
            {
                MessageBox.Show("wyy没有发现任何的kinect设备");
            }
        }

        void _kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (isWindowsClosing) return;
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                   // RenderWeathermanTransparentPortrait(colorFrame, depthFrame);
                }

            }
        
        }

        //关闭kinect
        private void stopKinect()
        {
            if (_kinect != null)
            {
                if (_kinect.Status == KinectStatus.Connected)
                {
                    _kinect.Stop();
                    _kinect.AudioSource.Stop();
                }
            }
        }

        // 通过深度图像帧的“用户索引标志”，将深度图像帧中属于前景用户的点映射到彩色图像帧中，创建前景用户“透明图”
        private void pptmanTransparentPortrait(ColorImageFrame colorFrame, DepthImageFrame depthFrame)
        {
            if (depthFrame != null && colorFrame != null)
            {
                int depthPixelIndex;
                int playerIndex;
                int colorPixelIndex;
                ColorImagePoint colorPoint;
                int colorStride = colorFrame.BytesPerPixel * colorFrame.Width;
                int bytesPerPixelOfBgrImage = 4;
                int playerImageIndex = 0;

                depthFrame.CopyPixelDataTo(DepthPixelData);
                //colorFrame.CopyPixelDataTo(ColorPixelData);

                byte[] pptmanImage = new byte[depthFrame.Height * pptmanBitmapStride];
      
                for (int depthY = 0; depthY < depthFrame.Height; depthY++)
                {
                    for (int depthX = 0; depthX < depthFrame.Width; depthX++, playerImageIndex += bytesPerPixelOfBgrImage)
                    {
                        depthPixelIndex = depthX + (depthY * depthFrame.Width);
                        playerIndex = DepthPixelData[depthPixelIndex] & DepthImageFrame.PlayerIndexBitmask;

                        //用户索引标志不为零，则代表该处属于人体部位
                        if (playerIndex != 0)
                        { //将深度图像中的某一个点坐标映射到彩色图像坐标点上
                            colorPoint = _kinect.MapDepthToColorImagePoint(depthFrame.Format, depthX, depthY, DepthPixelData[depthPixelIndex], colorFrame.Format);
                             
                            colorPixelIndex = (colorPoint.X * colorFrame.BytesPerPixel) + (colorPoint.Y * colorStride);


                            pptmanImage[playerImageIndex] = ColorPixelData[colorPixelIndex];         //Blue    
                            pptmanImage[playerImageIndex + 1] = ColorPixelData[colorPixelIndex + 1];  //Green
                            pptmanImage[playerImageIndex + 2] = ColorPixelData[colorPixelIndex + 2];   //Red
                            pptmanImage[playerImageIndex + 3] = 0xFF;                                 //Alpha
                        }
                    }
                }
                pptmanBitmap.WritePixels(pptmanRect, pptmanImage, pptmanBitmapStride, 0);
            }
        }

        //语音识别函数
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        //具体的哪张PPT的识别
        private void pptChooserViaVoice()
        {
            // 等待5秒钟时间，让Kinect传感器初始化启动完成
            System.Threading.Thread.Sleep(5000);

            // 获取Kinect音频对象
            KinectAudioSource source = _kinect.AudioSource;
            source.EchoCancellationMode = EchoCancellationMode.None; // 本示例中关闭“回声抑制模式”
            source.AutomaticGainControlEnabled = false; // 启用语音命令识别需要关闭“自动增益”

            RecognizerInfo ri = GetKinectRecognizer();

            if (ri == null)
            {
                MessageBox.Show("Could not find Kinect speech recognizer.");
                return;
            }

            _wyy = new SpeechRecognitionEngine(ri.Id);

            //添加多张PPT。
            var ppts = new Choices();
            ppts.Add("one");
            ppts.Add("two");
            ppts.Add("three");

            var gb = new GrammarBuilder { Culture = ri.Culture };
            //创建语法对象
            gb.Append(ppts);
            var g = new Grammar(gb);//根据语音区域，创建语法识别对象
            _wyy.LoadGrammar(g);//加载进语音识别引擎
            //注册事件,有效语音命令识别,疑似识别,无效识别
            _wyy.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(wyy_SpeechRecognized);
            _wyy.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(wyy_SpeechHypothesized);
            _wyy.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(wyy_SpeechRecognitionRejected);

            //初始化并且启动Kinect音频流
            Stream s = source.Start();
            _wyy.SetInputToAudioStream(
                s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            // 异步开启语音识别引擎，可识别多次
            _wyy.RecognizeAsync(RecognizeMode.Multiple);

        }

        void wyy_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void wyy_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void wyy_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.75)
            {
                String ppt = e.Result.Text.ToLower();
                String pptMap;
                if (ppt == "one")
                {
                    pptMap = "Resources/images/back1.jpg";      
                }
                else if (ppt == "two")
                {
                   pptMap = "Resources/images/back1.jpg";      
                }
                else if (ppt == "three")
                {
                   pptMap = "Resources/images/back1.jpg";
                }
                else if (ppt == "four")
                {
                    pptMap = "Resources/images/back1.jpg";
                }
                else//默认的空PPT
                {
                    pptMap = "Resources/images/back1.jpg";
                }
                pptImage.Source = new BitmapImage(new Uri(pptMap));
            }
        }

        public Window6()
        {
            InitializeComponent();
        }
        //封转kinect的开启和关闭
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startKinect();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isWindowsClosing = true;
            stopKinect();
        }
    }
}
