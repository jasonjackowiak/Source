using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modularise
{
    class UnitMap
    {

        private string _unit;
        private List<string> _subUnits = new List<string>();

        public UnitMap(string Unit)
        {
            _unit = Unit;
        }

        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }

        public List<string> SubUnits
        {
            get
            {
                return _subUnits;
            }
            set
            {
                _subUnits = value;
            }
        }

    }
}
