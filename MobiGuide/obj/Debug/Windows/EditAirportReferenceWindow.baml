   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	   *    $MobiGuide.EditAirportReferenceWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	   -   5          � Title$  Edit Airport Reference��5   	   $	��224��6(   $	��300��65    � WindowStartupLocation$ CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�6.    � SizeToContent$ Width=�5
       .��5      � .+�5   
   �� $	��Top=�6   $	��149��6-    �� Orientation$ 
Horizontal=�6:   $	��294��6   .+�5      �� $	��100��6   $��0q�6%   $	��150��60   .+�5      �� -   5         ��Name$ 	textBlock��5      $
��Wrap=�6/   $��Airport Code��6C   $��	10,10,0,0q�6W   $��14(�6j   �� 5      -   5        $ airportNameTextBlock��5      $
��Wrap=�6:   $��Airport Name��6N   $��14(�6b   $��	10,10,0,0q�6p   $y�Hidden=�6�   �� 5      -   5        $ statusTextBlock��5      $
��Wrap=�65   $��Status��6I   $��14(�6W   $��	10,10,0,0q�6e   $y�Hidden=�6x   �� 5      -   5        $ commitByTextBlock��5      $
��Wrap=�67   $��	Commit By��6K   $��14(�6\   $��	10,10,0,0q�6j   $y�Hidden=�6}   �� 5      -   5        $ commitDateTimeTextBlock��5      $
��Wrap=�6=   $��Commit Time��6Q   $��14(�6d   $��	10,10,0,0q�6r   $y�Hidden=�6�   5      �� 5      $	��194��6   $	��150��6%   .+�5      �� -   5         ��Name$ airportCodeComboBox��5       �� SelectedIndex$ 0��68   $��10,7,0,0q�6J   $
��Left=�6\   $��14(�6w    �� SelectedValuePath$ Value��6�   �� 5      -   5        	 ��Name$	 airportNameTextBox��5      $��23��66   $��10,5,0,0q�6B   $��14(�6T   $	��165��6b   $
��Left=�6n   $y�Hidden=�6�   6�   �� 5      -	   5        $ statusComboBox��5      $��	10,5,10,0q�63   $��14(�6F   $ 0��6T   $
��Left=�6f   $ Value��6�   $y�Hidden=�6�   �� 5      -
   5        $ commitByTextBlockValue��5      $��14(�6<   $��	10,7,10,0q�6J   $
��Bold"�6]   $y�Hidden=�6o   �� 5      -   5        $" commitDateTimeTextBlockValue��5      $��14(�6B   $��	10,9,10,0q�6P   $
��Bold"�6c   $y�Hidden=�6u   5      5      �� 5   
   $��41��6   $��Bottom=�6!   $ 
Horizontal=�6<   $��Center=�6U   .+�5      �� -   5        
 ��Name$
 saveBtn��5      $
��Save��6&   $��25��65   $��0,0,10,0q�6A   $��60��6S    �� 	IsDefault . {�.  6�   �� 5      -   5        $
 	cancelBtn��5      $��Close��6(   $��60��68   $��25��6C    �� IsCancel . 6O   5      5       5!      