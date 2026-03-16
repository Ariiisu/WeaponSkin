using Sharp.Shared.Enums;

namespace WeaponSkin.Shared;

public record Sticker
{
    public int StickerId { get; init; }
    public int Schema    { get; init; }

    public float Wear     { get; init; }
    public float OffsetX  { get; init; }
    public float OffsetY  { get; init; }
    public float Scale    { get; init; }
    public float Rotation { get; init; }
}

public record Keychain
{
    public int   KeychainId { get; init; }
    public float X          { get; init; }
    public float Y          { get; init; }
    public float Z          { get; init; }
    public float Seed       { get; init; }
}

public record WeaponCosmetics
{
    public EconItemId ItemId { get; init; }

    public ushort PaintId { get; init; }

    public float Wear { get; init; }
    public float Seed { get; init; }

    public int? StatTrak { get; set; }

    public string NameTag { get; init; } = string.Empty;

    public Sticker?[] Stickers { get; init; } = new Sticker?[5];

    public Keychain? Keychain { get; init; } = null;
}
