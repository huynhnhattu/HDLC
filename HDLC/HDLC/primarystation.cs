using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDLC
{
    public partial class primarystation : UserControl
    {
        public primarystation()
        {
            InitializeComponent();
        }
        public void StartBtHandler(EventHandler handler)
        {
            this.button1.Click += handler;
        }

        public void StopBtHandler(EventHandler handler)
        {
            this.button2.Click += handler;
        }
        public void CbTextIndexChanged(EventHandler handler)
        {
            this.comboBox1.SelectedIndexChanged += handler;
        }
    }

}
