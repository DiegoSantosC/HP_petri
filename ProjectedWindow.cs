using hp.pc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Drawing;

namespace PetriUI
{
    public class ProjectedWindow : IPcWindowManager
    {
        private Form managedWindow;

        public ProjectedWindow(Form pw)
        {
            this.managedWindow = pw;
            this.managedWindow.Size = new Size(400, 400);
        }

        // Moves top-left corner position of the managed window to pos.
        public override void Move(PcPixelPoint pos)
        {
            managedWindow.Bounds = new Rectangle(pos.X, pos.Y, managedWindow.Bounds.Width, managedWindow.Bounds.Height);

        }

        // Resizes the managed window to size.
        public override void Resize(PcPixelSize size)
        {
            managedWindow.Bounds = new Rectangle(size.Width, size.Height, managedWindow.Bounds.Width, managedWindow.Bounds.Height);
        }

        // Shows the managed window.
        public override void Show()
        {
            managedWindow.Show();
        }
    }
    class ProjectedWidowHandler
    { 
        public void HandleProjection(System.Windows.Controls.Image img)
        {
            IPcLink link = HPPC.CreateLink();

            Form matForm = new Form();

            IPcWindowRegistration registration = null;

            //Instantiates a FormWindowManager to Display the window through the WindowRegistration instance
            ProjectedWindow manager = new ProjectedWindow(matForm);

            //Requesting Sprout's HW specification
            IPcSpecification spec = link.AccessSpecification();

            IPcTouch touch = null;

            matForm.Show();

            //Event raised when the Form handle is created
            matForm.HandleCreated += (sender, eventArgs) =>
            {
                // Register this window in the Sprout Platform.
                registration = link.RegisterWindow(matForm.Handle.ToInt64());

                // Loading Sprout's touch handler for this window
                touch = link.AccessTouch(registration);
            };

             matForm.Load += (sender, eventArgs) =>
                {
                    //Display the window using the appropriate IPcWindowManager
                    registration.Display(manager);
                    
                    //Make the input layer available for the user
                    using (IPcTouchLayer layer = touch.GetTouchLayer(spec.Touch.Input))
                    {
                        layer.Enable();
                    }

                    // Adding a simple UI control (button) to allow closing the application
                    System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
                    matForm.Controls.Add(btn);
                    btn.Bounds = new Rectangle(2,2, 40, 30);
                    btn.Text = "Close";
                    btn.Click += (s, e) =>
                    {
                        matForm.Close();
                    };
                };


            //Called when the the window handle is destroyed by the application
            matForm.HandleDestroyed += (sender, eventArgs) =>
            {
                // Hiding the input layer
                using (IPcTouchLayer layer = touch.GetTouchLayer(spec.Touch.Input))
                {
                    layer.Disable();
                }

                //Unregister the window within the platform
                registration.Unregister();
            };

            //When the form is disposed we dispose remaining resources allocated in this scope
            matForm.Disposed += (sender, eventArgs) =>
            {
                touch.Dispose();
                spec.Dispose();
                registration.Dispose();
                manager.Dispose();
                link.Dispose();
            };

            Application.Run(matForm);
        }
    }
}
