   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	   *    $MobiGuide.AddLanguageReferenceWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	   -   5          � Title$  Add Language Reference��5   	   $	��181��6(   $	��300��65    � WindowStartupLocation$ CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�6.   .��5
      � .+�5   
   �� $	��Top=�6   $	��106��6-    �� Orientation$ 
Horizontal=�6:   $	��294��6   .+�5      �� $	��115��6   .+�5      �� -   5         ��Name$ 	textBlock��5      $
��Wrap=�6/   $��Language Code��6C   $��	10,10,0,0q�6X   $��14(�6k   �� 5      -   5        $ 
textBlock1��5      $
��Wrap=�60   $��Language Name��6D   $��14(�6Y   $��	10,10,0,0q�6g   �� 5      -   5        $ 
textBlock2��5      $
��Wrap=�60   $��Status��6D   $��14(�6R   $��	10,10,0,0q�6`   5      �� 5      $	��194��6   .+�5      �� -   5         ��Name$ languageCodeTextBox��5      $��23��67   $��10,9,0,0q�6C   $��34.667��6U   $
��Left=�6d   $��14(�6    �� 	MaxLength$ 3��6�   �� 5      -   5        $ languageNameTextBox��5      $��23��67   $��10,5,0,0q�6C   $��14(�6U   $	��165��6c   $
��Left=�6o   �� 5      -   5         ��Name$ statusComboBox��5      $��	10,5,10,0q�63   $��14(�6F    �� SelectedIndex$ 0��6T   $
��Left=�6f   	 �� SelectedValuePath$	 Value��6�   5      5      �� 5   
   $��41��6   $��Bottom=�6!   $ 
Horizontal=�6<   $��Center=�6U   .+�5      �� -   5        
 ��Name$
 saveBtn��5      $
��Save��6&   $��25��65   $��0,0,10,0q�6A   $��60��6S    �� 	IsDefault . 6t   �� 5      -	   5        $
 	cancelBtn��5      $��Cancel��6(   $��60��69   $��25��6D    �� IsCancel . 6h   5      5      5      