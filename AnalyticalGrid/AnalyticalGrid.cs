using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Jas.Utils.AnalyticalGrid.Helpers;
using Jas.Utils.CSVTools;

namespace Jas.Utils.AnalyticalGrid {

    public class AnalyticalGrid : NoEnterDataGrid {

        private Dictionary<string, int> distinctResult;
        private int distinctTreshold = 1;

        delegate void doText( string s );
        public delegate void OnSumAction( string text, decimal? sum, decimal? minNeg, decimal? maxNeg, decimal? minPos, decimal? maxPos );

        public event OnSumAction SumAction = null;

        public string ValueHistory { get; set; }

        private string searchText = null;

        private int lastSearchPos;
        private bool columnSearch = false;

        public bool returnDisabled = false;

        public AnalyticalGrid() {
            this.CellDoubleClick += new DataGridViewCellEventHandler( AnalyticalGrid_CellDoubleClick );
            this.KeyDown += new KeyEventHandler( AnalyticalGrid_KeyDown );
            this.CellClick += new DataGridViewCellEventHandler( AnalyticalGrid_CellClick );
        }

        protected override bool DoubleBuffered {
            get {
                return true;
            }
            set {
                base.DoubleBuffered = true;
            }
        }

        void AnalyticalGrid_CellClick( object sender, DataGridViewCellEventArgs e ) {
            sum( e.ColumnIndex );
        }

        void AnalyticalGrid_KeyDown( object sender, KeyEventArgs e ) {
            if ( e.Control && e.KeyCode == Keys.Return ) {
                openRowDetail();
                return;
            }

            if ( e.Control && e.KeyCode == Keys.S ) {
                saveGrid();
                return;
            }

            if ( e.Control && e.Shift && e.KeyCode == Keys.F ) {
                columnSearch = true;
                searchText = null;
                lastSearchPos = getCurrentCursorPos();
                doSearch();
                return;
            }

            if ( e.Control && e.KeyCode == Keys.F ) {
                columnSearch = false;
                searchText = null;
                lastSearchPos = getCurrentCursorPos();
                doSearch();
                return;
            }

            if ( e.Control && e.KeyCode == Keys.D ) {
                doDistinct( this.CurrentCell.ColumnIndex );
                return;
            }

            if ( e.KeyCode == Keys.F3 ) {
                if ( lastSearchPos == getCurrentCursorPos() ) {
                    lastSearchPos = moveSearchPos( getCurrentCursorPos() );
                }
                else {
                    lastSearchPos = getCurrentCursorPos();
                }
                doSearch();
                return;
            }

            if ( e.KeyCode == Keys.Return) {
                extractRows();
                return;
            }

            if ( e.KeyCode == Keys.F2 ) {
                jumpToColumn();
                return;
            }

            if ( e.Control && e.KeyCode == Keys.Delete ) {
                deleteRow();
                return;
            }

            if ( e.KeyCode == Keys.Insert ) {
                extractRowsInner();
                return;
            }
        }

        private void doDistinct( int column) {
            distinctResult = new Dictionary<string, int>();

            foreach ( DataGridViewRow row in this.Rows ) {
                if ( row.Cells[column].Value != null ) {
                    string s = row.Cells[column].Value.ToString();

                    if ( distinctResult.ContainsKey( s ) ) {
                        distinctResult[s]++;
                    }
                    else {
                        distinctResult.Add( s, 1 );
                    }
                }
            }

            ListBox lb = new ListBox();
            lb.Parent = this;
            lb.Left = 0;
            lb.Top = 0;
            lb.Width = 300;
            lb.Height = this.Height;
            lb.Sorted = true;
            lb.KeyDown += new KeyEventHandler( lb_KeyDown );

            fillDistinctResult( lb );

            lb.Select();
        }

        private void fillDistinctResult( ListBox lb ) {
            lb.Items.Clear();
            foreach ( var kvp in distinctResult ) {
                if ( kvp.Value >= distinctTreshold ) {
                    lb.Items.Add( string.Concat( kvp.Key, "   x", kvp.Value ) );
                }
            }
        }

        void lb_KeyDown( object sender, KeyEventArgs e ) {
            if ( e.KeyCode == Keys.Escape ) {
                ListBox lb = (ListBox)sender;
                lb.Dispose();
            }

            if ( e.KeyCode == Keys.Add ) {
                distinctTreshold++;
                fillDistinctResult( sender as ListBox );
            }

            if ( e.KeyCode == Keys.Subtract ) {
                if ( distinctTreshold > 1 ) {
                    distinctTreshold--;
                    fillDistinctResult( sender as ListBox );
                }
            }
        }

        private void deleteRow() {
            try {
                this.Rows.RemoveAt( this.CurrentCell.RowIndex );
            } catch { }
        }

        private void jumpToColumn() {
            List<string> list = new List<string>();

            foreach ( DataGridViewColumn col in Columns ) {
                list.Add( col.Name );
            }

            Forms.ColumnQuickSelectForm fr = new Forms.ColumnQuickSelectForm( list.ToArray() );
            if ( fr.ShowDialog() == DialogResult.OK) {
                focusColumn( fr.ColumnSelected );
            }
        }

        private void focusColumn( string p ) {
            foreach ( DataGridViewColumn col in Columns ) {
                if ( col.Name.Equals( p ) ) {
                    this.CurrentCell = this.CurrentRow.Cells[col.Index];
                    break;
                }
            }
        }

        private int moveSearchPos( int p ) {
            if ( columnSearch ) {
                return p + this.Columns.Count;
            }
            else {
                return p + 1;
            }
        }

        private int getCurrentCursorPos() {
            return ( this.CurrentCell.RowIndex * this.Columns.Count ) + this.CurrentCell.ColumnIndex;
        }

