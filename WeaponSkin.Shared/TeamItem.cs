using Sharp.Shared.Enums;

namespace WeaponSkin.Shared;

public readonly record struct TeamItem
{
    public CStrikeTeam Team   { get; init; }
    public EconItemId  ItemId { get; init; }
}
