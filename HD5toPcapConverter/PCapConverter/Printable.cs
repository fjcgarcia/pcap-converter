using System;

/**
 * A simple interface to be implemented by all objects that want to provide
 * a pretty indented description of themselves.
 *
 * @author  Francisco J. Garcia
 * @version 1.0 07/01/09
 */

namespace PCapConverter
{
    public abstract class Printable
    {
        string tab_string = "\t";

        public abstract string toStringTab(int tabs);

        public string toHexString(byte[] ptr, int size)
        {
            return toHexString(ptr, size, 16, 0);
        }

        public string toHexString(byte[] ptr, int size, int line_size, int tabs)
        {
            int cnt = 0;
            int spccnt = 0;

            string str = "";

            if (size == 0)
            {
                str += "\r\n";
                return str;
            }

            string tabstr = "";
            for (int i = 0; i < tabs; i++)
                tabstr += " ";
            str = String.Copy(tabstr);

            for (int i = 0; i < size; i++)
            {
                str += String.Format("{0:X2} ", ptr[i]);
                cnt++;
                spccnt++;
                if (spccnt == 8)
                {
                    str += tab_string;
                    spccnt = 0;
                }
                if (cnt == line_size)
                {
                    str += "\r\n";
                    str += tabstr;
                    cnt = 0;
                    spccnt = 0;
                }
            }
            return str;
        }
    }
}
