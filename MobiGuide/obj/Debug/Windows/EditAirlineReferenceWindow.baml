   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	   *    $MobiGuide.EditAirlineReferenceWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	   X TXceed.Wpf.Toolkit, Version=3.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   -   5          � Title$  Edit Airline Reference��5	   	   $	��542��6(   $	��500��65    � 
ResizeMode$ NoResize=�5
   	    � WindowStartupLocation$ CenterScreen=�6    � SizeToContent$ Width=�5   	    � 	Resources 5         sysFonts �� Source   System.Windows.Media.Fonts   SystemFontFamilies �� Source  savedScrollViewer �� 
TargetType	 �Name
 � ColumnDefinitions � RowDefinitions ��Name ��Name �� CanContentScroll �� CanHorizontallyScroll �� CanVerticallyScroll ��Name4  .System.Windows.Automation.AutomationProperties   AutomationId � Cursor �� ScrollableHeight � Maximum � Minimum* �� #ComputedVerticalScrollBarVisibility �� ViewportHeight �� ViewportSize � Value �� Mode �� RelativeSource �� ScrollableWidth �� Orientation, �� %ComputedHorizontalScrollBarVisibility  �� ViewportWidth��%  &	        &	 (     �� 5   
    6   �� # Z 6]   �� 5   
   " ��64   .R�5      � $
	 Grid��6   #��z��6!   
 5      �� 5      $��* �6'   �� 5      $
��Auto �6'    5      �� 5      $��* �6$   �� 5      $
��Auto �6$   .+�5      �� $ Corner��6   $��1��6,   #��� ��6<   $��1��6}   �� 5      $! PART_ScrollContentPresenter��6)   # z��6N    .   .  #��z��6�   #��z��6�   $��0��6  #��z��6&  $��0��6I  �� 5      $ PART_VerticalScrollBar��6   $ VerticalScrollBar��6<   $ Arrow��6r   $��1��6�   # z 6�   $ 0Q�6�   $��0��6�   #y�z 6�   # z 6y   6  �� *VerticalOffset+$ OneWay=� �� *TemplatedParent+6y  �� 5      $ PART_HorizontalScrollBar��6   $ HorizontalScrollBar��6>   $ Arrow��6v   $��0��6�   # z 6�   $ 0Q�6�   $ 
Horizontal=�6�   $��1��6�   #y�z 6�   # z  6�   68  �� *HorizontalOffset+$ OneWay=� �� *TemplatedParent+6�  5      5      5      .��5      �� .+�5    
   �� $��0,0,0,10q�6G   $	��450��6Y   #���  6f   .��5!      �� .+�5"      �� ! �� Orientation$! 
Horizontal=�6   .+�5#      �� -   5        " ��Name$" 
stackTitle��5#   !   .+�5$      �� $
��Wrap=�6$   $��Airline Code��68   $��14(�6L   $��
10,10,0,10q�6Z   �� 5%      $
��Wrap=�6$   $��Airline Name��68   $��14(�6L   $��	10,0,0,10q�6Z   �� 5&      $	��0.5q�6!   
���   �$��	10,0,10,9q�6K   ._�5'      �� -   5        # ��Name$# alLargeLogoImg��5'   $   $��149.6��6<   $	��150��6K   5(      �� 5)      $	��0.5q�6!   
���   �$��	10,0,10,9q�6K   ._�5*      �� -   5        $# alSmallLogoImg��5*   $   $��149.6��6<   $	��150��6K   5+      �� 5,      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��	Font Name��6Y   �� 5-      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��	Font Size��6Y   �� 5.      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��
Font Color��6Y   �� 5/      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��Background Color��6Y   �� 50      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��
Line Color��6Y   �� 51      $
��Wrap=�6$   $��14(�68   $��	10,0,0,12q�6F   $��
Seat Color��6Y   �� 52      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��Display Guidance Time (s)��6Y   �� 53      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��Status��6Y   �� 54      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��	Commit By��6Y   �� 55      $
��Wrap=�6$   $��14(�68   $��	10,0,0,10q�6F   $��Commit Time��6Y   56      �� 57      -   5        $" stackDetail��57   !   $��0,0,10,0q�6P   .+�58      �� -   5        $ ��Name$$ alCodeTxtBlock��58   $   $
��Wrap=�6<   $��14(�6P   $��	10,10,0,9q�6^   $
��Bold"�6q   $��[Airline Code]��6�   �� 59      -   5        % ��Name$% alNameTxtBox��59   "   $��20��68   & �� TextWrapping$
& Wrap=�6D   $��14(�6X   $��10,0,0,9q�6f   $��[Airline Name]��6x   �� 5:      $��0,35,0,0q�6%   .+�5;      �� -   5        $$ 
textBlock1��5;   (   $
��Wrap=�6<   $��Airline Large Logo��6P   $��14(�6j   $��10,0,0,0q�6x   �� 5<      $! 
Horizontal=�6)   $��	10,10,0,0q�6\   .+�5=   "   �� -	   5        $% srcLargeLogoTxtBox��5=   *   $
& Wrap=�6F   $��0q�6Z   $��14(�6e   $��1,0,10,1q�6s   ���	#FFABADB3{�.  $	��220��6�   �� 5>   "   -
   5        ' ��Name$' browseSrcLargeLogoBtn��5>   )   $��Browse��6H   $��Center=�6Y   $��5,0,0,0q�6{   $	��2,1q�6�   $��14(�6�   6�   5?      �� 5@      -   5        $' removeLargeLogoBtn��5@   %   $
��Left=�6A   $��	10,15,0,0q�6\   
���  ��$��11��6�   $��0,-5,0,0q�6�   $��0q�6�   $y�Hidden=�6�   ��6o   Z� ��6�   Z� .��5A   "   �� B� 6-   .�68   �� $�Remove��6<   6M   6Y   5B      5C      �� 5D      $��0,82,0,0q�6%   .+�5E      �� -   5        $$ 
textBlock2��5E   (   $
��Wrap=�6<   $��Airline Small Logo��6P   $��14(�6j   $��10,0,0,0q�6x   �� 5F      $! 
Horizontal=�6)   $��	10,10,0,0q�6\   .+�5G   "   �� -   5        $% srcSmallLogoTxtBox��5G   *   $
& Wrap=�6F   $��0q�6Z   $��14(�6e   $��1,0,10,1q�6s   ���	#FFABADB3{�.  $	��220��6�   �� 5H   "   -   5        $' browseSrcSmallLogoBtn��5H   )   $��Browse��6H   $��Center=�6Y   $��5,0,0,0q�6{   $	��2,1q�6�   $��14(�6�   6�   5I      �� 5J      -   5        $' removeSmallLogoBtn��5J   %   $
��Left=�6A   $��	10,15,0,0q�6\   
���  ��$��11��6�   $��0,-5,0,0q�6�   $��0q�6�   $y�Hidden=�6�   ��6o   Z� ��6�   Z� .��5K   "   �� B� 6-   .�68   �� $�Remove��6<   6M   6Y   5L      5M      �� 5N      -   5        ( ��Name$( fontNameComboBox��5N   #   $��3��6=   $��	10,49,0,2q�6J   $
��Left=�6]   $��23��5O   V   $	��Top=�6b   $��2��6z   ��6   �� # [  ��5P      �� 5Q   "   .R�5R   &   $� 5S   #   ��5U      �� 5V   "   .R�5W   &   �� $��20��6X   ��60   �� ��6A   �� 6X   5X   #   5Z      %  Xceed.Wpf.Toolkit.IntegerUpDown  5[      -   5        )  Name$) fontSizeUpDown��5[   -   $
��Left=�6E   $��10,4,0,3q�6`   �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]*   Value$	* 100X�6r   +   Minimum$+ 10X�6~   ,   Maximum$	, 100X�6�   #  Xceed.Wpf.Toolkit.ColorPicker  5\      -   5        -  Name$- fontColorPicker��5\   +   $��10,0,0,6q�6D   .   DisplayColorAndName. . $
��Left=�6q   $��Stretch=�6�   $	��100��6�   $��0,0,3,0q�6�   $��14(�6�   �� 5]      $! 
Horizontal=�6%   $��10,0,0,3q�6>   $
��Left=�6P   .+�5^        -   5        $- backgroundColorPicker��5^   /   . . $
��Left=�6i   $��Stretch=�6�   $	��100��6�   $��0,0,3,0q�6�   $��14(�6�   $��0q�6�   6�     5_      -   5        $- backgroundColorPicker2��5_   /   /   SelectedColor$/ TransparentX�6O   . . $
��Left=�6�   $��Stretch=�6�   $	��100��6�   $��0,0,3,0q�6�   $��14(�6�   $��10,0,0,0q�6�   6  �� 5`      -   5        $ backgroundPreview��5`   (   
�������$��22��6Q   $��10,0,0,0q�6]   $��22��6o   $	��0.2��6z   0 � ToolTip$0 Preview��6�   5a        5b      -   5        $- lineColorPicker��5b   +   $��10,0,0,3q�6D   . . $
��Left=�6q   $��Stretch=�6�   $	��100��6�   $��0,0,3,0q�6�   $��14(�6�     5c      -   5        $- seatColorPicker��5c   +   $��10,0,0,6q�6D   . . $
��Left=�6q   $��Stretch=�6�   $	��100��6�   $��0,0,3,3q�6�   $��14(�6�     5d      -   5        $) dispGuideTimeIntUpDown��5d   -   $��10,0,0,0q�6M   $
��Left=�6_   $��45��6z   $* 1X�6�   1   DefaultValue$1 1X�6�   $+ 1X�6�   $	, 999X�6�   $��14(�6�   �� 5e      -   5        $( statusCodeComboBox��5e   #   $��10,5,0,3q�6?   2 �� SelectedIndex$2 0��6Q   $
��Left=�6c   $��14(�6~   3 �� SelectedValuePath$3 Value��6�   �� 5f      -   5        $$ commitByTxtBlock��5f   $   $
��Wrap=�6>   $��14(�6R   $��10,4,0,9q�6`   $��[Commit By]��6r   �� 5g      -   5        $$ commitTimeTxtBlock��5g   $   $
��Wrap=�6@   $��14(�6T   $��10,1,0,9q�6b   $��[Commit Time]��6t   5h      5i      5j      5k      �� 5l   
   $! 
Horizontal=�6   $��Center=�6.   $��0,4,0,0q�6K   .+�5m      �� -   5        $' saveBtn��5m      $
��Save��6&   $��25��65   $��0,0,10,0q�6A   $��60��6S   4 �� 	IsDefault4 . 6t   �� 5n      -   5        $' 	cancelBtn��5n      $��Cancel��6(   $��60��69   $��25��6D   5 �� IsCancel5 . 6h   5o      5p      5q      