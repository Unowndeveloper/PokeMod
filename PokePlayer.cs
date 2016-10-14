using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using PokeModBlue.Items.Weapons;


namespace PokeModBlue
{
    public class PokePlayer : ModPlayer
    {
        public override void PreUpdate()
        {
            // closes the conversation window if trying to select a pokemon and talking to the goblin to prevent reforging pokemon
            if (player.talkNPC > -1)
            {
                if (Main.npc[player.talkNPC].type == 107 && player.selectedItem == 58 && player.inventory[player.selectedItem].modItem != null)
                {
                    PokemonWeapon pokeWeapon;
                    pokeWeapon = player.inventory[player.selectedItem].modItem as PokemonWeapon;
                    if (pokeWeapon != null)
                    {
                        player.talkNPC = -1;
                    }
                }
            }
        }

        public override void SetupStartInventory(IList<Item> items)
        {
            Item item = new Item();
            item.SetDefaults(mod.ItemType("Pokecase"));
            item.stack = 1;
            items.Add(item);

            item = new Item();
            item.SetDefaults(mod.ItemType("PokeBall"));
            item.stack = 5;
            items.Add(item);
        }

        // 0: v0.2 -- v, numExtra, readInventory
        public static int SaveVersion = 0;
        public static int MaxExtraAccessories = 6;
        public Item[] ExtraAccessories = new Item[MaxExtraAccessories];
        public int numberExtraAccessoriesEnabled = 0;

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            for (int i = 0; i < numberExtraAccessoriesEnabled; i++)
            {
                player.VanillaUpdateEquip(ExtraAccessories[i]);
            }
            for (int i = 0; i < numberExtraAccessoriesEnabled; i++)
            {
                player.VanillaUpdateAccessory(ExtraAccessories[i], false, ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
            }
        }

        public override void Initialize()
        {
            ExtraAccessories = new Item[MaxExtraAccessories];
            for (int i = 0; i < MaxExtraAccessories; i++)
            {
                ExtraAccessories[i] = new Item();
                ExtraAccessories[i].SetDefaults();
            }
        }

        public override void LoadCustomData(BinaryReader reader)
        {
            for (int i = 0; i < MaxExtraAccessories; i++)
            {
                ExtraAccessories[i] = new Item();
                ExtraAccessories[i].SetDefaults();
            }

            int loadVersion = reader.ReadInt32();

            if (loadVersion == 0)
            {
                numberExtraAccessoriesEnabled = reader.ReadInt32();
                ReadInventory(ExtraAccessories, reader, false, false);
            }
        }

        public override void SaveCustomData(BinaryWriter writer)
        {
            writer.Write(SaveVersion);
            writer.Write(numberExtraAccessoriesEnabled);

            WriteInventory(ExtraAccessories, writer, false, false);
        }

        internal static bool WriteInventory(Item[] inv, BinaryWriter writer, bool writeStack = false, bool writeFavorite = false)
        {
            ushort count = 0;
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter invWriter = new BinaryWriter(stream))
                {
                    for (int k = 0; k < inv.Length; k++)
                    {
                        //    if (IsModItem(inv[k]))
                        {
                            invWriter.Write((ushort)k);
                            Terraria.ModLoader.IO.ItemIO.WriteItem(inv[k], invWriter, writeStack, writeFavorite);
                            count++;
                        }
                    }
                }

                data = stream.ToArray();
            }
            if (count > 0)
            {
                writer.Write(count);
                writer.Write(data);


                return true;
            }
            return false;
        }

        internal static void ReadInventory(Item[] inv, BinaryReader reader, bool readStack = false, bool readFavorite = false)
        {
            int count = reader.ReadUInt16();
            for (int k = 0; k < count; k++)
            {
                ushort index = reader.ReadUInt16();
                Terraria.ModLoader.IO.ItemIO.ReadItem(inv[index], reader, readStack, readFavorite);
            }

        }

        internal static bool IsModItem(Item item)
        {
            return item.type >= ItemID.Count;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, string deathText) {
            for (int i = 0; i < player.inventory.Length; i++) {
                PokemonWeapon pokemonWeapon = player.inventory[i].modItem as PokemonWeapon;
                if (pokemonWeapon != null) {
                    pokemonWeapon.currentHP = pokemonWeapon.maxHP;
                    pokemonWeapon.SetToolTip();
                    if (pokemonWeapon.npc != null) {
                        pokemonWeapon.npc.life = pokemonWeapon.maxHP;
                        pokemonWeapon.npc.HealEffect(pokemonWeapon.maxHP);
                    }
                }
            }
        }
    }
}
