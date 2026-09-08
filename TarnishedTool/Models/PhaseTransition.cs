using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TarnishedTool.GameIds;
using TarnishedTool.Interfaces;
using TarnishedTool.Services;

namespace TarnishedTool.Models;

public abstract class PhaseTransition
{
    public abstract string Label { get; }
    public abstract bool CanActivate(ITargetService targetService);
    public virtual bool IsPhase2(ISpEffectService spEffectService, ITargetService targetService) => false;
    public abstract void Execute(ITargetService targetService, IEmevdService emevdService);


    #region helpers

    public abstract class SimplePhaseTransition : PhaseTransition
    {
        protected abstract float Threshold { get; }
        protected abstract uint Phase2SpEffect { get; }
        protected virtual bool FlatHp => false;
        protected virtual uint[] ExtraPhase2SpEffects => [];

        public override bool IsPhase2(ISpEffectService spEffectService, ITargetService targetService)
        {
            var chrIns = targetService.GetTargetChrIns();
            if (chrIns == 0) return false;
            if (spEffectService.HasSpEffect(chrIns, Phase2SpEffect)) return true;
            return ExtraPhase2SpEffects.Any(se => spEffectService.HasSpEffect(chrIns, se));
        }

        public override bool CanActivate(ITargetService targetService)
            => FlatHp
                ? targetService.GetCurrentHp() > Threshold
                : targetService.GetCurrentHp() / (float)targetService.GetMaxHp() > Threshold;


        public override void Execute(ITargetService targetService, IEmevdService emevdService)
            => targetService.SetHp(FlatHp
                ? (int)Threshold
                : (int)(targetService.GetMaxHp() * Threshold));
    }

    public abstract class SimplePhaseTransitionNoSpEffect : PhaseTransition
    {
        protected abstract float Threshold { get; }

        public override bool IsPhase2(ISpEffectService spEffectService, ITargetService targetService) => false;

        public override bool CanActivate(ITargetService targetService)
            => targetService.GetCurrentHp() / (float)targetService.GetMaxHp() > Threshold;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
            => targetService.SetHp((int)(targetService.GetMaxHp() * Threshold));
    }

    public abstract class TwoEntitiesPhaseTransition : PhaseTransition
    {
        protected abstract uint Entity1Id { get; }
        protected abstract uint Entity2Id { get; }
        protected abstract float Threshold { get; }
        protected virtual bool FlatHp => false;
        protected abstract uint[] Phase2SpEffects { get; }

        private readonly IChrInsService _chrInsService;

        protected TwoEntitiesPhaseTransition(IChrInsService chrInsService)
        {
            _chrInsService = chrInsService;
        }

        protected nint GetChrIns(uint entityId) => _chrInsService.ChrInsByEntityId(entityId);
        protected int GetMaxHp(nint chrIns) => _chrInsService.GetMaxHp(chrIns);
        protected void SetHp(nint chrIns, int hp) => _chrInsService.SetHp(chrIns, hp);

        protected int GetCurrentAnimation(nint chrIns) => _chrInsService.GetCurrentAnimation(chrIns);

        public override bool IsPhase2(ISpEffectService spEffectService, ITargetService targetService)
        {
            var chrIns = targetService.GetTargetChrIns();
            if (chrIns == 0) return false;
            foreach (var spEffect in Phase2SpEffects)
                if (spEffectService.HasSpEffect(chrIns, spEffect))
                    return true;
            return false;
        }

        public override bool CanActivate(ITargetService targetService)
            => FlatHp
                ? targetService.GetCurrentHp() > Threshold
                : targetService.GetCurrentHp() / (float)targetService.GetMaxHp() > Threshold;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var chrIns1 = _chrInsService.ChrInsByEntityId(Entity1Id);
            var chrIns2 = _chrInsService.ChrInsByEntityId(Entity2Id);

            int hp1 = FlatHp ? (int)Threshold : (int)(_chrInsService.GetMaxHp(chrIns1) * Threshold);
            int hp2 = FlatHp ? (int)Threshold : (int)(_chrInsService.GetMaxHp(chrIns2) * Threshold);

