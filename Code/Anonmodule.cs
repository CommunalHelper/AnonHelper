﻿using Monocle;
using System;

namespace Celeste.Mod.Anonhelper {
    public class AnonModule : EverestModule {
        public static AnonModule Instance;
        public static SpriteBank spriteBank;

        public AnonModule() {
            Instance = this;
        }

        public static AnonhelperSession Session => (AnonhelperSession)Instance._Session;
        public override Type SessionType => typeof(AnonhelperSession);

        public override void LoadContent(bool firstLoad) {
            base.LoadContent(firstLoad);
            spriteBank = new SpriteBank(GFX.Game, "Graphics/carelessAnonymous/Sprites.xml");
            SuperDashRefill.LoadContent();
        }

        public override void Load() {
            CustomDashHooks.Load();
            InvisibleSeekerBarrier.Load();
            OneUseBooster.Load();
            SuperDashRefill.Load();
        }

        public override void Unload() {
            CustomDashHooks.Unload();
            InvisibleSeekerBarrier.Unload();
            OneUseBooster.Unload();
            SuperDashRefill.Unload();
        }
    }
}