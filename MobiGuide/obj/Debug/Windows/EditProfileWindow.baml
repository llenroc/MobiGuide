   M S B A M L   `   `   `  ����D  @MobiGuide, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MobiGuide	MobiGuide  5   	   !    MobiGuide.EditProfileWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MobiGuide     5   	   -   5          � Title$  Edit User Profile��5   	   $	��370��6#   $	��300��60    � WindowStartupLocation$ CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�6.    � SizeToContent$ Width=�5
   	   $	��380��66   .��5      �� .+�5   
   ��  �� Orientation$ 
Horizontal=�6   .+�5      �� -   5         ��Name$ 
stackTitle��5      .+�5      �� -   5         ��Name$ 	textBlock��5      $
��Wrap=�6/   $��Username��6C   $��14(�6S   $��
10,10,0,10q�6a   �� 5      -   5        $ textBlock_Copy��5      $
��Wrap=�64   $��Airline Name��6H   $��14(�6\   $��	10,0,0,10q�6j   �� 5      -   5        $ textBlock_Copy1��5      $
��Wrap=�65   $��
First Name��6I   $��14(�6[   $��	10,0,0,10q�6i   �� 5      -   5        $ textBlock_Copy2��5      $
��Wrap=�65   $��	Last Name��6I   $��14(�6Z   $��	10,0,0,10q�6h   �� 5      -   5        $ textBlock_Copy5��5      $
��Wrap=�65   $��Status��6I   $��14(�6W   $��	10,0,0,10q�6e   �� 5      -   5        $ textBlock_Copy3��5      $
��Wrap=�65   $��New Password��6I   $��14(�6]   $��	10,0,0,10q�6k   �� 5      -	   5        $ textBlock_Copy4��5      $
��Wrap=�65   $��Confirm Password��6I   $��14(�6a   $��	10,0,0,10q�6o   �� 5      -
   5        $ textBlock_Copy6��5      $
��Wrap=�65   $��	Commit By��6I   $��14(�6Z   $��	10,0,0,10q�6h   �� 5      -   5        $ textBlock_Copy7��5      $
��Wrap=�65   $��Commit Time��6I   $��14(�6\   $��	10,0,0,10q�6j   5      �� 5      -   5        $ stackDetail��5      $��0,0,10,0q�6.   $	��230��6@   .+�5      �� -   5        $ uNameTxtBlock��5      $
��Wrap=�63   $��14(�6G   $��
10,10,0,10q�6U   $
��Bold"�6i   �� 5      -   5        $ alNameTxtBlock��5      $
��Wrap=�64   $��14(�6H   $��	10,0,0,10q�6V   $
��Bold"�6i   �� 5      -   5         ��Name$ firstNameTxtBox��5      $��20��63    �� TextWrapping$
 Wrap=�6?   $��14(�6S   $��10,0,0,8q�6a   �� 5      -   5        $ lastNameTxtBox��5      $��20��62   $
 Wrap=�6>   $��14(�6R   $��10,0,0,8q�6`   �� 5      -   5        	 ��Name$	 statusComboBox��5      $��10,0,0,9q�63   
 �� SelectedIndex$
 0��6E   $
��Left=�6W   G� 5      -   5         G�Name$ 	newPwdBox��5      $��14(�61   $��10,0,0,8q�6?   G� 5      -   5        $ 	cfmPwdBox��5      $��14(�61   $��10,0,0,7q�6?   �� 5       -   5        $ commitByTxtBlock��5       $
��Wrap=�66   $��14(�6J   $��10,0,0,9q�6X   �� 5!      -   5        $ commitDateTxtBlock��5!      $
��Wrap=�68   $��14(�6L   $��	10,0,0,10q�6Z   5"      5#      �� 5$   
   $ 
Horizontal=�6   $��Center=�6.   $��0,20,0,0q�6K   .+�5%      �� -   5         ��Name$ saveBtn��5%      $
��Save��6&   $��25��65   $��0,0,10,0q�6A   $��60��6S    �� 	IsDefault . 6t   �� 5&      -   5        $ 	cancelBtn��5&      $��Cancel��6(   $��60��69   $��25��6D    �� IsCancel . 6h   5'      5(      5)      