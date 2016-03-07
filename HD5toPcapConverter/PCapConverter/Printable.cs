/*
Copyright (c) 2016, Keysight Technologies

All rights reserved.
Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice,
this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


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
