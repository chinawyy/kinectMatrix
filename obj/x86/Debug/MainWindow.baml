   M S B A M L   `   `   `  ����P  LCoding4Fun.Kinect.Wpf, Version=1.5.0.0, Culture=neutral, PublicKeyToken=nullnKclr-namespace:Coding4Fun.Kinect.Wpf.Controls;assembly=Coding4Fun.Kinect.WpfCoding4Fun.Kinect.Wpf.Controls  5       KinectUserInterfaceDemo   KinectHub.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   `ControlsKclr-namespace:Coding4Fun.Kinect.Wpf.Controls;assembly=Coding4Fun.Kinect.Wpf     5      -   5          � WindowState$  	Maximized=�5   	    � WindowStyle$ SingleBorderWindow=�5   	   ���#1C4A57 � Title$ 游戏欢迎界面��5      $	��800��6   $
��1280��6   .��5	      � -   5         �Name$ theGrid��5	      .+�5
   
   �� -   5        
������  � DataContext 5
   *   �� .+�5      0   *Coding4Fun.Kinect.Wpf.Controls.HoverButton  -   5          Name$ kinectButton��5   :   $��0q�6#   $��0q�6.      	ImageSize$ 64Q�6P      ImageSource$ /Resources/Hand_Basic.png��5   !      ActiveImageSource$ /Resources/Hand_Basic.png��5   !   	   TimeInterval$
	 3000��5   !   
 �� Left$	
 525��66   
 �� Top$ 32��6H    K� ZIndex$
 1000��6X   �� 5      -   5        $��
textBlock3��5   h   $	
 322��6   $ 32��6*   $��46(�6:   
�������$	��101��6[   $��Welcome to wyy world��6z   $	��645��6�   �� 5      -   5        $��button1��5   D   $
 41��6   $	 112��6&   $	��300��67   $	��335��6S   $��Right=�6_   $��Bottom=�6�   ��5      �� 5       �� ImageSource$  Resources/images/start.jpg��6!   5      �� 5      -   5        $��button2��5   E   $	
 455��6   $	 115��6'   $	��300��68   $	��335��6T   $��Right=�6`   $��Bottom=�6�   ��5      �� 5      $A ;/KinectUserInterfaceDemo;component/Resources/images/end.jpg��6!   5      �� 5      -   5        $��button3��5   D   $
 41��6   $	 435��6&   $	��300��67   $	��335��6S   $��Right=�5      $��Bottom=�6&   ��5      �� 5       $E ?/KinectUserInterfaceDemo;component/Resources/images/explore.jpg��6!   5"      �� 5#      -	   5         ��Name$ button4��5#   E   $	
 455��6   $	 435��6'   $	��300��68   $	��335��6V   $��Right=�5$   	   $��Bottom=�6,   ��5%      �� 5&      $  Resources/images/yuyin.jpg��6!   5(      �� 5)      -
   5        $��
videoImage��5)      $	��769��6&   $	��441��63   $	
 831��6?   $ 0��6Q   5+      5,      5.      