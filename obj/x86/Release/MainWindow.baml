   M S B A M L   `   `   `  ���� P  LCoding4Fun.Kinect.Wpf, Version=1.5.0.0, Culture=neutral, PublicKeyToken=nullnKclr-namespace:Coding4Fun.Kinect.Wpf.Controls;assembly=Coding4Fun.Kinect.WpfCoding4Fun.Kinect.Wpf.Controls   KinectUserInterfaceDemo   KinectHub.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     `ControlsKclr-namespace:Coding4Fun.Kinect.Wpf.Controls;assembly=Coding4Fun.Kinect.Wpf     -     � WindowState$  	Maximized=� � WindowStyle$ SingleBorderWindow=����#1C4A57 � Title$ 游戏欢迎界面��$	��800��$
��1280��.��� -    �Name$ theGrid��.+��� -   
������  � DataContext �� .+�0   *Coding4Fun.Kinect.Wpf.Controls.HoverButton  -     Name$ kinectButton��$��0q�$��0q�   	ImageSize$ 64Q�   ImageSource$ /Resources/Hand_Basic.png��   ActiveImageSource$ /Resources/Hand_Basic.png��	   TimeInterval$
	 3000��
 �� Left$	
 525��
 �� Top$ 32�� K� ZIndex$
 1000���� -   $��
textBlock3��$	
 322��$ 32��$��46(�
�������$	��101��$��Welcome to wyy world��$	��645���� -   $��button1��$
 41��$	 112��$	��300��$	��335��$��Right=�$��Bottom=�����  �� ImageSource$  Resources/images/start.jpg���� -   $��button2��$	
 455��$	 115��$	��300��$	��335��$��Right=�$��Bottom=����� $A ;/KinectUserInterfaceDemo;component/Resources/images/end.jpg���� -   $��button3��$	
 853��$	 112��$	��300��$	��335��$��Right=�$��Bottom=����� $E ?/KinectUserInterfaceDemo;component/Resources/images/explore.jpg���� -	    ��Name$ textTip��$��24(�
�������$��35��$��The current scenery��$	��289��$	
 899��$	 451���� -
   $��
videoImage��$	��166��$	��289��$	
 899��$	 491���� -   $��listBoxHoverEvent��$
 41��$	 464��$	��128��$	��339�����	#FF1E1C37���	#FF20ED16