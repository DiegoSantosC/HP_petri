﻿#pragma checksum "..\..\CapturePreviews.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "96B5ED1074DF1179E1D1D2C7134C3600"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PetriUI;
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


namespace PetriUI {
    
    
    /// <summary>
    /// CapturePreviews
    /// </summary>
    public partial class CapturePreviews : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 35 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel navigationSp;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel leftSp;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel rightSp;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ShowButton;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label infoLabel;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label emptyLabel;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border Border1;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border Border2;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\CapturePreviews.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel sampleSP;
        
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
            System.Uri resourceLocater = new System.Uri("/PetriUI;component/capturepreviews.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CapturePreviews.xaml"
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
            this.navigationSp = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.leftSp = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.rightSp = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 4:
            this.ShowButton = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\CapturePreviews.xaml"
            this.ShowButton.Click += new System.Windows.RoutedEventHandler(this.showDetails);
            
            #line default
            #line hidden
            return;
            case 5:
            this.infoLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.emptyLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.Border1 = ((System.Windows.Controls.Border)(target));
            return;
            case 8:
            this.Border2 = ((System.Windows.Controls.Border)(target));
            return;
            case 9:
            this.sampleSP = ((System.Windows.Controls.StackPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

