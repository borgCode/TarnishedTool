using System.Collections.Generic;
using TarnishedTool.Interfaces;

namespace TarnishedTool.Models;

public static class PhaseTransitionRegistry
{
    private static Dictionary<uint, List<PhaseTransition>>? _transitions;

    public static void Initialize(IChrInsService chrInsService, IEventService eventService, ISpEffectService spEffectService, IAiService aiService)
    {
        _transitions = new Dictionary<uint, List<PhaseTransition>>
        {

#region Example
            /*
            // Boss Name
            {
                // Entity Id
                NpcId, new List<PhaseTransition>
                {
                    new PhaseTransition.PhaseTransitionName1(),
                    new PhaseTransition.PhaseTransitionName2(),
                    new PhaseTransition.PhaseTransitionName3(), 
                    // However many I need
                }
            },     
            */
#endregion       

#region Base Game Main
            // Margit, the Fell Omen
            {
                21300014, new List<PhaseTransition>
                {
                    new PhaseTransition.MargitPhase2()
                }
            },

            // Godrick the Grafted
            {
                47500014, new List<PhaseTransition>
                {
                    new PhaseTransition.GodrickPhase2()
                }
            },
            // Starscourge Radahn
            {
                47300040, new List<PhaseTransition>
                {
                    new PhaseTransition.StarscourgeRadahnPhase1Point5(),
                    new PhaseTransition.StarscourgeRadahnPhase2(chrInsService, aiService)
                }
            },
            
            // Godskin Noble (Manor)
            {
                35700038, new List<PhaseTransition>
                {
                    new PhaseTransition.NoblePhase2() 
                    
                }
            },
            
            // Rykard (God-Devouring Serpent)
            {
                47100038, new List<PhaseTransition>
                {
                    new PhaseTransition.RykardPhase2()
                }
            },
            
            // Rykard
            {
                47101038, new List<PhaseTransition>
                {
                    new PhaseTransition.RykardPhase2Point5()
                }
            },

            // Rennala Phase 2
            {
                20300024, new List<PhaseTransition>
                {
                    new PhaseTransition.RennalaPhase2()
                }
            },
            // Rennala Phase 2.5
            {
                20310024, new List<PhaseTransition>
                {
                    new PhaseTransition.RennalaPhase2Point5()
                }
            },
            // Draconic Tree Sentinel
            {
                32500033, new List<PhaseTransition>
                {
                    new PhaseTransition.DraconicTreeSentinelPhase2() 
                    
                }
            },
            // Morgott, the Omen King
            {
                21300534, new List<PhaseTransition>
                {
                    new PhaseTransition.MorgottPhase2()
                }
            },

            // Fire Giant
            {
                47600050, new List<PhaseTransition>
                {
                    new PhaseTransition.FireGiantPhase1Point5(chrInsService),
                    new PhaseTransition.FireGiantPhase2(chrInsService)
                }
            },
            // Godskin Duo (Noble)
            {
                35700172, new List<PhaseTransition>
                {
                    new PhaseTransition.NobleGduoPhase2(chrInsService)  
                    
                }
            },
            // Godskin Duo (Apostle)
            {
                35600172, new List<PhaseTransition>
                {
                    new PhaseTransition.ApostleGduoPhase2(chrInsService) 
                    
                }
            },

            // Clergyman -> Maliketh
            {
                21100072, new List<PhaseTransition>
                {
                    new PhaseTransition.MalikethPhase2(chrInsService)
                }
            },

            // Godfrey → Hoarah Loux
            {
                47200070, new List<PhaseTransition>
                {
                    new PhaseTransition.GodfreyPhase1Point5(chrInsService),
                    new PhaseTransition.GodfreyPhase2(chrInsService),
                }
            },

            // Hoarah Loux
            {
                47210070, new List<PhaseTransition>
                {
                    new PhaseTransition.HoarahLouxPhase2Point5(),
                }
            },
            
            // Radagon of the Golden Order
            {
                21900078, new List<PhaseTransition>
                {
                    new PhaseTransition.RadagonPhase2(),
                    new PhaseTransition.RadagonPhase3()
                    
                    
                }
            },
            
            // Elden Beast
            {
                22000078, new List<PhaseTransition>
                {
                    new PhaseTransition.EldenBeastSingleRing(),
                    new PhaseTransition.EldenBeastEldenStars(),
                    new PhaseTransition.EldenBeastTripleRings(),
                    
                    
                }
            },
            
            // Commander Niall
            {
                30500051, new List<PhaseTransition>
                {
                    new PhaseTransition.NiallPhase2() 
                    
                }
            },
            
            // Loretta, Knight of the Haligtree
            {
                32520054, new List<PhaseTransition>
                {
                    new PhaseTransition.HaligtreeLorettaPhase2() 
                    
                }
            },

            // Malenia, Blade of Miquella
            {
                21200056, new List<PhaseTransition>
                {
                    new PhaseTransition.MaleniaPhase2()
                }
            },
            
            // Mohg, Lord of Blood
            {
                48000068, new List<PhaseTransition>
                {
                    new PhaseTransition.MohgPhase2() 
                    
                }
            },
            
            // Royal Kight Loretta
            {
                32520921, new List<PhaseTransition>
                {
                    new PhaseTransition.RoyalLorettaPhase2() 
                    
                }
            },
            
            // Astel, Naturalborn of the Void
            {
                46200062, new List<PhaseTransition>
                {
                    new PhaseTransition.AstelNaturalbornPhase2() 
                    
                }
            },
            
            // Dragonlord Placidusax
            {
                45200072, new List<PhaseTransition>
                {
                    new PhaseTransition.PlacidusaxPhase2(),
                    new PhaseTransition.PlacidusaxPhase3(chrInsService, aiService),
                    new PhaseTransition.PlacidusaxPhase4(chrInsService, aiService) 
                    
                }
            },
            
            // Lichdragon Fortissax
            {
                45110066, new List<PhaseTransition>
                {
                    new PhaseTransition.FortissaxPhase2() 
                    
                }
            },
            
#endregion

#region DLC Main

            // Belurat Divine Beast Dancing Lion
            {
                // 20000800
                52100088, new List<PhaseTransition>
                {
                    new PhaseTransition.BeluratLionPhase2() 
                    
                }
            },
            
            // Rellana, Twin Moon Knight
            {
                // 2048440800
                53000082, new List<PhaseTransition>
                {
                    new PhaseTransition.RellanaPhase1Point5(chrInsService, aiService),
                    new PhaseTransition.RellanaPhase2(chrInsService, aiService) 
                    
                }
            },
            
            // Commander Gaius
            {
                // 2049480800
                50000092, new List<PhaseTransition>
                {
                    new PhaseTransition.GaiusPhase2() 
                    
                }
            },
            
            // Scadutree Avatar Phase 1
            {
                // 2050480802 Phase 1 Body
                // 2050480812 Phase 1 HP
                // 2050480801 Phase 2 Body
                // 2050480811 Phase 2 HP
                // 2050480800 Phase 3 Body
                // 2050480810 Phase 3 HP
                52300096, new List<PhaseTransition>
                {
                    new PhaseTransition.ScadutreeAvatarPhase1Point5(chrInsService),
                    new PhaseTransition.ScadutreeAvatarPhase2(chrInsService),
                    new PhaseTransition.ScadutreeAvatarPhase1To3(chrInsService) 
                    
                }
            },
            // Scadutree Avatar Phase 2
            {
                // 2050480801 Phase 2 Body
                // 2050480811 Phase 2 HP
                52300296, new List<PhaseTransition>
                {
                    new PhaseTransition.ScadutreeAvatarPhase3(chrInsService) 
                    
                }
            },
            
            // Scadutree Avatar Phase 3
            {
                // 2050480800 Phase 3 Body
                // 2050480810 Phase 3 HP
                52300396, new List<PhaseTransition>
                {
                    new PhaseTransition.ScadutreeAvatarPhase3Point5(chrInsService) 
                    
                }
            },
            
            // Putrescent Knight
            {
                // 22000800
                50200087, new List<PhaseTransition>
                {
                    new PhaseTransition.PutresecentKnightPhase2(chrInsService, aiService) 
                    
                }
            },
            
            // Midra, Lord of Frenzied Flame (Mini Midra)
            {
                // 28000801
                50500086, new List<PhaseTransition>
                {
                    new PhaseTransition.MidraPhase1() 
                    
                }
            },
            
            // Midra, Lord of Frenzied Flame
            {
                // 28000800
                50510086, new List<PhaseTransition>
                {
                    new PhaseTransition.MidraPhase2(chrInsService, aiService) 
                    
                }
            },
            
            // Romina, Saint of the Bud
            {
              //  2044450800
                50300094, new List<PhaseTransition>
                {
                    new PhaseTransition.RominaPhase2(chrInsService, aiService) 
                    
                }
            },
            
            // Metyr, Mother of Fingers
            {
                // 25000800
                52000097, new List<PhaseTransition>
                {
                    new PhaseTransition.MetyrPhase2() 
                    
                }
            },
            
            // Messmer the Impaler
            {
                // 21010801
                51300099, new List<PhaseTransition>
                {
                    new PhaseTransition.MessmerPhase2(chrInsService) 
                    
                }
            },
            
            // Promised Consort Radahn
            {
                // 20010801
                52200089, new List<PhaseTransition>
                {
                    new PhaseTransition.PcrPhase2(chrInsService) 
                    
                }
            },
            
            
            // Bayle the Dread
            {
                // 2054390800
                51200085, new List<PhaseTransition>
                {
                    new PhaseTransition.BaylePhase2() 
                    
                }
            },
            
#endregion           
        };
    }

    public static List<PhaseTransition>? Get(uint npcParamId)
    {
        if (_transitions == null) return null;
        _transitions.TryGetValue(npcParamId, out var transitions);
        return transitions;
    }
}