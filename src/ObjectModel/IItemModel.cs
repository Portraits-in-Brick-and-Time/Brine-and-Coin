using System.Collections.Generic;
using ObjectModel.Referencing;

namespace ObjectModel;

internal interface IHasItems
{
   List<ModelRef> Items { get; set; }
}
