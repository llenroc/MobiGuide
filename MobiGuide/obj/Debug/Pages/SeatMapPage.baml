   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5          MobiGuide.SeatMapPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      )localclr-namespace:MobiGuide     5      -   5          N� Title$  SeatMapPage��5	      $
��Auto��60   $
��Auto��6=   .��5      � .+�5   
   �� -   5         ��Name$ imageCanvas��5      $��Center=�6&   $��0q�6C   .+�5      �� -   5         ��Name$ 
seatMapImg��5      5      �� 5   
   $
��Left=�6   $	��Top=�60    �� Orientation$ 
Horizontal=�6H   $��	10,10,0,0q�6a   .+�5      �� -   5         ��Name$ 	textBlock��5      $
��Wrap=�6+   $��	Show Seat��6?   �� 5      -   5         ��Name$ seatComboBox��5      $	��120��6-   $��10,0,0,0q�69    �� SelectedValuePath$ Value��6|   5      (� 5   
   -   5         (�Name$ seatNumberViewBox��5      $��Right=�6-   $	��Top=�6I   $��0,0,20,0q�6a   .��5      �� -   5        $ 
textBlock1��5      $
��Wrap=�6,   5      �� 5   
   $
��Left=�6   $	��Top=�60   $ 
Horizontal=�6H   $��	10,40,0,0q�6a   .+�5      �� -   5        $ 
textBlock2��5      $
��Wrap=�6,   $��Barcode��6@   �� 5      -	   5         ��Name$ barCodeTextBox��5      $	��100��6.   	 �� TextWrapping$
	 Wrap=�6;   $	��200��6O   $��23,0,0,0q�6[   �� 5      -
   5        
 ��Name$
 
extractBtn��5      $��Extract��6)   $��75��6;   $��10,0,0,0q�6F   $	��Top=�6q   5      5      5      