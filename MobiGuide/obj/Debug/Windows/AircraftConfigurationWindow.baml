   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	   +    %MobiGuide.AircraftConfigurationWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	   -   5          � Title$  Aircraft Configuration��5   	   $	��500��6(   $	��700��65    � WindowStartupLocation$ CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�6.    � SizeToContent$ Height=�6D   .��5
      �� .+�5   
   �� $��35��6    �� Orientation$ 
Horizontal=�6n   $��0,5,0,0q�6�   .+�5      �� -   5         ��Name$ 	textBlock��5      $!��Aircraft Configuration Code��6+   $��14(�6N   $��Center=�6\   $��10,0,0,0q�6w   �� 5      -   5         ��Name$ aircraftConfigCodeTextBox��5       �� TextWrapping$
 Wrap=�69   $	��235��6M   $��14(�6Y   $��Center=�6g   $��29,0,0,0q�6�   �� 5      -   5         ��Name$ 	searchBtn��5      $��Search��6(   $��75��69   $��14(�6D   $��Center=�6R   $��	155,0,0,0q�6m   	 �� 	IsDefault	 . 6�   5      �� 5   
   $��35��6   $ 
Horizontal=�6Q   .+�5      �� -   5        $ textBlock_Copy��5      $!��Aircraft Configuration Name��60   $��14(�6S   $��Center=�6a   $��10,0,0,0q�6|   �� 5      -   5        $ aircraftConfigNameTextBox��5      $
 Wrap=�69   $	��235��6M   $��14(�6Y   $��Center=�6g   $��24,0,0,0q�6�   �� 5      -   5        $ clearBtn��5      $��Clear��6'   $��75��67   $��14(�6B   $��Center=�6P   $��	155,0,0,0q�6k   6~   5      �� 5   
   $��35��6   $ 
Horizontal=�6!   .+�5      �� -   5        $ textBlock1_Copy��5      $
��Wrap=�61   $��Aircraft Type��6E   $��Center=�6Z   $��14(�6u   $��10,8.19,0,8.19q�6�   �� 5      -	   5        
 ��Name$
 aircraftTypeComboBox��5      $	��130��65    �� SelectedIndex$ 0��6A   $��Center=�6S   $��14(�6n   $��20,5.19,0,5.19q�6|    �� SelectedValuePath$ Value��6�   �� 5      -
   5        $ 
textBlock1��5      $
��Wrap=�6,   $��Status��6@   $��Center=�6N   $��14(�6i   $��30,8.19,0,8.19q�6w   �� 5      -   5        $
 statusComboBox��5      $	��130��6/   $ 0��6;   $��Center=�6M   $��14(�6h   $��10,5.19,0,5.19q�6v   $ Value��6�   5      �� 5   
   #��� ��6   $��1q�6V   $
��Left=�6j   $	��330��6�   $��
10,10,10,0q�6�   ._�5      � $
��Auto=�6   $��0q�6C   $	��674��6N   .+�5      &   System.Windows.Controls.DataGrid  -   5          Name$ aircraftConfigDataGrid��5         AutoGenerateColumns .  $	��Top=�6q      SelectionUnit$ FullRow=�6�   $��0q�5         SelectionMode$ Single=�6#      CanUserAddRows .     HorizontalGridLinesBrush �	#FFD1D1D1   VerticalGridLinesBrush �	#FFD1D1D1 �� AlternationCount$ 2��5   _      AlternatingRowBackground �	#FFEEFFA2$��0q�6�      
IsReadOnly . $��Stretch=�5       $��Stretch=�6<   $   HorizontalScrollBarVisibility$ Disabled=�6_      	CellStyle 5!      �� 5"       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6     �� Triggers 5#      P� 5$   "    P� Property �@ 
IsSelected P� Value . . �5%   &   ��  �� Property � �� �� Value
 ���� 6D   �� 5&   &    � ��$ 0q�6H   5'   #   5)         Columns 5+      0  *System.Windows.Controls.DataGridTextColumn  5,      ,  &System.Windows.Controls.DataGridColumn    Header$
  Code��6-   !   Width5  /System.Windows.Controls.DataGridLengthConverter$! 80 6i   "   FontSize$" 14Q�6t   1  +System.Windows.Controls.DataGridBoundColumn#   Binding# 6;   �� *AircraftConfigurationCode+6t     5-      $  Configuration Name��6-   $! * 6w   $" 14Q�6�   # 6I   �� *AircraftConfigurationName+6�     5.      $  Aircraft Type��6-   $	! 150 6v   $" 14Q�6�   # 6D   �� *AircraftType.AircraftTypeName+6�     5/      $  Status��6-   $	! 100 6X   $" 14Q�6d   $   CanUserResize$ .  # 6=   �� *Status+6r   51      52      53      �� 54   
   $��45��6   $ 
Horizontal=�6!   .+�55      �� -   5        $ newBtn��55      $	��New��6%   $��75��63   $��14(�6>   $��Center=�6L   $��10,0,0,0q�6g   6y   �� 56      -   5        $ editBtn��56      $
��Edit��6&   $��75��65   $��14(�6@   $��Center=�6N   $��10,0,0,0q�6i   {�.  6�   57      58      59      