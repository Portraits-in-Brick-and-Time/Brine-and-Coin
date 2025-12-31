using MessagePack;

namespace ObjectModel;

#nullable disable

[MessagePackObject(AllowPrivate = true)]
class AttributeModel
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public string Description { get; set; }

    [Key(2)]
    public int Min { get; set; }

    [Key(3)]
    public int Max { get; set; }

    [Key(4)]
    public bool Visible { get; set; }

    public static AttributeModel FromAttribute(NetAF.Assets.Attributes.Attribute attribute)
    {
        return new AttributeModel
        {
            Name = attribute.Name,
            Description = attribute.Description,
            Min = attribute.Minimum,
            Max = attribute.Maximum,
            Visible = attribute.IsPlayerVisible
        };
    }

    public NetAF.Assets.Attributes.Attribute ToAttribute()
    {
        return new NetAF.Assets.Attributes.Attribute(Name, Description, Min, Max, Visible);
    }
}