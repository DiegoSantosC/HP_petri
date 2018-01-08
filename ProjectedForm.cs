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
            matForm.StartPosition = FormStartPosition.Manual;
            matForm.Top = 1200;
            matForm.Left = 600;

            matForm.Show();

            LoadContent(matForm, img);

            System.Windows.Forms.Application.Run(matForm);

        }

       
        private void LoadContent(Form matForm, System.Windows.Controls.Image img)
        {
            
            PictureBox pb = new PictureBox();
         
            //Conversion form Controls.Image into Drawing.Image

            MemoryStream ms = new MemoryStream();
            BmpBitmapEncoder bmpEncoder = new BmpBitmapEncoder();
            bmpEncoder.Frames.Add(BitmapFrame.Create(new Uri(img.Source.ToString(), UriKind.RelativeOrAbsolute)));

            bmpEncoder.Save(ms);

            System.Drawing.Image drawingImage = System.Drawing.Image.FromStream(ms);

            System.Drawing.Image resized = (System.Drawing.Image)(new Bitmap(drawingImage, new System.Drawing.Size(drawingImage.Size.Height*280/(int)System.Windows.SystemParameters.PrimaryScreenHeight, drawingImage.Size.Width*280/ (int)System.Windows.SystemParameters.PrimaryScreenHeight)));
            
            pb.Image = resized;
            pb.Height = resized.Height;
            pb.Width = resized.Width;
         
            matForm.Height = (int)(resized.Height * 1.2);
            matForm.Width = (int)(resized.Width * 1.2);
            matForm.Controls.Add(pb);
             
        }
    }
}
