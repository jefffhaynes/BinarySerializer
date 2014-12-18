using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphGen
{
    public interface IBindingSource
    {
        Binding Binding { get; set; }
    }
}
