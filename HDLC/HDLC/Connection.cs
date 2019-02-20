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
    public partial class Connection : UserControl
    {
        public Connection()
        {
            InitializeComponent();
        }

        public void ConnectBtHandler(EventHandler handler)
        {
            this.bunifuImageButton1.Click += handler;
        }

        public void DisconnectBtHandler(EventHandler handler)
        {
            this.bunifuImageButton2.Click += handler;
        }
    }
}
