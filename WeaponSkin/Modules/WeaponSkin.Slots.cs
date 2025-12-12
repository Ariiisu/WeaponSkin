namespace WeaponSkin.Modules;

internal partial class WeaponSkin
{
    private readonly ref struct StickerSchema(
        ReadOnlySpan<byte> id,
        ReadOnlySpan<byte> wear,
        ReadOnlySpan<byte> scale,
        ReadOnlySpan<byte> rot,
        ReadOnlySpan<byte> x,
        ReadOnlySpan<byte> y)
    {
        public readonly ReadOnlySpan<byte> Id       = id;
        public readonly ReadOnlySpan<byte> Wear     = wear;
        public readonly ReadOnlySpan<byte> Scale    = scale;
        public readonly ReadOnlySpan<byte> Rotation = rot;
        public readonly ReadOnlySpan<byte> OffsetX  = x;
        public readonly ReadOnlySpan<byte> OffsetY  = y;
    }

    private static StickerSchema GetStickerSchema(int slot)
    {
        return slot switch
        {
            0 => new("sticker slot 0 id"u8, "sticker slot 0 wear"u8, "sticker slot 0 scale"u8, "sticker slot 0 rotation"u8, "sticker slot 0 offset x"u8, "sticker slot 0 offset y"u8),
            1 => new("sticker slot 1 id"u8, "sticker slot 1 wear"u8, "sticker slot 1 scale"u8, "sticker slot 1 rotation"u8, "sticker slot 1 offset x"u8, "sticker slot 1 offset y"u8),
            2 => new("sticker slot 2 id"u8, "sticker slot 2 wear"u8, "sticker slot 2 scale"u8, "sticker slot 2 rotation"u8, "sticker slot 2 offset x"u8, "sticker slot 2 offset y"u8),
            3 => new("sticker slot 3 id"u8, "sticker slot 3 wear"u8, "sticker slot 3 scale"u8, "sticker slot 3 rotation"u8, "sticker slot 3 offset x"u8, "sticker slot 3 offset y"u8),
            4 => new("sticker slot 4 id"u8, "sticker slot 4 wear"u8, "sticker slot 4 scale"u8, "sticker slot 4 rotation"u8, "sticker slot 4 offset x"u8, "sticker slot 4 offset y"u8),
            5 => new("sticker slot 5 id"u8, "sticker slot 5 wear"u8, "sticker slot 5 scale"u8, "sticker slot 5 rotation"u8, "sticker slot 5 offset x"u8, "sticker slot 5 offset y"u8),
            _ => default
        };
    }

    private readonly ref struct KeychainSchema(
        ReadOnlySpan<byte> id,
        ReadOnlySpan<byte> x,
        ReadOnlySpan<byte> y,
        ReadOnlySpan<byte> z,
        ReadOnlySpan<byte> seed,
        ReadOnlySpan<byte> highlight,
        ReadOnlySpan<byte> sticker)
    {
        public readonly ReadOnlySpan<byte> Id        = id;
        public readonly ReadOnlySpan<byte> OffsetX   = x;
        public readonly ReadOnlySpan<byte> OffsetY   = y;
        public readonly ReadOnlySpan<byte> OffsetZ   = z;
        public readonly ReadOnlySpan<byte> Seed      = seed;
        public readonly ReadOnlySpan<byte> Highlight = highlight;
        public readonly ReadOnlySpan<byte> Sticker   = sticker;
    }

    private static KeychainSchema GetKeychainSchema(int slot)
    {
        return slot switch
        {
            0 => new("keychain slot 0 id"u8, "keychain slot 0 offset x"u8, "keychain slot 0 offset y"u8, "keychain slot 0 offset z"u8, "keychain slot 0 seed"u8, "keychain slot 0 highlight"u8, "keychain slot 0 sticker"u8),
            1 => new("keychain slot 1 id"u8, "keychain slot 1 offset x"u8, "keychain slot 1 offset y"u8, "keychain slot 1 offset z"u8, "keychain slot 1 seed"u8, "keychain slot 1 highlight"u8, "keychain slot 1 sticker"u8),
            2 => new("keychain slot 2 id"u8, "keychain slot 2 offset x"u8, "keychain slot 2 offset y"u8, "keychain slot 2 offset z"u8, "keychain slot 2 seed"u8, "keychain slot 2 highlight"u8, "keychain slot 2 sticker"u8),
            3 => new("keychain slot 3 id"u8, "keychain slot 3 offset x"u8, "keychain slot 3 offset y"u8, "keychain slot 3 offset z"u8, "keychain slot 3 seed"u8, "keychain slot 3 highlight"u8, "keychain slot 3 sticker"u8),
            4 => new("keychain slot 4 id"u8, "keychain slot 4 offset x"u8, "keychain slot 4 offset y"u8, "keychain slot 4 offset z"u8, "keychain slot 4 seed"u8, "keychain slot 4 highlight"u8, "keychain slot 4 sticker"u8),
            5 => new("keychain slot 5 id"u8, "keychain slot 5 offset x"u8, "keychain slot 5 offset y"u8, "keychain slot 5 offset z"u8, "keychain slot 5 seed"u8, "keychain slot 5 highlight"u8, "keychain slot 5 sticker"u8),
            _ => default
        };
    }
}