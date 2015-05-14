using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Jas.Utils.AnalyticalGrid.Helpers {
    
    internal static class Utils {

        public static string RemoveAccent( string s ) {
            s = s.Normalize( NormalizationForm.FormD );
            StringBuilder sb = new StringBuilder();

            for ( int i = 0; i < s.Length; i++ ) {
                if ( CharUnicodeInfo.GetUnicodeCategory( s[i] ) != UnicodeCategory.NonSpacingMark ) {
                    sb.Append( s[i] );
                }
            }

            return sb.ToString();
        }

    }
}
