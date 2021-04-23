using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TgenRemoter
{
    public partial class Graphic_Tester : Form
    {
        public Graphic_Tester()
        {
            InitializeComponent();
        }

        private void Graphic_Tester_Load(object sender, EventArgs e)
        {
            screenSharePictureBox.Location = Point.Empty;
            screenSharePictureBox.Width = Width - 20;
            screenSharePictureBox.Height = Height - 40;
            SizeChanged += Graphic_Tester_SizeChanged;
        }

        private void Graphic_Tester_SizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("CHANGED");
            screenSharePictureBox.Width = Width - 20;
            screenSharePictureBox.Height = Height - 40;
        }
    }
}
