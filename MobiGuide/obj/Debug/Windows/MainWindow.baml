   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	       MobiGuide.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	   R NMahApps.Metro.IconPacks, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null=	iconPacks-http://metro.mahapps.com/winfx/xaml/iconpacks  5   	   X TXceed.Wpf.Toolkit, Version=3.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  6I   -   5          � Title$  	MobiGuide��5	   	   $��896.6��6   $	��600��6*    � 
ResizeMode$ NoResize=�66    � WindowStartupLocation$ CenterScreen=�6L    � SizeToContent$ Height=�6�    � 	Resources 5
         BorderGapMaskConverter  GroupBoxStyle1 �� 
TargetType �� Property �� Value �� 
TargetType	 J� SnapsToDevicePixels
 � ColumnDefinitions � RowDefinitions �� CornerRadius ��Name �� Padding��%2  &	        &	      �� 5   
   6!   �� 5   
   " ��6'   .�5      ��  � �� �#D5DFE56,   �� 5       � ��$ 1q�60   �� 5       � �� 5      �� 5      " ��6&   .R�5      � 	 . 
 5      �� 5   "   $��6 �63   �� 5   "   $
��Auto �63   �� 5   "   $��* �63   �� 5   "   $��6 �63    5      �� 5   "   $
��Auto �60   �� 5   "   $
��Auto �60   �� 5   "   $��* �60   �� 5   "   $��6 �60   .+�5      �� 
������ #��z��6?   #��z��6s   $��4��6�   $��0��6�   $ 4��6�   $��1��6�   $��3��6�   �� 5       
�������#��z��69   $��4��6m   $ 4��6�   $��1��6�   $��3��6�   ._�5"   "   �� #��z��6)   #��z��6U   $ 3��6�   ._�5#   &   �� 
�������#��z��6A   $ 2��6u   5$   #   5%      �� 5&      $ Header��6%   $��1��65   $ 3,1,3,0q�6E   $��0��6W   $��2��6d   ._�5'   "   �� $��Header��63   ��. #	 z	 6e   5(      �� 5)      $��2��6/   $��1��6C   #��z��6S   $��2��6v   #	 z	 6�   5*      5+      5-      5.      5/      .��50      � $��0q�6   .+�51   
   o� -   5         o�Name$ mainMenu��51      $��25��6!   $	��Top=�6-    o� 
IsMainMenu . .@�52      m� $
��User��6   $��25��6%   .@�53      m� -   5         m�Name$ AddNewUserMenuItem��53      $��Add New User��67   6M   m� 54      -   5        $ EditCurrentUserMenuItem��54      $��Edit Current User��6<   6W   m� 55      -   5        $ LogoutMenuItem��55      $��Logout��63   6C   56      m� 57      $��Airline��6   $��25��6(   .@�58      m� -   5        $" EditAirlineReferenceMenuItem��58      $��Edit Airline Reference��6A   6a   59      m� 5:      -   5        $ AirportMenuItem��5:      $��Airports��60   6B   m� 5;      -   5        $ Aircraft��5;      $��Aircraft��6)   .@�5<      m� -	   5        $ AircraftConfiguration��5<      $��Aircraft Configuration��6:   6Z   m� 5=      -
   5        $ AircraftType��5=      $��Aircraft Type��61   6H   5>      m� 5?      -   5        $ TextTemplate��5?      $��Text Template��6-   6D   5@      �� 5A   
   -   5         ��Name$ groupBox��5A      $	��Top=�6%   $��75��6=   $��5,30,5,0q�6I   #���  6[   .��5B      � $��0q�6   .+�5D      �� $
��Left=�6    �� Orientation$ 
Horizontal=�68   .+�5E      �� -   5         ��Name$ logoImg��5E      $
��Left=�6-   $
��53.1��6H   $��0,0,20,-0.1q�6U   �� 5F      .+�5G      �� -   5         ��Name$ airlineNameTxtBlock��5G   $   $��Center=�6A   $
��Wrap=�6\   $
��Bold"�6p   $��0,5,0,0q�6�   5H      5I      �� 5J      $��25��6   $	��Top=�6)   $ 
Horizontal=�6A   $��Right=�6Z   $��0,0,10,0q�6v   .+�5K      �� -   5        $ 	textBlock��5K       $
��Wrap=�63   $��Logged In As :��6G   $��Center=�6]   $��0,0,5,0q�6x   �� 5L      -   5        $ nameTxtBlock��5L       $
��Wrap=�66   $
��Left=�6J   $��Center=�6e   $��0q�6�   $
��Bold"�6�   5M      5O      5P      �� 5Q   
   -   5        $ 	groupBox1��5Q      $��	5,110,5,5q�6&   #���  69   $	��Top=�6b   .��5R      �� $	��Top=�6   .+�5S      �� $ 
