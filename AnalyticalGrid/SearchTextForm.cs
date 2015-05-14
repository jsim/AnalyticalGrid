using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Jas.Utils.AnalyticalGrid.Forms {
    internal partial class SearchTextForm : Form {

        public string SearchText {
            get {
                return textBox1.Text;
            }
        }

        public SearchTextForm() {
            InitializeComponent();
        }

        private void SearchTextForm_Load( object sender, EventArgs e ) {
            textBox1.Select();
        }

        private void textBox1_KeyUp( object sender, KeyEventArgs e ) {
            if ( e.KeyCode == Keys.Return ) {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }

            if ( e.KeyCode == Keys.Escape ) {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            }
        }
    }
}
