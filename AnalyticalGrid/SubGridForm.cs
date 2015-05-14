using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Jas.Utils.AnalyticalGrid.Forms {

    internal partial class SubGridForm : Form {

        public SubGridForm() {
            InitializeComponent();
            dgv.SumAction += new AnalyticalGrid.OnSumAction( dgv_SumAction );
        }

        void dgv_SumAction( string text, decimal? sum, decimal? minNeg, decimal? maxNeg, decimal? minPos, decimal? maxPos ) {
            int rc = dgv.Rows.Count - 1;
            decimal avg = 0;
            if ( rc != 0 && sum != null ) {
                avg = sum.Value / rc;
            }

            tslSum.Text =
                string.Concat(
                "Součet= ", sum,
                "  MaxNeg= ", maxNeg,
                "  MinNeg= ", minNeg,
                "  MinPos= ", minPos,
                "  MaxPos= ", maxPos,
                "  Avg= ", avg.ToString( "0.000" )
                );
        }

        public void InitColumns( DataGridView source ) {
            dgv.Columns.Clear();
            foreach ( DataGridViewColumn c in source.Columns ) {
                dgv.Columns.Add( c.Name, c.HeaderText );
            }
        }

        public void AddRow( object[] items ) {
            dgv.Rows.Add( items );
            tslCount.Text = string.Concat( "Počet : ", dgv.Rows.Count - 1 );
        }


        internal void SetExtractingValue( string value ) {
            this.Text = value;
            dgv.ValueHistory = value;
        }
    }


}
