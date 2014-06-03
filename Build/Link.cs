using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Build
{
    public class Link
    {
        private string _callingEnt;
        private string _calledEnt;

        public Link(string callingEnt, string calledEnt)
        {
            _callingEnt = callingEnt;
            _calledEnt = calledEnt;
        }
        public string CallingEnt
        {
            get
            {
                return _callingEnt;
            }
            set
            {
                _callingEnt = value;
            }
        }

        public string CalledEnt
        {
            get
            {
                return _calledEnt;
            }
            set
            {
                _calledEnt = value;
            }
        }

    }
}
