   M S B A M L   `   `   `  ����B  >PetriUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:PetriUIPetriUI  5          PetriUI.CaptureWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      'localclr-namespace:PetriUI     5      -   5          � Title$  CaptureWindow��5      .��5      �  � ColumnDefinitions 5   
   �� 5      $	��600 �6   �� 5      $	��600 �6    � RowDefinitions 5   
   �� 5      $	��100 �6   �� 5      $��90 �6   �� 5      �� 5      $	��100 �6   .+�5   
   �� $��0��6   $��2��6   ���#00BFFF62   �� 5   
   $��0��6   $��1��6    $��20q�6-   $	��Top=�69   $
��Left=�6Q      headerStyle#��[  6l   .��6�   Capture details6�   �� 5#   
   -   5        $��1��5#      $��3��6!     button2Style#��[ 6.   $��Bottom=�6T   $��	0,2,50,40q�6o   $	��169��6�   $��40��6�   .��6�   Stop Capturing6�   �� 5&   
   -   5        $��
SaveButton��5&      $��1��6#   $��1��63   #��[ 6@   $y�Hidden=�6f   $��	0,0,50,20q�6z   $	��169��6�   $��40��6�   .��6�   Save6�   �� 5)   
   -   5        $��ProjectButton��5)   !   $��0��6   $y�Hidden=�66   $��3��6K   #��[ 6X   $
��Left=�5*      $��Bottom=�6,   $��	30,0,0,40q�6G   $	��220��6Z   $��40��6f   .��6�   Project into Mat Screen6�   �� 5.   
   -   5        $��ImagesCanvas��5.      $��10q�6%   $��1��62   $��2��6B   �� 51   
   -   5        $��LastImageCanvas��51      $��50q�6(   $��0��65   $��2��6E   �� 55   
   -   5        $��finishedCapture��55   a   $��1��6   $��3��6!   $
��Left=�6.   $	��Top=�6I   ���#990E0E._�56      �� -   5        $��RunningLabel��56      $��15(�6(   
�������.��6I   Capture Running6Z   57      �� 5:   
   -	   5        $��	infoLabel��5:      $��1��6!   $��1��61   $
��Left=�6>   $��Bottom=�6Z   $y�Hidden=�6u   .��6�   :8Click capture to focus (last capture focused by default)6�   �� 5=   
   -
   5        $��	timeLabel��5=      $��0��6!   $��2��61   $
��Left=�6>   $��Bottom=�6Z   $y�Hidden=�6u   $��	80,0,0,30q�6�   .��6�   Capture taken at:6�   5?      5@      