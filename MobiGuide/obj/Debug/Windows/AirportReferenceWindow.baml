   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	   &     MobiGuide.AirportReferenceWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	     � Title$  Airport Reference��5   	   $	��500��6#   $	��700��60    � WindowStartupLocation$ CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�6.   .��5
      �� .+�5   
   �� $��35��6    �� Orientation$ 
Horizontal=�6n   $��0,5,0,0q�6�   .+�5      �� -   5         ��Name$ 	textBlock��5      $��Airport Code��6+   $��14(�6?   $��Center=�6M   $��10,0,0,0q�6h   �� 5      -   5         ��Name$ airportCodeTextBox��5       �� TextWrapping$
 Wrap=�62   $	��150��6F   $��14(�6R   $��Center=�6`   $��15,0,0,0q�6{   �� 5      -   5        $ 
textBlock1��5      $
��Wrap=�6,   $��Status��6@   $��Center=�6N   $��14(�6i   $��30,0,0,0q�6w   �� 5      -   5         ��Name$ statusComboBox��5      $	��149��6/    �� SelectedIndex$ 0��6;   $��Center=�6M   $��14(�6h   $��10,0,0,0q�6v   	 �� SelectedValuePath$	 Value��6�   �� 5      -   5        
 ��Name$
 	searchBtn��5      $��Search��6(   $��75��69   $��14(�6D   $��Center=�6R   $��	120,6,0,6q�6m   6�   5      �� 5   
   $��35��6   $ 
Horizontal=�6Q   .+�5      �� -   5        $ textBlock_Copy��5      $��Airport Name��60   $��14(�6D   $��Center=�6R   $��10,0,0,0q�6m   �� 5      -   5        $ airportNameTextBox��5      $
 Wrap=�62   $	��150��6F   $��14(�6R   $��Center=�6`   $��10,7.19,0,7.19q�6{   �� 5      -   5        $
 clearBtn��5      $��Clear��6'   $��75��67   $��14(�6B   $��Center=�6P   $��	347,6,0,6q�6k   6~   5      � 5   
   $
��Auto=�6   $	��330��6?   $��
10,10,10,0q�6L   .+�5      &   System.Windows.Controls.DataGrid  -	   5          Name$ airportDataGrid��5         AutoGenerateColumns .  $	��Top=�6f      SelectionUnit$ FullRow=�6~   $��0q�5         SelectionMode$ Single=�6#      CanUserAddRows .     HorizontalGridLinesBrush �	#FFD1D1D1   VerticalGridLinesBrush �	#FFD1D1D1 �� AlternationCount$ 2��5   _      AlternatingRowBackground �	#FFEEFFA2$��0q�6�      
IsReadOnly . ��6�   Z�    	CellStyle 5      �� 5       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6    �� Triggers 5      P� 5       P� Property �@ 
IsSelected P� Value . . �5    "   ��  �� Property � �� �� Value 5!   &   �� 5"   *    �� Color$ Transparent��6:   5$   #   �� 5%   "    � ��$ 0q�6D   5&      5(         Columns 5*      0  *System.Windows.Controls.DataGridTextColumn  5+      ,  &System.Windows.Controls.DataGridColumn   Header$ Airport Code��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6_       FontSize$  14Q�6k   1  +System.Windows.Controls.DataGridBoundColumn!   Binding! 6?   �� *AirportCode+6k     5,      $ Airport Name��6)   $ * 6_   $  14Q�6i   ! 6?   �� *AirportName+6i     5-      $ Status��6)   $	 100 6T   $  14Q�6`   ! 69   �� *Status+6`   5/      50      �� 51   
   $��45��6   $ 
Horizontal=�6!   .+�52      �� -
   5        $
 newBtn��52      $	��New��6%   $��75��63   $��14(�6>   $��Center=�6L   $��10,0,0,0q�6g   6y   �� 53      -   5        $
 editBtn��53      $
��Edit��6&   $��75��65   $��14(�6@   $��Center=�6N   $��10,0,0,0q�6i   {�.  6�   54      55      56      