            _chrInsService.SetHp(chrIns1, hp1);
            _chrInsService.SetHp(chrIns2, hp2);
        }
    }

    private static void ForceActAndWait(ITargetService targetService, int act)
    {
        targetService.ForceAct(act);
        while (targetService.GetLastAct() != act)
            Thread.Sleep(50);
        targetService.ForceAct(0);
    }

    private static void SetAttackCooldown(IChrInsService chrInsService, IAiService aiService, uint entityId,
        uint animationId)
    {
        var chrIns = chrInsService.ChrInsByEntityId(entityId);
        var aiThink = aiService.GetAiThinkPtr(chrIns);
        if (aiThink != 0)
        {
            aiService.RequestAttackCooldown(aiThink, animationId);
        }
    }

    private static void PlayAnimation(IEmevdService emevdService, uint entityId, uint animationId)
    {
        emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.ForceAnimationPlayback(entityId, (int)animationId));
    }

    private static void PlayAnimationAndSetCooldown(IEmevdService emevdService, IChrInsService chrInsService,
        IAiService aiService, uint entityId, uint animationId)
    {
        SetAttackCooldown(chrInsService, aiService, entityId, animationId);
        PlayAnimation(emevdService, entityId, animationId);
    }

    #endregion


    #region main bosses (Base Game)

    // Margit, the Fell Omen
    public class MargitPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.6501f;
        protected override uint Phase2SpEffect => 16200;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimation(emevdService, 10000850, 3026);
        }
    }

    // Godrick the Grafted
    public class GodrickPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.6000000238418579f;
        protected override uint Phase2SpEffect => 14750;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimation(emevdService, 10000800, 20010);
        }
    }

    // Starscourge Radahn Phase 1.5
    public class StarscourgeRadahnPhase1Point5 : SimplePhaseTransition
    {
        public override string Label => "Phase 1.5";
        protected override float Threshold => 0.80f;
        protected override uint Phase2SpEffect => 13903;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            ForceActAndWait(targetService, 10);
        }
    }

    // Starscourge Radahn Phase 2
    public class StarscourgeRadahnPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.50f;
        protected override uint Phase2SpEffect => 13904;

        private readonly IChrInsService _chrInsService;
        private readonly IAiService _aiService;

        public StarscourgeRadahnPhase2(IChrInsService chrInsService, IAiService aiService)
        {
            _aiService = aiService;
            _chrInsService = chrInsService;
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(1052380800, 13902));
            PlayAnimationAndSetCooldown(emevdService, _chrInsService, _aiService, 1052380800, 3035);
        }
    }

    // Godskin Noble (Manor)
    public class NoblePhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.599f;
        protected override uint Phase2SpEffect => 15501;
        
        
        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(16000850, 15501));
            ForceActAndWait(targetService, 15);
        }
    }

    // Rykard (God-Devouring Serpent)
    public class RykardPhase2 : SimplePhaseTransitionNoSpEffect
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.01f;
    }

    // Rykard, Lord of Blasphemy
    public class RykardPhase2Point5 : SimplePhaseTransitionNoSpEffect
    {
        public override string Label => "Phase 2.5";
        protected override float Threshold => 0.50f;
    }

    // Rennala Queen of the Full Moon Phase 2
    public class RennalaPhase2 : SimplePhaseTransitionNoSpEffect
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0f;
    }

    // Rennala Queen of the Full Moon Phase 2.5
    public class RennalaPhase2Point5 : SimplePhaseTransition
    {
        public override string Label => "Phase 2.5";
        protected override float Threshold => 0.60f;
        protected override uint Phase2SpEffect => 13020;
    }

    // Draconic Tree Sentinel
    public class DraconicTreeSentinelPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.60f;
        protected override uint Phase2SpEffect => 13708;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.ClearSpEffect(1045520800, 13707));
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(1045520800, 13708));
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(1045520800, 13700));
            PlayAnimation(emevdService, 1045520800, 3027);
        }
    }

    // Morgott, the Omen King
    public class MorgottPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.60f;
        protected override uint Phase2SpEffect => 16208;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimation(emevdService, 11000800, 3024);
        }
    }

    // Fire Giant (Anklet thingy)
    public class FireGiantPhase1Point5 : PhaseTransition
    {
        public override string Label => "Phase 1.5";
        private const uint FireGiantP1EntityId = 1052520801;
        private const uint FireGiantP2EntityId = 1052520800;

        public override bool IsPhase2(ISpEffectService spEffectService, ITargetService targetService)
        {
            var chrIns = targetService.GetTargetChrIns();
            if (chrIns == 0) return false;
            return spEffectService.HasSpEffect(chrIns, 12705);
        }

        private readonly IChrInsService _chrInsService;

        public FireGiantPhase1Point5(IChrInsService chrInsService)
        {
            _chrInsService = chrInsService;
        }

        public override bool CanActivate(ITargetService targetService)
            => targetService.GetCurrentHp() > 0;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var fireGiantP1ChrIns = _chrInsService.ChrInsByEntityId(FireGiantP1EntityId);
            var fireGiantP2ChrIns = _chrInsService.ChrInsByEntityId(FireGiantP2EntityId);

            int fireGiantP1MaxHp = _chrInsService.GetMaxHp(fireGiantP1ChrIns);
            int fireGiantP2MaxHp = _chrInsService.GetMaxHp(fireGiantP2ChrIns);

            int fireGiantP2Hp = (fireGiantP2MaxHp - (int)(fireGiantP1MaxHp * 0.097f));

            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(FireGiantP1EntityId, 12752));
            PlayAnimation(emevdService, FireGiantP1EntityId, 20010);
            _chrInsService.SetHp(fireGiantP1ChrIns, (int)(fireGiantP1MaxHp * 0.903f));
            _chrInsService.SetHp(fireGiantP2ChrIns, fireGiantP2Hp);
        }
    }

    // Fire Giant
    public class FireGiantPhase2 : PhaseTransition
    {
        public override string Label => "Phase 2";
        private const uint FireGiantP1EntityId = 1052520801;
        private const uint FireGiantP2EntityId = 1052520800;

        private readonly IChrInsService _chrInsService;

        public FireGiantPhase2(IChrInsService chrInsService)
        {
            _chrInsService = chrInsService;
        }

        public override bool CanActivate(ITargetService targetService)
            => targetService.GetCurrentHp() > 0;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var fireGiantP1ChrIns = _chrInsService.ChrInsByEntityId(FireGiantP1EntityId);
            var fireGiantP2ChrIns = _chrInsService.ChrInsByEntityId(FireGiantP2EntityId);

            int fireGiantP1MaxHp = _chrInsService.GetMaxHp(fireGiantP1ChrIns);
            int fireGiantP2MaxHp = _chrInsService.GetMaxHp(fireGiantP2ChrIns);

            int fireGiantP2Hp = (fireGiantP2MaxHp - fireGiantP1MaxHp);

            _chrInsService.SetHp(fireGiantP1ChrIns, 0);
            _chrInsService.SetHp(fireGiantP2ChrIns, fireGiantP2Hp);
        }
    }

    // Godskin Duo (Noble)
    public class NobleGduoPhase2 : PhaseTransition
    {
        public override string Label => "Phase 2";
        private const uint NobleEntityId = 13000852;
        private const uint UndergroundApostleEntityId = 13000850;

        private readonly IChrInsService _chrInsService;

        public NobleGduoPhase2(IChrInsService chrInsService)
        {
            _chrInsService = chrInsService;

        }

        public override bool IsPhase2(ISpEffectService spEffectService, ITargetService targetService)
        {
            var chrIns = targetService.GetTargetChrIns();
            if (chrIns == 0) return false;
            return spEffectService.HasSpEffect(chrIns, 15501);
        }

        public override bool CanActivate(ITargetService targetService)
            => targetService.GetCurrentHp() > 0.60f;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var nobleChrIns = _chrInsService.ChrInsByEntityId(NobleEntityId);
            var undergroundApostleChrIns = _chrInsService.ChrInsByEntityId(UndergroundApostleEntityId);

            int nobleMaxHp = _chrInsService.GetMaxHp(nobleChrIns);
            int undergroundApostleHp = _chrInsService.GetCurrentHp(undergroundApostleChrIns);

            int undergroundCurrentApostleHp = (undergroundApostleHp - (int)(nobleMaxHp * 0.599f));

            _chrInsService.SetHp(nobleChrIns, (int)(nobleMaxHp * 0.599f));
            _chrInsService.SetHp(undergroundApostleChrIns, undergroundCurrentApostleHp);
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(NobleEntityId, 15501));
            ForceActAndWait(targetService, 15);
        }
    }

    // Godskin Duo (Apostle)
    public class ApostleGduoPhase2 : PhaseTransition
    {
        public override string Label => "Phase 2";
        private const uint ApostleEntityId = 13000851;
        private const uint UndergroundApostleEntityId = 13000850;

        private readonly IChrInsService _chrInsService;

        public ApostleGduoPhase2(IChrInsService chrInsService)
        {
            _chrInsService = chrInsService;
        }

        public override bool IsPhase2(ISpEffectService spEffectService, ITargetService targetService)
        {
            var chrIns = targetService.GetTargetChrIns();
            if (chrIns == 0) return false;
            return spEffectService.HasSpEffect(chrIns, 15451);
        }

        public override bool CanActivate(ITargetService targetService)
            => targetService.GetCurrentHp() > 0.60f;
        

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var apostleChrIns = _chrInsService.ChrInsByEntityId(ApostleEntityId);
            var undergroundApostleChrIns = _chrInsService.ChrInsByEntityId(UndergroundApostleEntityId);

            int apostleMaxHp = _chrInsService.GetMaxHp(apostleChrIns);
            int undergroundApostleHp = _chrInsService.GetCurrentHp(undergroundApostleChrIns);

            int undergroundCurrentApostleHp = (undergroundApostleHp - (int)(apostleMaxHp * 0.599f));

            _chrInsService.SetHp(apostleChrIns, (int)(apostleMaxHp * 0.599f));
            _chrInsService.SetHp(undergroundApostleChrIns, undergroundCurrentApostleHp);
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(ApostleEntityId, 15451));
            ForceActAndWait(targetService, 11);
            
        }
    }

    // Beast Clergyman → Maliketh, the Black Blade
    public class MalikethPhase2 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 2";
        protected override uint Entity1Id => 13000801; // Clergyman
        protected override uint Entity2Id => 13000800; // Maliketh
        protected override float Threshold => 0.55f;
        protected override uint[] Phase2SpEffects => [];

        public MalikethPhase2(IChrInsService chrInsService) : base(chrInsService)
        {
        }
    }

    // Godfrey First Elden Lord Phase 1.5
    public class GodfreyPhase1Point5 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 1.5";
        protected override uint Entity1Id => 11050801; // Godfrey
        protected override uint Entity2Id => 11050800; // Hoarah Loux
        protected override float Threshold => 0.70f;
        protected override uint[] Phase2SpEffects => [12290];

        public GodfreyPhase1Point5(IChrInsService chrInsService) : base(chrInsService)
        {
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var godfreyChrIns = GetChrIns(Entity1Id);
            var hoarahLouxChrIns = GetChrIns(Entity2Id);

            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(Entity1Id, 12290));
            SetHp(godfreyChrIns, (int)(GetMaxHp(godfreyChrIns) * 0.50f));
            SetHp(hoarahLouxChrIns, (int)(GetMaxHp(hoarahLouxChrIns) * 0.73f));
            PlayAnimation(emevdService, Entity1Id, 3019);
        }
    }

    // Godfrey, First Elden Lord → Hoarah Loux, Warrior
    public class GodfreyPhase2 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 2";
        protected override uint Entity1Id => 11050801; // Godfrey
        protected override uint Entity2Id => 11050800; // Hoarah Loux
        protected override float Threshold => 0f;
        protected override uint[] Phase2SpEffects => [];

        public GodfreyPhase2(IChrInsService chrInsService) : base(chrInsService)
        {
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var godfreyChrIns = GetChrIns(Entity1Id);
            var hoarahLouxChrIns = GetChrIns(Entity2Id);

            int godfreyMaxHp = GetMaxHp(godfreyChrIns);
            int hoarahLouxMaxHp = GetMaxHp(hoarahLouxChrIns);

            SetHp(godfreyChrIns, 1);
            SetHp(hoarahLouxChrIns, hoarahLouxMaxHp - godfreyMaxHp + 1);
        }
    }

    // Hoarah Loux, Warrior Phase 2.5
    public class HoarahLouxPhase2Point5 : SimplePhaseTransition
    {
        public override string Label => "Phase 2.5";
        protected override float Threshold => 0.30f;
        protected override uint Phase2SpEffect => 12290;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(11050800, 12290));
            targetService.SetHp((int)(targetService.GetMaxHp() * 0.30f));
            PlayAnimation(emevdService, 11050800, 3022);
        }
    }

    // Radagon of the Golden Order Phase 2
    public class RadagonPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.70f;
        protected override uint Phase2SpEffect => 18582;
    }

    // Radagon of the Golden Order Phase 3
    public class RadagonPhase3 : SimplePhaseTransition
    {
        public override string Label => "Phase 3";
        protected override float Threshold => 0.40f;
        protected override uint Phase2SpEffect => 18583;
    }

    // Elden Beast Unlocked Attacks (1 Ring)
    public class EldenBeastSingleRing : SimplePhaseTransition
    {
        public override string Label => "1 Ring";
        protected override float Threshold => 0.80f;
        protected override uint Phase2SpEffect => 18603;
        protected override uint[] ExtraPhase2SpEffects => [18604, 18605];
    }

    // Elden Beast Unlocked Attacks (ELDEN STAHS)
    public class EldenBeastEldenStars : SimplePhaseTransition
    {
        public override string Label => "Stars";
        protected override float Threshold => 0.50f;
        protected override uint Phase2SpEffect => 18604;
        protected override uint[] ExtraPhase2SpEffects => [18605];
    }

    // Elden Beast Unlocked Attacks (3 Rings)
    public class EldenBeastTripleRings : SimplePhaseTransition
    {
        public override string Label => "3 Rings";
        protected override float Threshold => 0.30f;
        protected override uint Phase2SpEffect => 18605;
    }

    // Commander Niall
    public class NiallPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.60f;
        protected override uint Phase2SpEffect => 11136;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimation(emevdService, 1051570800, 3021);
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(1051570800, 11136));
        }
    }

    // Loretta, Knight of the Haligtree
    public class HaligtreeLorettaPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.499f;
        protected override uint Phase2SpEffect => 13802;
    }

    // Malenia, Blade of Miquella
    public class MaleniaPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";

        protected override float Threshold => 1;
        protected override bool FlatHp => true;
        protected override uint Phase2SpEffect => 18016;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            targetService.SetHp(1);
            Thread.Sleep(17);
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.ShootBullet(10000, 15000800, 290,
                301110900, 0, 1, 0));
        }
    }

    // Mohg, Lord of Blood
    public class MohgPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.50f;
        protected override uint Phase2SpEffect => 10648;
    }

    // Royal Kight Loretta
    public class RoyalLorettaPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.499f;
        protected override uint Phase2SpEffect => 13802;
    }

    // Astel, Naturalborn of the Void
    public class AstelNaturalbornPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.599f;
        protected override uint Phase2SpEffect => 16744;
    }

    // Dragonlord Placidusax Phase 2
    public class PlacidusaxPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.649f;
        protected override uint Phase2SpEffect => 16890;
        protected override uint[] ExtraPhase2SpEffects => [16891, 16892];

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(13000830, 16890));
            base.Execute(targetService, emevdService);
            PlayAnimation(emevdService, 13000830, 3032);
        }
    }

    // Dragonlord Placidusax Phase 3
    public class PlacidusaxPhase3 : SimplePhaseTransition
    {
        public override string Label => "Phase 3";
        protected override float Threshold => 0.449f;
        protected override uint Phase2SpEffect => 16891;
        protected override uint[] ExtraPhase2SpEffects => [16892];

        private readonly IChrInsService _chrInsService;
        private readonly IAiService _aiService;

        public PlacidusaxPhase3(IChrInsService chrInsService, IAiService aiService)
        {
            _chrInsService = chrInsService;
            _aiService = aiService;
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(13000830, 16890));
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(13000830, 16891));
           SetAttackCooldown(_chrInsService, _aiService, 13000830, 3034);
           Thread.Sleep(100);
           PlayAnimation(emevdService, 13000830, 3034);
  //          emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.ForceAnimationPlayback(13000830, 3034, false, false, false, 0, 1f));
          //  PlayAnimationAndSetCooldown(emevdService, _chrInsService, _aiService, 13000830, 3034);
        }
    }

    // Dragonlord Placidusax Phase 4
    public class PlacidusaxPhase4 : SimplePhaseTransition
    {
        public override string Label => "Phase 4";
        protected override float Threshold => 0.299f;
        protected override uint Phase2SpEffect => 5;

        private readonly IChrInsService _chrInsService;
        private readonly IAiService _aiService;

        public PlacidusaxPhase4(IChrInsService chrInsService, IAiService aiService)
        {
            _chrInsService = chrInsService;
            _aiService = aiService;
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(13000830, 5400));
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(13000830, 16890));
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(13000830, 16891));
            emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.SetSpEffect(13000830, 16892));
            base.Execute(targetService, emevdService);
            SetAttackCooldown(_chrInsService, _aiService, 13000830, 3034);
            SetAttackCooldown(_chrInsService, _aiService, 13000830, 20015);
            Thread.Sleep(100);
            PlayAnimation(emevdService, 13000830, 20015);
   //        emevdService.ExecuteEmevdCommand(Emevd.EmevdCommands.ForceAnimationPlayback(13000830, 20015, false, false, false, 0, 1f));
   //         PlayAnimationAndSetCooldown(emevdService, _chrInsService, _aiService, 13000830, 20015);
        }
    }

    // Lichdragon Fortissax
    public class FortissaxPhase2 : SimplePhaseTransitionNoSpEffect
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.60f;
    }

    #endregion

    #region Main Bosses DLC

    // put shit here later ok


    // Belurat Divine Beast Dancing Lion
    public class BeluratLionPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";

        // set a stupid threshold to not have him trigger phase transition twice
        protected override float Threshold =>
            0.7001f; // it's 69.999999 - 70% normally lol, whatever bro nobody will notice surely

        protected override uint Phase2SpEffect => 20011245;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimation(emevdService, 20000800, 20002);
        }
    }

    // Rellana, Twin Moon Knight


    public class RellanaPhase1Point5 : SimplePhaseTransition
    {
        public override string Label => "Phase 1.5";
        protected override float Threshold => 0.72f;
        protected override uint Phase2SpEffect => 20012001;

        private readonly IChrInsService _chrInsService;
        private readonly IAiService _aiService;

        public RellanaPhase1Point5(IChrInsService chrInsService, IAiService aiService)
        {
            _chrInsService = chrInsService;
            _aiService = aiService;
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService); 
            PlayAnimationAndSetCooldown(emevdService,_chrInsService,_aiService,2048440800, 3030);
        }
    }

    public class RellanaPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";

        // same deal as lion xd
        protected override float Threshold => 0.5501f;
        protected override uint Phase2SpEffect => 20012001;

        private readonly IChrInsService _chrInsService;
        private readonly IAiService _aiService;

        public RellanaPhase2(IChrInsService chrInsService, IAiService aiService)
        {
            _chrInsService = chrInsService;
            _aiService = aiService;
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimationAndSetCooldown(emevdService,_chrInsService,_aiService,2048440800, 3024);
        }
    }

    // Commander Gaius
    public class GaiusPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.60f;
        protected override uint Phase2SpEffect => 10010000;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            ForceActAndWait(targetService, 10);
        }
    }

    public class ScadutreeAvatarPhase1Point5 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 1.5";
        protected override uint Entity1Id => 2050480802; // Body
        protected override uint Entity2Id => 2050480812; // Health
        protected override bool FlatHp => true;
        protected override float Threshold => 1;
        protected override uint[] Phase2SpEffects => [];

        public ScadutreeAvatarPhase1Point5(IChrInsService chrInsService) : base(chrInsService)
        {
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var scaduBodyChrIns = GetChrIns(Entity1Id);
            var scaduHealthChrIns = GetChrIns(Entity2Id);

            SetHp(scaduBodyChrIns, (int)(GetMaxHp(scaduBodyChrIns) * 0.60f));
            SetHp(scaduHealthChrIns, (int)(GetMaxHp(scaduHealthChrIns) * 0.60f));
            ForceActAndWait(targetService, 16);
        }
    }

    // Scadutree Avatar Phase 2
    public class ScadutreeAvatarPhase2 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 2";
        protected override uint Entity1Id => 2050480802; // Body
        protected override uint Entity2Id => 2050480812; // Health
        protected override bool FlatHp => true;
        protected override float Threshold => 1;
        protected override uint[] Phase2SpEffects => [];

        public ScadutreeAvatarPhase2(IChrInsService chrInsService) : base(chrInsService)
        {
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var scaduBodyChrIns = GetChrIns(Entity1Id);
            var scaduHealthChrIns = GetChrIns(Entity2Id);

            SetHp(scaduBodyChrIns, 1);
            SetHp(scaduHealthChrIns, 0);
        }
    }

    public class ScadutreeAvatarPhase1To3 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 3";
        protected override uint Entity1Id => 2050480802; // Body
        protected override uint Entity2Id => 2050480812; // Health
        protected override bool FlatHp => true;
        protected override float Threshold => 1;
        protected override uint[] Phase2SpEffects => [];

        public ScadutreeAvatarPhase1To3(IChrInsService chrInsService) : base(chrInsService)
        {
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var scaduBodyChrIns = GetChrIns(Entity1Id);
            var scaduHealthChrIns = GetChrIns(Entity2Id);

            // Keep at 1 hp to avoid overlapping phases by triggering the real events early
            // don't keep at max to avoid delay issues when setting health
            SetHp(scaduBodyChrIns, 1);
            SetHp(scaduHealthChrIns, 1);
            PlayAnimation(emevdService, Entity1Id,20004);

            Task.Run(async () =>
                {
                    var scaduBodyP2ChrIns = GetChrIns(2050480801);
                    var scaduHealthP2ChrIns = GetChrIns(2050480811);
                    var scaduBodyP3ChrIns = GetChrIns(2050480800);
                    var scaduHealthP3ChrIns = GetChrIns(2050480810);
                    var scaduBodyP3MaxHp = GetMaxHp(scaduBodyP3ChrIns);
                    var scaduHealthP3MaxHp = GetMaxHp(scaduHealthP3ChrIns);

                    bool isInRiposte = false;
                    int timeElapsed = 0;
                    while (timeElapsed < 5400)
                    {
                        await Task.Delay(10);
                        timeElapsed += 10;
                        if (GetCurrentAnimation(scaduBodyChrIns) == 11060)
                        {
                            isInRiposte = true;
                            break;
                        }
                    }

                    if (isInRiposte)
                    {
                        await Task.Delay(180);
                        SetHp(scaduBodyChrIns, 0);
                        SetHp(scaduHealthChrIns, 0);
                        await Task.Delay(160);
                        SetHp(scaduBodyP2ChrIns, 0);
                        SetHp(scaduHealthP2ChrIns, 0);
                        SetHp(scaduBodyP3ChrIns, (int)(scaduBodyP3MaxHp * 0.70f));
                        SetHp(scaduHealthP3ChrIns, (int)(scaduHealthP3MaxHp * 0.70f));
                    }
                    else
                    {
                        SetHp(scaduBodyP2ChrIns, 1);
                        SetHp(scaduHealthP2ChrIns, 1);
                        // forcing an animation with a death shorter than the default one lol 
                        PlayAnimation(emevdService, 2050480801, 20003);
                    }
                }
            );
        }
    }

    // Scadutree Avatar Phase 3
    public class ScadutreeAvatarPhase3 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 3";
        protected override uint Entity1Id => 2050480801; // Body
        protected override uint Entity2Id => 2050480811; // Health
        protected override bool FlatHp => true;
        protected override float Threshold => 1;
        protected override uint[] Phase2SpEffects => [];

        public ScadutreeAvatarPhase3(IChrInsService chrInsService) : base(chrInsService)
        {
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            var scaduBodyChrIns = GetChrIns(Entity1Id);
            var scaduHealthChrIns = GetChrIns(Entity2Id);

            SetHp(scaduBodyChrIns, 1);
            SetHp(scaduHealthChrIns, 0);
        }
    }

    public class ScadutreeAvatarPhase3Point5 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 3.5";
        protected override uint Entity1Id => 2050480800; // Body
        protected override uint Entity2Id => 2050480810; // Health
        protected override float Threshold => 0.4001f;
        protected override uint[] Phase2SpEffects => [];

        public ScadutreeAvatarPhase3Point5(IChrInsService chrInsService) : base(chrInsService)
        {
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            ForceActAndWait(targetService, 19);
        }
    }

    // Putrescent Knight
    public class PutresecentKnightPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.6501f;
        protected override uint Phase2SpEffect => 20010050;
        
        private readonly IChrInsService _chrInsService;
        private readonly IAiService _aiService;

        public PutresecentKnightPhase2(IChrInsService chrInsService, IAiService aiService)
        {
            _chrInsService = chrInsService;
            _aiService = aiService;
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            // imitating the AI call here
            var distanceFromPlayer = targetService.GetDist();
            if (distanceFromPlayer >= 10)
            {
                PlayAnimationAndSetCooldown(emevdService,_chrInsService, _aiService, 22000800, 3028);
            }
            else
            {
                PlayAnimationAndSetCooldown(emevdService,_chrInsService, _aiService, 22000800, 3012);
                Thread.Sleep(3666); // duration of the previous animation
                PlayAnimationAndSetCooldown(emevdService,_chrInsService, _aiService, 22000800, 3028);
            }
        }
    }

    // Midra, Lord of Frenzied Flame (Mini Midra)
    public class MidraPhase1 : SimplePhaseTransitionNoSpEffect
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0;
    }

    // Midra, Lord of Frenzied Flame
    public class MidraPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.7001f;
        protected override uint Phase2SpEffect => 20010262;

        private readonly IChrInsService _chrInsService;
        private readonly IAiService _aiService;

        public MidraPhase2(IChrInsService chrInsService, IAiService aiService)
        {
            _chrInsService = chrInsService;
            _aiService = aiService;
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimationAndSetCooldown(emevdService,_chrInsService, _aiService, 28000800, 3020);
        }
    }

    // Romina, Saint of the Bud
    public class RominaPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.6001f;
        protected override uint Phase2SpEffect => 10010050;

        private readonly IChrInsService _chrInsService;
        private readonly IAiService _aiService;

        public RominaPhase2(IChrInsService chrInsService, IAiService aiService)
        {
            _chrInsService = chrInsService;
            _aiService = aiService;
        }

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimationAndSetCooldown(emevdService,_chrInsService, _aiService, 2044450800, 3018);
        }
    }

    // Metyr, Mother of Fingers
    public class MetyrPhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.6001f;
        protected override uint Phase2SpEffect => 20010890;


        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            PlayAnimation(emevdService, 25000800, 3031);
        }
    }

    // Messmer the Impaler
    public class MessmerPhase2 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 2";
        protected override uint Entity1Id => 21010801; // p1
        protected override uint Entity2Id => 21010800; // p2
        protected override float Threshold => 0.50f;
        protected override uint[] Phase2SpEffects => [];

        public MessmerPhase2(IChrInsService chrInsService) : base(chrInsService)
        {
        }
    }

    // Promised Consort Radahn
    public class PcrPhase2 : TwoEntitiesPhaseTransition
    {
        public override string Label => "Phase 2";
        protected override uint Entity1Id => 20010801; // p1
        protected override uint Entity2Id => 20010800; // p2
        protected override float Threshold => 0.65f;
        protected override uint[] Phase2SpEffects => [];

        public PcrPhase2(IChrInsService chrInsService) : base(chrInsService)
        {
        }
    }

    // Bayle the Dread
    public class BaylePhase2 : SimplePhaseTransition
    {
        public override string Label => "Phase 2";
        protected override float Threshold => 0.65f;
        protected override uint Phase2SpEffect => 20010826;

        public override void Execute(ITargetService targetService, IEmevdService emevdService)
        {
            base.Execute(targetService, emevdService);
            ForceActAndWait(targetService, 22);
        }
    }

    #endregion

    #region optionals

    // put shit here later ok

    #endregion
}