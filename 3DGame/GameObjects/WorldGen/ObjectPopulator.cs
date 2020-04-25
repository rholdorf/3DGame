﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DGame.GameObjects.WorldGen
{
    public class ObjectPopulator
    {
        private System.Random RNG;

        public ObjectPopulator(System.Random RNG)
        {
            this.RNG = RNG;
        }

        public List<MapEntity> GenerateObjectsTest(int count)
        {
            List<MapEntity> results = new List<MapEntity>();
            MonsterGenerator mg = new MonsterGenerator(this.RNG);
            NPCGenerator ng = new NPCGenerator(this.RNG);
            results.AddRange(mg.GenerateTestMonsters((int)count / 3));
            results.AddRange(ng.GenerateTestNPCs((int)count));
            return results;
        }

    }
}