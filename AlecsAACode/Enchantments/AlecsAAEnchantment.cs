using AlecsAA.AlecsAACode.Extensions;
using BaseLib.Abstracts;
using BaseLib.Extensions;

namespace AlecsAA.AlecsAACode.Relics;

/// <summary>
/// This is the base class for your mod's enchantments, which is set up to load the enchantment's images from your mod's resources.
/// </summary>
public abstract class AlecsAAEnchantment : CustomEnchantmentModel
{
    //AlecsAA/images/enchantments
    protected override string? CustomIconPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();
}