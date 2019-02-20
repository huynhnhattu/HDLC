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
    public partial class secondarystation : UserControl
    {
        public secondarystation()
        {
            InitializeComponent();
        }
        public void CbbSelectedChanged(EventHandler handler)
        {
            this.comboBox1.SelectedIndexChanged += handler;
        }
    }
}
