﻿#pragma checksum "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "05AAF045B80F9C9C87AE19A40026A40F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MobiGuide;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MobiGuide {
    
    
    /// <summary>
    /// AddAirportReferenceWindow
    /// </summary>
    public partial class AddAirportReferenceWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBlock;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBlock1;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBlock2;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox airportCodeTextBox;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox airportNameTextBox;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox statusComboBox;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button saveBtn;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cancelBtn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MobiGuide;component/windows/addairportreferencewindow%20-%20copy.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
            ((MobiGuide.AddAirportReferenceWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.textBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.textBlock1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.textBlock2 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.airportCodeTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 18 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
            this.airportCodeTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.airportCodeTextBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 6:
            this.airportNameTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.statusComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.saveBtn = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
            this.saveBtn.Click += new System.Windows.RoutedEventHandler(this.saveBtn_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.cancelBtn = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\Windows\AddAirportReferenceWindow - Copy.xaml"
            this.cancelBtn.Click += new System.Windows.RoutedEventHandler(this.cancelBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
