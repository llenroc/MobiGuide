   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	   !    MobiGuide.TextDisplayWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	   -   5          � Title$  TextDisplayWindow��5   	   $	��500��6#   $	��600��60    � 
ResizeMode$ NoResize=�6<    � WindowStartupLocation$ CenterScreen=�6R   .��5	      �� .+�5
   
   ��  �� Orientation$ 
Horizontal=�6   $��37��6.   $��Center=�6:   $��0,10,0,0q�6U   .+�5      �� -   5         ��Name$ 	textBlock��5      $
��Wrap=�6+   $��14(�6?   $��Language Code��6M   $��10,0,0,0q�6b   $��Center=�6t   �� 5      -   5         ��Name$ languageCodeTextBox��5       �� TextWrapping$
 Wrap=�63   $	��200��6G   $��14(�6S   $��Center=�6a   $��25,0,0,0q�6|   {�.  6�   5      �� 5   
   $ 
Horizontal=�6   $��37��6.   $��Center=�6:   $��0,10,0,0q�6U   .+�5      �� -   5        $ 
textBlock1��5      $
��Wrap=�6,   $��14(�6@   $��Language Name��6N   $��10,0,0,0q�6c   $��Center=�6u   �� 5      -   5        $ languageNameTextBox��5      $
 Wrap=�63   $	��200��6G   $��14(�6S   $��Center=�6a   $��20,0,0,0q�6|   {�.  6�   5      �� 5   
   $ 
Horizontal=�6   $��Center=�6.   $��0,10,0,0q�6I   .+�5      �� -   5        $ 
textBlock2��5      $
��Wrap=�6,   $��14(�6@   $��Display Text��6N   $��10,7,0,0q�6b   $	��Top=�6t   �� 5      -   5        $ displayTextTextBox��5      $
 Wrap=�62   $	��430��6F   $��14(�6R   $��Center=�6`   $��48,8.19,0,0q�6{   $	��300��6�   5      �� 5   
   $ 
Horizontal=�6   $��
0,15,33,10q�6.   $��Right=�6B   .+�5      �� -   5         ��Name$ saveBtn��5      $
��Save��6&   $��75��65   $��14(�6@   $��Center=�6N   $��0q�6i    �� 	IsDefault . 6�   �� 5      -	   5        $ closeBtn��5      $��Close��6'   $��75��67   $��14(�6B   $��Center=�6P   $��10,0,0,0q�6k   	 �� IsCancel	 . 6�   5      5      5      