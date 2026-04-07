using System.Collections.Generic;
using ObjectModel.Referencing;

namespace ObjectModel;

// todo: rename to IHasItems
internal interface IItemModel
{
   List<ModelRef> Items { get; set; }
}
