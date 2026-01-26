using System.Collections.Generic;
using ObjectModel.Referencing;

namespace ObjectModel;

internal interface IItemModel
{
   List<ModelRef> Items { get; set; }
}
