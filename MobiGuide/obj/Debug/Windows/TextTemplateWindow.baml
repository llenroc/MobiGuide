   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	   "    MobiGuide.TextTemplateWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	     � Title$  Text Template��5   	   $	��500��6   $	��700��6,    � WindowStartupLocation$ CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�6.   .��5
      �� .+�5   
   �� $��35��6    �� Orientation$ 
Horizontal=�6n   $��0,5,0,0q�6�   .+�5      �� -   5         ��Name$ 	textBlock��5      $��	Text Name��6+   $��14(�6<   $��Center=�6J   $��10,0,0,0q�6e   �� 5      -   5         ��Name$ textNameTextBox��5       �� TextWrapping$
 Wrap=�6/   $	��150��6C   $��14(�6O   $��Center=�6]   $��29,0,0,0q�6x   �� 5      -   5        $ 
textBlock1��5      $
��Wrap=�6,   $��Status��6@   $��Center=�6N   $��14(�6i   $��30,0,0,0q�6w   �� 5      -   5         ��Name$ statusComboBox��5      $	��149��6/    �� SelectedIndex$ 0��6;   $��Center=�6M   $��14(�6h   $��10,0,0,0q�6v   	 �� SelectedValuePath$	 Value��6�   �� 5      -   5        
 ��Name$
 	searchBtn��5      $��Search��6(   $��75��69   $��14(�6D   $��Center=�6R   $��	120,6,0,6q�6m    �� 	IsDefault . 6�   5      �� 5   
   $��35��6   $ 
Horizontal=�6Q   .+�5      �� -   5        $ textBlock_Copy��5      $��Text Template��60   $��14(�6E   $��Center=�6S   $��10,0,0,0q�6n   �� 5      -   5        $ textTemplateTextBox��5      $
 Wrap=�63   $	��376��6G   $��14(�6S   $��Center=�6a   $��10,7.19,0,7.19q�6|   �� 5      -   5        $
 clearBtn��5      $��Clear��6'   $��75��67   $��14(�6B   $��Center=�6P   $��	120,6,0,6q�6k   6~   5      � 5   
   $
��Auto=�6   $	��330��6?   $��
10,10,10,0q�6L   .+�5      &   System.Windows.Controls.DataGrid  -	   5          Name$ textTemplateDataGrid��5         AutoGenerateColumns .  $	��Top=�6k      SelectionUnit$ FullRow=�6�   $��0q�5         SelectionMode$ Single=�6#      CanUserAddRows .     HorizontalGridLinesBrush �	#FFD1D1D1   VerticalGridLinesBrush �	#FFD1D1D1 �� AlternationCount$ 2��5   _      AlternatingRowBackground �	#FFEEFFA2$��0q�6�      
IsReadOnly . $��Stretch=�5   �   $��Stretch=�6�   $   HorizontalScrollBarVisibility$ Disabled=�6�   ��5   �   Z�    	CellStyle 5      �� 5       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6    �� Triggers 5      P� 5       P� Property �@ 
IsSelected P� Value . . �5    "   ��  �� Property � �� �� Value 5!   &   �� 5"   *    �� Color$ Transparent��6:   5$   #   �� 5%   "    � ��$ 0q�6D   5&      5(         Columns 5*      0  *System.Windows.Controls.DataGridTextColumn  5+      ,  &System.Windows.Controls.DataGridColumn    Header$  	Text Name��6)   !   Width5  /System.Windows.Controls.DataGridLengthConverter$	! 120 6Y   "   FontSize$" 14Q�6e   1  +System.Windows.Controls.DataGridBoundColumn#   Binding# 6<   �� *
TextName+$   ElementStyle$ 5,      �� 5-      " ��6$   .�5.   "   ��  � ��$ CharacterEllipsis=�6A   6]   5/      51        52      $  Text Template��6)   $! * 6`   $" 14Q�6j   # 6@   �� *TextDisplay+$ 53      �� 54      " ��6$   .�55   "   ��  � ��$ CharacterEllipsis=�6A   6]   56      58        59      $  Rotate In (s)��6)   $! 80 6d   $" 14Q�6o   %   CanUserResize% .  # 6@   �� *RotateInSeconds+6}     5:      $  Status��6)   $	! 100 6T   $" 14Q�6`   % .  # 69   �� *Status+6n   5<      5=      �� 5>   
   $��45��6   $ 
Horizontal=�6!   .+�5?      �� -
   5        $
 newBtn��5?      $	��New��6%   $��75��63   $��14(�6>   $��Center=�6L   $��10,0,0,0q�6g   6y   �� 5@      -   5        $
 editBtn��5@      $
��Edit��6&   $��75��65   $��14(�6@   $��Center=�6N   $��10,0,0,0q�6i   {�.  6�   5A      5B      5C      