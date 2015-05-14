using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Jas.Utils.AnalyticalGrid {

    public class NoEnterDataGrid : DataGridView {

        public NoEnterDataGrid() {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseDown( MouseEventArgs e ) {
            base.OnMouseDown( e );

            DataGridView.HitTestInfo Hti;

            if ( e.Button == MouseButtons.Right ) {

                Hti = this.HitTest( e.X, e.Y );

                if ( Hti.Type == DataGridViewHitTestType.Cell && Hti.RowIndex >= 0 ) {
                    this.CurrentCell = this.Rows[Hti.RowIndex].Cells[Hti.ColumnIndex];
                }
                if ( ContextMenuStrip != null ) {
                    ContextMenuStrip.Show( this, new Point( e.X, e.Y ) );
                }
            }
        }

        protected override bool ProcessDialogKey( Keys keyData ) {
            if ( keyData != Keys.Return ) {
                try {
                    return base.ProcessDialogKey( keyData );
                }
                catch ( ArgumentOutOfRangeException ) {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        protected override bool ProcessDataGridViewKey( KeyEventArgs e ) {
            if ( e.KeyCode != Keys.Return ) {
                try {
                    return base.ProcessDataGridViewKey( e );
                }
                catch ( ArgumentOutOfRangeException ) {
                    return false;
                }
            }
            else {
                return false;
            }
        }
    }
}
