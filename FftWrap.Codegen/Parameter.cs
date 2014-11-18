using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FftWrap.Codegen
{
    public class Parameter
    {
        private readonly string _type;
        private readonly string _name;
        private readonly bool _isPointer;
        public Parameter(string type, string name, bool isPointer)
        {
            _type = type;
            _name = name;
            _isPointer = isPointer;
        }

        public string Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsPointer
        {
            get { return _isPointer; }
        }
    }
}
