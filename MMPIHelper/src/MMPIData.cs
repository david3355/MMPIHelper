using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMPIHelper
{
    public class MMPIData
    {
        public MMPIData(int Index, int Number)
        {
            index = Index;
            number = Number;
        }

        private int index;
        private int number;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }        

        public int Number
        {
            get { return number; }
            set { number = value; }
        }


    }
}