        private void doSearch() {
            if ( searchText == null ) {
                Forms.SearchTextForm fr = new Forms.SearchTextForm();
                fr.Text = columnSearch ? "Hledat v aktuálním sloupci..." : "Hledat ve všech sloupcích...";
                if ( fr.ShowDialog() == DialogResult.OK ) {
                    searchText = string.IsNullOrEmpty( fr.SearchText ) ? null : fr.SearchText;
                }
            }

            if ( columnSearch ) {
                searchNextColumn();
            }
            else {
                searchNext();
            }
        }

        private void searchNextColumn() {
            if ( searchText == null ) {
                return;
            }

            searchText = Helpers.Utils.RemoveAccent( searchText );

            int lastRow = lastSearchPos / this.Columns.Count;
            int col = lastSearchPos % this.Columns.Count;

            for ( int row = lastRow; row < this.Rows.Count; row++ ) {
                DataGridViewRow rw = this.Rows[row];

                object o = rw.Cells[col].Value;

                if ( o != null ) {
                    string s = Helpers.Utils.RemoveAccent( o.ToString().ToLower() );

                    if ( s.Contains( searchText ) ) {
                        this.CurrentCell = this.Rows[row].Cells[col];
                        break;
                    }
                }

            }

            lastSearchPos = getCurrentCursorPos();
        }

        private void searchNext() {
            if ( searchText == null ) {
                lastSearchPos = 0;
                return;
            }

            searchText = Helpers.Utils.RemoveAccent( searchText );

            int lastRow = lastSearchPos / this.Columns.Count;
            int col = lastSearchPos % this.Columns.Count;
            bool found = false;

            for ( int row = lastRow; row < this.Rows.Count; row++ ) {
                DataGridViewRow rw = this.Rows[row];

                while ( col < this.Columns.Count ) {
                    object o = rw.Cells[col].Value;

                    if ( o != null ) {
                        string s = Helpers.Utils.RemoveAccent( o.ToString().ToLower() );

                        if ( s.Contains( searchText ) ) {
                            found = true;
                            this.CurrentCell = this.Rows[row].Cells[col];
                            break;
                        }
                    }

                    lastSearchPos++;
                    col++;
                }

                col = 0;

                if ( found ) {
                    break;
                }

            }

            lastSearchPos = getCurrentCursorPos();
        }

        private void saveGrid() {
            SaveFileDialog sd1 = new SaveFileDialog();
            sd1.RestoreDirectory = true;
            sd1.Filter = "Soubory CSV | *.csv";
            if ( sd1.ShowDialog() == DialogResult.OK ) {
                CSVExport ex = new CSVExport();
                ex.ExportDefault( sd1.Filter, this );
            }
            MessageBox.Show( "Export dokončen" );
        }

        private void openRowDetail() {
            Forms.RowDetailForm.ShowDetail( this, this.CurrentCell.RowIndex );
        }

        void AnalyticalGrid_CellDoubleClick( object sender, DataGridViewCellEventArgs e ) {
            extractRows();
        }

        private void extractRows() {
            if ( !returnDisabled ) {
                RowExtractor.ExtractSameRows( this, ValueHistory );
            }
        }

        private void extractRowsInner() {
            RowExtractor.ExtractSameRowsInner( this, ValueHistory );
        }

        private void notifySumAction( string text, decimal? sum, decimal? minNeg, decimal? maxNeg, decimal? minPos, decimal? maxPos ) {
            if ( SumAction != null ) {
                SumAction( text, sum, minNeg, maxNeg, minPos, maxPos );
            }
        }

        private void sum( int column ) {
            if ( column < 0 ) {
                return;
            }

            notifySumAction( "...", 0, 0, 0, 0, 0 );

            List<object> r = new List<object>( this.Rows.Count + 1 );
            foreach ( DataGridViewRow row in this.Rows ) {
                r.Add( row.Cells[column].Value );
            }

            new Thread( sumStart ).Start( r );
        }

        private void sumStart( object list ) {
            List<object> lst = (List<object>)list;
            decimal sum = 0m;
            decimal? minNeg, maxNeg, minPos, maxPos;

            minNeg = maxNeg = minPos = maxPos = null;

            foreach ( object o in lst ) {
                decimal? val = null;

                if ( o is decimal ) {
                    val = (decimal)o;
                }
                else if ( o is int ) {
                    val = (int)o;
                }
                else if ( o is float ) {
                    val = (decimal)(float)o;
                }
                else if ( o is double ) {
                    val = (decimal)(double)o;
                }
                else if ( o is string ) {
                    decimal d = 0;
                    if ( decimal.TryParse( (string)o, out d ) ) {
                        val = d;
                    }
                }

                if ( val == null ) {
                    continue;
                }

                sum += val.Value;

                if ( val < 0 ) {

                    if ( minNeg == null ) {
                        minNeg = val;
                    }

                    if ( maxNeg == null ) {
                        maxNeg = val;
                    }

                    if ( val > minNeg ) {
                        minNeg = val;
                    }
                    else if ( val < maxNeg ) {
                        maxNeg = val;
                    }
                }
                else {
                    if ( minPos == null ) {
                        minPos = val;
                    }

                    if ( maxPos == null ) {
                        maxPos = val;
                    }

                    if ( val < minPos ) {
                        minPos = val;
                    }
                    else if ( val > maxPos ) {
                        maxPos = val;
                    }
                }
            }

            OnSumAction prsc = notifySumAction;
            Invoke( prsc, sum.ToString(), sum, minNeg, maxNeg, minPos, maxPos );
        }

        public void SetReturnDisabled( bool disabled ) {
            returnDisabled = disabled;
        }
    }
}
