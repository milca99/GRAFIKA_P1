   M S B A M L   `   `   `  ����\  XGMap.NET.WindowsForms, Version=1.7.0.0, Culture=neutral, PublicKeyToken=b85b9027b614afef\Bclr-namespace:GMap.NET.WindowsForms;assembly=GMap.NET.WindowsFormsGMap.NET.WindowsForms  5   	   T PGMap.NET.Core, Version=1.7.0.0, Culture=neutral, PublicKeyToken=b85b9027b614afef:-clr-namespace:GMap.NET;assembly=GMap.NET.CoreGMap.NET 5   	   [ WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms 5   	   B >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1 5	   	      WpfApp1.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   RgmfBclr-namespace:GMap.NET.WindowsForms;assembly=GMap.NET.WindowsForms     5   	   <gm-clr-namespace:GMap.NET;assembly=GMap.NET.Core     5   	   Owf@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     5   	   'localclr-namespace:WpfApp1     5	   	     � Title$  
MainWindow��5   	   $��1013.502��6   $
��1071��6.   
�������.��5      � .+�5   
   ]� .+�5      �� $	��Top=�6   
�������$��52��6A   .+�5      �� -   5         ��Name$ 
LoadButton��5      $��
Load Model��6-   $	��150��6[   $��5q�6g   �� 5      -   5         ��Name$ slider��5       � Minimum$ .1Q�6)    � Maximum$ 10Q�66    � Value$ 1Q�6C   $	��Top=�6M   5      ]� 5      $��0,5,0,0q�6   .+�5      �� $��	0,100,0,0q�6   $��66��60   .+�5      �� -   5         ��Name$ ellipse��5      
����������#47ADE9$��Ellipse��6�   �� 5      -   5        $ polygon��5      
����������#59B6EC$��Polygon��6�   �� 5      -   5        $
 text��5      
����������#6DBDEC$
��Text��6�   �� 5      -   5        $
 Undo��5      $��1,0,0,2q�6+   
�������
�������$
��Undo��6u   $��47��6�   �� 5      -   5        $
 Redo��5      $��1,0,0,2q�6+   
�������
�������$
��Redo��6u   $��47��6�   �� 5      -   5        $ Clear��5      $��1,0,0,2q�6,   
�������
�������$��Clear��6w   $��47��6�   5      �� 5      $
��Auto=�6   $
��Auto=�6E   .��5      �� -	   5         ��Name$ 	canvasMap��5   1   
�������$	��896��6D   $	��894��6Q    � LayoutTransform 5      �� 5       	 �� ScaleX	 6-   �� 
 �� ElementName$
 slider�� �� Path$ Value� �� ScaleY 5!   %   �� $
 slider��$ Value�.+�5#      �� $��1q�6!   
��� � � �� Left$ 10��6I   
 �� Top$ 10��6Z   5&      5,      5/      50      52      53      54      