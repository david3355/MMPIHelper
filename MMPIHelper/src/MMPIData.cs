using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMPIHelper
{
    public class MMPIData
    {
        public MMPIData(int Index, String Number)
        {
            index = Index;
            number = Number;
        }

        private int index;
        private String number;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }        

        public String Number
        {
            get { return number; }
            set { number = value; }
        }


        public override string ToString()
        {
            return String.Format("{0}:{1}", index, number);
        }

    }
}
