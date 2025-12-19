using Content.Shared.Tools.Systems;
//Space Prototype changes
using Content.Server.Destructible;
using Content.Shared.Damage.Components;
using Content.Shared.Examine;
using Robust.Shared.Utility;
using Content.Shared.Tools.Components;
using Content.Server.PowerCell;
using Content.Shared.Power.Components;

namespace Content.Server.Tools;

public sealed class ToolSystem : SharedToolSystem
{
    //Space Prototype changes start
    [Dependency] private readonly DestructibleSystem _destructible = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;

    public override void ToolDamageExamine(EntityUid uid, DamageableComponent damageable, ref ExaminedEvent args)
    {
        base.ToolDamageExamine(uid, damageable, ref args);

        if (!TryComp<DestructibleComponent>(uid, out var _))
            return;

        var damage = damageable.TotalDamage;
        var damageThreshold = _destructible.DestroyedAt(uid);

        if (damageThreshold == 0)
            return;
        var damagePercent = 100f - (float)(damage / damageThreshold) * 100f;

        var msg = new FormattedMessage();
        msg.AddMarkupPermissive(Loc.GetString("tool-component-damage-percent", ("percent", damagePercent)));
        args.PushMessage(msg);
    }

    public override bool CanStartEnergyTool(EntityUid uid, ToolComponent tool, EntityUid user)
    {
        base.CanStartEnergyTool(uid, tool, user);

        if (!_powerCell.HasCharge(uid, tool.ChargeUse, user: user))
            return false;

        return true;
    }

    public override void DoAfterEnergyTool(EntityUid uid, ToolComponent tool)
    {
        base.DoAfterEnergyTool(uid, tool);

        _powerCell.TryUseCharge(uid, tool.ChargeUse);
    }
    //Space Prototype changes end
}
