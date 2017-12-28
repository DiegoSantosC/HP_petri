using hp.pc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Drawing;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using hp.pc.specification;

namespace PetriUI
{
    public class ProjectedForm : IPcWindowManager
    {
        private Form managedWindow;

        public ProjectedForm(Form pw)
        {
            this.managedWindow = pw;
            this.managedWindow.Size = new System.Drawing.Size(400, 400);
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
    class ProjectedFormHandler
    {
        public void HandleProjection(System.Windows.Controls.Image img)
        {
            IPcLink link = HPPC.CreateLink();

            Form matForm = new Form();
            matForm.Height = 400;
            matForm.Width = 400;

            IPcWindowRegistration registration = null;

            //Instantiates a FormWindowManager to Display the window through the WindowRegistration instance
            ProjectedForm manager = new ProjectedForm(matForm);

            //Requesting Sprout's HW specification
            IPcSpecification spec = link.AccessSpecification();

            IPcTouch touch = null;

            //Event raised when the Form handle is created
            matForm.HandleCreated += (sender, eventArgs) =>
            {
                Console.WriteLine("handle created");

                // Register this window in the Sprout Platform.
                registration = link.RegisterWindow(matForm.Handle.ToInt64());

                // Loading Sprout's touch handler for this window
                touch = link.AccessTouch(registration);
            };

            matForm.Load += (sender, eventArgs) =>
            {

                Console.WriteLine("loaded");

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
                btn.Bounds = new Rectangle(2, 2, 40, 30);
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

            matForm.Show();

            LoadContent(matForm, img);

            System.Windows.Forms.Application.Run(matForm);

            PlaceContent(registration, link, matForm, touch, manager, spec);

        }

        private void PlaceContent(IPcWindowRegistration registration, IPcLink link, Form matForm, IPcTouch touch, ProjectedForm manager, IPcSpecification spec)
        {
            // Register this window in the Sprout Platform.
            registration = link.RegisterWindow(matForm.Handle.ToInt64());

            // Loading Sprout's touch handler for this window
            touch = link.AccessTouch(registration);

            Console.WriteLine("Traza 3");
            //Display the window using the appropriate IPcWindowManager
            registration.Display(manager);

            Console.WriteLine("Traza 2");
            //Make the input layer available for the user

            IPcTouchLayerOption l = spec.Touch.Input;

            IPcTouchLayer layer = touch.GetTouchLayer(l);
            
            layer.Enable();
            Console.WriteLine("Traza 1");
            
        }

        private void LoadContent(Form matForm, System.Windows.Controls.Image img)
        {
            
            // Adding a simple UI control (button) to allow closing the application
            System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
            matForm.Controls.Add(btn);
            btn.Bounds = new Rectangle(2, 2, 50, 40);
            btn.Text = "Close";
            btn.Click += (s, e) =>
            {
                matForm.Close();
            };

            PictureBox pb = new PictureBox();
            img.Width = 400;
            img.Height = 400;

            //Conversion form Controls.Image into Drawing.Image

            MemoryStream ms = new MemoryStream();
            BmpBitmapEncoder bmpEncoder = new BmpBitmapEncoder();
            bmpEncoder.Frames.Add(BitmapFrame.Create(new Uri(img.Source.ToString(), UriKind.RelativeOrAbsolute)));

            bmpEncoder.Save(ms);

            System.Drawing.Image drawingImage = System.Drawing.Image.FromStream(ms);
            
            pb.Image = drawingImage;
            pb.Height = 400;
            pb.Width = 400;
            matForm.Controls.Add(pb);        
        }
    }
}
