using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Jas.Utils.AnalyticalGrid.Helpers {

    internal static class RowExtractor {

        public static void ExtractSameRows( DataGridView dgv, string valueHistory ) {
            string value = Helpers.Utils.RemoveAccent(dgv.CurrentCell.Value.ToString().Trim());
            int column = dgv.CurrentCell.ColumnIndex;

            Forms.SubGridForm fr = new Forms.SubGridForm();
            fr.InitColumns( dgv );

            foreach ( DataGridViewRow row in dgv.Rows ) {
                object o = row.Cells[column].Value;
                string s = ( o == null ) ? "NULL" : o.ToString();

                s = Helpers.Utils.RemoveAccent( s.Trim() );

                if ( s.Equals( value ) ) {
                    fr.AddRow( makeArray( row.Cells ) );
                }
            }

            if ( !string.IsNullOrEmpty( valueHistory ) ) {
                valueHistory = string.Concat( valueHistory, " .. " );
            }

            value = string.Concat( dgv.Columns[column].HeaderText, "=", value );

            fr.SetExtractingValue( string.Concat( valueHistory, value ) );
            fr.Show();
        }

        public static void ExtractSameRowsInner( DataGridView dgv, string valueHistory ) {
            string value = Helpers.Utils.RemoveAccent( dgv.CurrentCell.Value.ToString().Trim() );
            int column = dgv.CurrentCell.ColumnIndex;

            List<object[]> data = new List<object[]>();

            foreach ( DataGridViewRow row in dgv.Rows ) {
                object o = row.Cells[column].Value;
                string s = ( o == null ) ? "NULL" : o.ToString();

                s = Helpers.Utils.RemoveAccent( s.Trim() );

                if ( s.Equals( value ) ) {
                    data.Add( makeArray( row.Cells ) );
                }
            }

            dgv.Rows.Clear();
            foreach ( object[] d in data ) {
                dgv.Rows.Add( d );
            }

            if ( !string.IsNullOrEmpty( valueHistory ) ) {
                valueHistory = string.Concat( valueHistory, " .. " );
            }

            value = string.Concat( dgv.Columns[column].HeaderText, "=", value );
        }

        private static object[] makeArray( DataGridViewCellCollection items ) {
            List<object> r = new List<object>();
            foreach ( DataGridViewCell c in items ) {
                r.Add( c.Value );
            }
            return r.ToArray();
        }

    }
}
