using Terraria;
using Terraria.ModLoader;

namespace PokeModBlue.NPCs.Pokemon {

    public class Wigglytuff : PokemonNPC {
        public override float id { get { return 40.0f; } }

        public override void SetDefaults() {
            base.SetDefaults();
            npc.width = 34;
            npc.height = 52;
            Main.npcFrameCount[npc.type] = 3;
        }

        public override float CanSpawn(NPCSpawnInfo spawnInfo) {
            return spawnInfo.spawnTileY < Main.rockLayer && Main.dayTime ? 1f * base.CanSpawn(spawnInfo) : 0f;
        }
    }
}