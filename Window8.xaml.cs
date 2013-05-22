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
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;


namespace KinectHubDemo
{


    public partial class Window8 : Window
    {
        private KinectSensor _kinect;

        //前景用户图片对象
        private WriteableBitmap WeathermanBitmap;
        private Int32Rect WeathermanImageRect;
        private int WeathermanBitmapStride;

        //深度图像帧数组、彩色图像帧数组
        private short[] DepthPixelData;
        private byte[] ColorPixelData;
        private bool isWindowsClosing = false;
        private SpeechRecognitionEngine _sre;

        private void startKinect()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                //选择第一个Kinect设备
                _kinect = KinectSensor.KinectSensors[0];
                if (_kinect == null)
                    return;

                //启用深度摄像头和彩色摄像头，为了获得更好的映射效果，彩色摄像头的分辨率恰好是深度摄像头分辨率的2倍
                _kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                _kinect.ColorStream.Enable(ColorImageFormat.RgbResolution1280x960Fps12);
                _kinect.SkeletonStream.Enable();

                //根据深度图像定义前景图片对象,指定WriteableBitmap对象高度、宽度及格式，一次性创建内存，之后只需更新像素即可
                DepthImageStream depthStream = _kinect.DepthStream;
                WeathermanBitmap = new WriteableBitmap(depthStream.FrameWidth, depthStream.FrameHeight, 96, 96, PixelFormats.Bgra32, null);
                WeathermanImageRect = new Int32Rect(0, 0, (int)Math.Ceiling(WeathermanBitmap.Width), (int)Math.Ceiling(WeathermanBitmap.Height));
                WeathermanBitmapStride = depthStream.FrameWidth * 4;


                DepthPixelData = new short[_kinect.DepthStream.FramePixelDataLength];
                ColorPixelData = new byte[_kinect.ColorStream.FramePixelDataLength];

                //同步深度图像和彩色图像事件
                _kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_kinect_AllFramesReady);

                _kinect.Start();

                //语音命令导播切换城市
                CityChooserViaVoice();
            }
            else
            {
                MessageBox.Show("没有发现任何Kinect设备");
            }
        }

        void _kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (isWindowsClosing)
                return;

            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    RenderWeathermanTransparentPortrait(colorFrame, depthFrame);
                }
            }
        }


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


        private void RenderWeathermanTransparentPortrait(ColorImageFrame colorFrame, DepthImageFrame depthFrame)
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
                colorFrame.CopyPixelDataTo(ColorPixelData);
                byte[] weathermanImage = new byte[depthFrame.Height * WeathermanBitmapStride];

                for (int depthY = 0; depthY < depthFrame.Height; depthY++)
                {
                    for (int depthX = 0; depthX < depthFrame.Width; depthX++, playerImageIndex += bytesPerPixelOfBgrImage)
                    {
                        depthPixelIndex = depthX + (depthY * depthFrame.Width);
                        playerIndex = DepthPixelData[depthPixelIndex] & DepthImageFrame.PlayerIndexBitmask;

                        //用户索引标志不为零，则代表该处属于人体部位
                        if (playerIndex != 0)
                        {
                            //将深度图像中的某一个点坐标映射到彩色图像坐标点上
                            colorPoint = _kinect.MapDepthToColorImagePoint(depthFrame.Format, depthX, depthY, DepthPixelData[depthPixelIndex], colorFrame.Format);
                            //colorPoint = _kinect.CoordinateMapper.MapDepthPointToColorPoint(depthFrame,DepthImagePoint,colorFrame.Format);
                            colorPixelIndex = (colorPoint.X * colorFrame.BytesPerPixel) + (colorPoint.Y * colorStride);

                        }
                    }
                }

                WeathermanBitmap.WritePixels(WeathermanImageRect, weathermanImage, WeathermanBitmapStride, 0);
            }
        }

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



        private void CityChooserViaVoice()
        {
            // 等待4秒钟时间，让Kinect传感器初始化启动完成
            System.Threading.Thread.Sleep(4000);

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

            _sre = new SpeechRecognitionEngine(ri.Id);

            // 示例，添加上海、北京两个城市
            var cities = new Choices();
            cities.Add("one");
            cities.Add("two");
            cities.Add("threee");
            cities.Add("four");
            cities.Add("five");
            cities.Add("six");
            cities.Add("seven");
            cities.Add("stop");
            var gb = new GrammarBuilder { Culture = ri.Culture };

            // 创建语法对象                                
            gb.Append(cities);

            //根据语言区域，创建语法识别对象
            var g = new Grammar(gb);

            // 将这些语法规则加载进语音识别引擎
            _sre.LoadGrammar(g);

            // 注册事件：有效语音命令识别、疑似识别、无效识别
            _sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            _sre.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(sre_SpeechHypothesized);
            _sre.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);

            // 初始化并启动 Kinect音频流
            Stream s = source.Start();
            _sre.SetInputToAudioStream(
                s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

            // 异步开启语音识别引擎，可识别多次
            _sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 语音命令识别处理，切换天气预报背景城市图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //语音识别信心度超过70%
            if (e.Result.Confidence >= 0.7)
            {
                string city = e.Result.Text.ToLower();
                if (city == "one")
                {
                    string cityMap = "pack://application:,,,/Resources/images/back1.jpg";
                    CityImage.Source = new BitmapImage(new Uri(cityMap));
                }
                else if (city == "two")
                {
                    string cityMap = "pack://application:,,,/Resources/images/back2.jpg";
                    CityImage.Source = new BitmapImage(new Uri(cityMap));
                }
                else if (city == "stop")
                {
                    //Window_Closing();
                    this.Close();
                }
            }
        }


        public Window8()
        {
            InitializeComponent();
        }

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