Horizontal=�6   $��Center=�66   $��0,10,0,0q�6S   .+�5T      �� -   5         ��Name$ showLogoBtn��5T      $	��120��62   $��Stretch=�6>   $��Stretch=�6c   $��0q�6�   $��40��6�   .��5U      (�  (� Stretch$
 Fill=�6"   .��5V      �� $��36��6)   $	��116��65   $ 
Horizontal=�6A   .+�5W   "   ,  &MahApps.Metro.IconPacks.PackIconModern  �  �ControlzEx.PackIconBase`1[[MahApps.Metro.IconPacks.PackIconModernKind, MahApps.Metro.IconPacks, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null]]   Kind$
 Home=�6;   $
��Auto��6G   $
��Auto��6T   $��
10,10,0,10q�6b   �� 5X   "   $
��Wrap=�6,   $��	Show Logo��6@   $��14(�6�   $��9,0,0,0q�6�   $��Center=�6�   5Y      5Z      5[      �� 5\      -   5        $ showSeatMapBtn��5\      $	��135��65   $��Stretch=�6A   $��Stretch=�6f   $��30,0,0,0q�6�   $��40��6�   .��5]      (� $
 Fill=�6"   .��5^      �� $ 
Horizontal=�6)   $��36��6B   $	��131��6N   .+�5_   "     $ People=�6;   $
��Auto��6I   $
��Auto��6V   $��
10,10,0,10q�6d   �� 5`   "   $
��Wrap=�6,   $��Show Seatmap��6@   $��Center=�6�   $��14(�6�   $��10,0,0,0q�6�   5a      5b      5c      �� 5d      -   5        $ showTextBtn��5d      $	��120��62   $��Stretch=�6>   $��Stretch=�6c   $��30,0,0,0q�6�   $��40��6�   .��5e      (� $
 Fill=�6"   .��5f      �� $ 
Horizontal=�6)   $��36��6B   $	��116��6N   .+�5g   "     $ TextAlignLeft=�6;   $
��Auto��6P   $
��Auto��6]   $��
10,10,0,10q�6k   �� 5h   "   $
��Wrap=�6,   $��	Show Text��6@   $��Center=�6�   $��14(�6�   $��10,0,0,0q�6�   5i      5j      5k      5l      �� 5m      $ 
Horizontal=�6   $��	50,20,0,0q�66   $
��Left=�6I   .+�5n      �� -   5        $ 
textBlock1��5n       $
��Wrap=�64   $��Aircraft Configuration��6H   $��14(�6f   $��20,0,0,0q�6t   �� 5o      $	��0.7q�6   $��30,0,0,0q�63   ._�5p      �� -   5         ��Name$ aircraftConfigComboBox��5p   #   $	��150��6C    �� SelectedValuePath$ Value��6R   6l   5q      5r      �� 5s      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5t      �� -   5        $ 
textBlock2��5t       $
��Wrap=�64   $��Boarding Location��6H   $��14(�6a   $��20,0,0,0q�6o   	� 5u      -   5         	�Name$ bridgeRadBtn��5u   "   $��Bridge��68   $��52,0,0,0q�6I    	� 	GroupName$ BoardingLocation��6[    d� 	IsChecked$
 TrueY�6x   5v      �� 5w      $ 
Horizontal=�6   $��50,0,0,0q�66   $
��Left=�6H   .+�5x      	� -   5        $ busgateRadBtn��5x   "   $��Busgate��69   $��	184,0,0,0q�6K   $ BoardingLocation��6^   5y      �� 5z      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5{      �� -   5        $ 
textBlock3��5{       $
��Wrap=�64   $��Boarding Using Doors��6H   $��14(�6d   $��20,0,0,0q�6r   �� 5|      -   5         ��Name$ frontDoorCheckBox��5|      $��
Front Door��6:   $��29,0,0,0q�6O   5}      �� 5~      $ 
Horizontal=�6   $��50,0,0,0q�66   $
��Left=�6H   .+�5      �� -   5        $ rearDoorCheckBox��5      $��	Rear Door��69   $��	185,0,0,0q�6M   5�      �� 5�      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5�      �� -   5        $ 
textBlock4��5�       $
��Wrap=�64   $��Flight Number��6H   $��14(�6]   $��20,0,0,0q�6k   �� 5�      -   5         ��Name$ flightNoTextBox��5�      $��23��67     �� TextWrapping$
  Wrap=�6C   $	��150��6W   $��75,0,0,0q�6c   5�      �� 5�      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5�      �� -   5        $ 
textBlock5��5�       $
��Wrap=�64   $��Origin��6H   $��14(�6V   $��20,0,0,0q�6d   �� 5�      $	��0.7q�6   $��	126,0,0,0q�63   ._�5�      �� -    5        $ originComboBox��5�   #   $��0,0,0,0q�6;   $	��150��6L   $ Value��6[   5�      5�      �� 5�      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5�      �� -!   5        $ 
textBlock6��5�       $
��Wrap=�64   $��Destination��6H   $��14(�6[   $��20,0,0,0q�6i   �� 5�      $	��0.7q�6   $��94,0,0,0q�63   ._�5�      �� -"   5        $ destinationComboBox��5�   #   $��0,0,0,0q�6@   $	��150��6Q   $ Value��6`   5�      5�      �� 5�      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5�      �� -#   5        $ 
textBlock7��5�       $
��Wrap=�64   $��Departure Date��6H   $��14(�6^   $��20,0,0,0q�6l   (  "System.Windows.Controls.DatePicker  5�      -$   5        !  Name$! departureDatePicker��5�   !   $��69,0,0,0q�6>   5�      �� 5�      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5�      �� -%   5        $ 
textBlock8��5�       $
��Wrap=�64   $��Departure Time��6H   $��14(�6^   $��20,0,0,0q�6l   "  Xceed.Wpf.Toolkit.TimePicker  5�      -&   5        "  Name$" departureTimePicker��5�   &   $��68,0,0,0q�6C   $	��100��6U   &   Xceed.Wpf.Toolkit.DateTimeUpDown#   Format$# Custom=�6a   $   FormatString$$ HH:mm��6q   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase%   AllowTextInput% .  6�   5�      �� 5�      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5�      �� -'   5        $ textBlock10��5�       $
��Wrap=�65   $��Text Template��6I   $��14(�6^   $��20,0,0,0q�6l   �� 5�      $	��0.7q�6   $��80,0,0,0q�63   ._�5�      �� -(   5        $ textTemplateComboBox��5�   #   $��0,0,0,0q�6A   $	��150��6R   $ Value��6a   6{   5�      5�      �� 5�      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   .+�5�      �� -)   5        $ 
textBlock9��5�       $
��Wrap=�64   $��Display Duration (s)��6H   $��14(�6d   $��20,0,0,0q�6r   %  Xceed.Wpf.Toolkit.IntegerUpDown  5�      -*   5        &  Name$& rotateSecondsUpDown��5�   )   $��44,0,0,0q�6F   $��50��6X   �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]'   Value$' 10X�6c   5�      �� 5�      $ 
Horizontal=�6   $��	50,10,0,0q�66   $
��Left=�6I   $	��Top=�6d   .+�5�      �� -+   5        $ textBlock11��5�       $
��Wrap=�65   $��Text Displayed��6I   $��14(�6_   $��20,0,0,0q�6m   �� 5�      -,   5        $ textDisplayedTextBox��5�      $
  Wrap=�6<   $	��260��6P   $��	76,0,0,20q�6\   "( �� VerticalScrollBarVisibility$
( Auto=�6o   $��90��6�   {�.  6�   5�      5�      5�      5�      5�      