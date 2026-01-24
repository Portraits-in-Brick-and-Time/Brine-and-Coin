using System.Collections.Generic;

namespace ObjectModel;

internal interface IItemModel
{
   List<ModelRef> Items { get; set; }
}
