using System;
using System.Collections.Generic;
using System.Text;

namespace XmlPro.Helpers
{
    public static class Chars
    {

//
//        public const char[] EntityReferences = { "&lt;", "&gt;", "&amp;",
//            "&apos;", "&quot;" };

        public const char Space = ' ';
        public const char Tab = '\t';
        public const char NewLine = '\n';
        public const char Return = '\r';


        public const char LessThan = '<';
        public const char GreaterThan = '>';
        public const char Slash = '/';
        public const char Exclamation = '!';
        public const char QuestionMark = '?';

        public const char TagStart = LessThan;
        public const char TagEnd = GreaterThan;

        public static string RemarkTagIdentity = "<!-->";
        public static string DocTypeTagIdentity = "<!DOCTYPE>";

        public static char[] TagNameEndings = new char[] { ' ', '>' };
        public static char[] IllegalCharsWithinTag = new char[] { '<', '>' };


    }
}
