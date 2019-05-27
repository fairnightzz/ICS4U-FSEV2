﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public static readonly Dictionary<string, double> staminaCosts = new Dictionary<string, double> { { "Idle", 0 }, { "Walking", 1 }, { "Sprinting", 3 } };

    // Identifiers < Store on login
    public readonly string Username;
    private readonly string Token;

    // Player's inventory < Save and Load. Clothing, Weapon, and Item all inherit from Equipment.
    private string[] equipped = new string[6]; // 0 = Helmet, 1 = Body Armour, 2 = Boots, 3 = Pendulum, 4 = Weapon 1, 5 = Weapon 2 
    private Dictionary<string, Clothing> clothing = new Dictionary<string, Clothing> { };
    private Dictionary<string, Weapon> weapons = new Dictionary<string, Weapon> { };
    private Dictionary<string, Item> items = new Dictionary<string, Item> { };

    // Game stats < Save and Load
    private int gold = 0;
    private List<string> titles = new List<string> { };
    private string chosenTitle = "Player";
    private int score = 0; // for world leaderboard

    // Quest stats < Save and Load
    private int mainProgress = 0; // the progress in the main storyline
    private Dictionary<string, int> optionalQuests = new Dictionary<string, int> { }; // the quests they have taken and their progress on them. -1 for completed

    // Special Skills < Save and Load. Magic, Attack and Ability all inherit from Skill
    private Dictionary<string, Magic> magicSpells = new Dictionary<string, Magic> { };
    private Dictionary<string, Attack> attackSkills = new Dictionary<string, Attack> { };
    private Dictionary<string, Ability> playerAbilities = new Dictionary<string, Ability> { };

    // Combat stats < Recalculate when needed
    private int[] hp = new int[] { 100, 100 }; // {max, current}
    private int[] mana = new int[] { 100, 100 }; // {max, current}
    private int[] stamina = new int[] { 100, 100 }; // {max, current}

    private float resist = 0;
    private float[] attackPower = new float[] { 0, 1 }; // {bonus, multiplier}
    private float[] magicPower = new float[] { 0, 1 }; // {bonus, multiplier}

    private int mobility = 20;

    // Game variables for playing
    private string status = "idle";
    private List<Condition> currentConditions = new List<Condition> { }; // poison, heal, slow, boost, etc. 
    private Vector3 position = new Vector3();
    private int guildID = -1;
    private float charge = -1; // the time they started charging. When the charge is used, it is reset to -1

    // public getters
    public int getGuild() { return guildID; }
    public float getCharge() { return Time.time - charge; }
    public Vector3 getPos(){ return position; }
    public int[] getStats() { return new int[] { hp[1], mana[1], stamina[1] }; }

    // functions
    public Player(string playerInfo) // gets the html from server, sets up player data 
    {

    }

    public void Charge(int status)
    {
        charge = (status == 1) ? Time.time : -1;
    }

    // Deals damage to the players, returns true if killed -- this is public because its the server and all code is reliable. 
    public bool TakeDamage(int damage) 
    {
        return false;
    }

    public void UpdateOne() // deals with health regen, poison effects, whatever
    {
        hp[1] = Mathf.Min(hp[0], hp[1] + hp[0] / 100);
        mana[1] = Mathf.Min(mana[0], mana[1] + mana[0] / 100);
        stamina[1] = Mathf.Min(stamina[0], stamina[1] + stamina[0] / 100);

        foreach (Condition effect in currentConditions)
        {
            effect.UpdateOne();
        }
    }

    public Weapon CheckWeapon(string name) // returns the weapon in question for when a player gets hit
    {
        if (equipped[4] == name || equipped[5] == name) // if it is equipped
        {
            return weapons[name];
        }
        return null;
    }

    public bool Cost(int stamina=0, int mana=0, int gold=0)
    {
        if (this.mana[1] >= mana && this.stamina[1] >= stamina && this.gold >= gold)
        {
            this.mana[1] -= mana;
            this.stamina[1] -= stamina;
            this.gold -= gold;
            return true;
        }
        return false;
    }
}