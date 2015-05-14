using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jas.Utils.CSVTools;

namespace Jas.Utils.AnalyticalGrid.Forms {

    internal partial class RowDetailForm : Form {

        public RowDetailForm() {
            InitializeComponent();
        }

        private void setGrid( DataGridView dg, int row ) {
            var rw = dg.Rows[row];

            for ( int i = 0; i < dg.Columns.Count; i++ ) {
                string key = dg.Columns[i].Name;
                string value = rw.Cells[i].Value.ToString();

                dgv.Rows.Add( new object[] { key, value } );
            }
        }

        public static void ShowDetail( DataGridView dg, int row ) {
            RowDetailForm fr = new RowDetailForm();
            fr.setGrid( dg, row );
            fr.Show();
        }

        private void bExport_Click( object sender, EventArgs e ) {
            if ( sd1.ShowDialog() == System.Windows.Forms.DialogResult.OK ) {
                CSVExport ex = new CSVExport();
                ex.ExportDefault( sd1.FileName, dgv );
            }
        }

        private void bClose_Click( object sender, EventArgs e ) {
            Close();
        }

        

    }

 
}
