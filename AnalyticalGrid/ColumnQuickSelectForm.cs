using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Jas.Utils.AnalyticalGrid.Forms {

    internal partial class ColumnQuickSelectForm : Form {

        public string ColumnSelected { get; private set; }

        public ColumnQuickSelectForm(params string[] columns) {
            InitializeComponent();

            listBox1.Items.Clear();
            listBox1.Items.AddRange( columns );
        }

        private void listBox1_DoubleClick( object sender, EventArgs e ) {
            selectColumn();
        }

        private void listBox1_KeyUp( object sender, KeyEventArgs e ) {
            if ( e.KeyCode == Keys.Return ) {
                selectColumn();
                return;
            }
        }

        private void selectColumn() {
            ColumnSelected = listBox1.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void ColumnQuickSelectForm_Load( object sender, EventArgs e ) {
            listBox1.Select();
        }

       
    }
}
