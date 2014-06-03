using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using Import;

namespace Modularise
{
    public class EntityMap
    {
        private string _name;
        private string _type;
        private string _sourceUnit;
        private string _normalisedUnit;

        public EntityMap(string Name, string Type,string SourceUnit, string NormalisedUnit)
        {
            _name = Name;
            _type = Type;
            _sourceUnit = SourceUnit;
            _normalisedUnit = NormalisedUnit;
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public string SourceUnit
        {
            get
            {
                return _sourceUnit;
            }
            set
            {
                _sourceUnit = value;
            }
        }

        public string NormalisedUnit
        {
            get
            {
                return _normalisedUnit;
            }
            set
            {
                _normalisedUnit = value;
            }
        }
    }
}
