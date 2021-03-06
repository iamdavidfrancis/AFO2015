﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Orlandia2015.Models
{
    public class Player
    {
        [Key]
        public Guid uPlayerID { get; set; }

        public Guid uFactionID { get; set; }

        public string sName { get; set; }

        public Guid uRankID { get; set; }

        public int iPoints { get; set; }

        public int iMissionsCompleted { get; set; }

        public virtual ICollection<PlayerAchievements> Achievements { get; set; }
        public virtual ICollection<PlayerMission> Missions { get; set; }
        public virtual Faction Faction { get; set; }
        public virtual Rank Rank { get; set; }
    }

    public class Faction
    {
        [Key]
        public Guid uFactionID { get; set; }

        public string sName { get; set; }

        public int iPoints { get; set; }
    }

    public class Achievement
    {
        [Key]
        public Guid uAchievementID { get; set; }

        public string sName { get; set; }

        public int iSortOrder { get; set; }

        public bool bCanBeManuallySet { get; set; }
    }

    public class PlayerAchievements
    {
        [Key]
        public Guid uPlayerAchievementID { get; set; }

        [ForeignKey("Player")]
        public Guid uPlayerID { get; set; }

        [ForeignKey("Achievement")]
        public Guid uAchievementID { get; set; }

        public virtual Player Player { get; set; }
        public virtual Achievement Achievement { get; set; }

    }

    public class Rank
    {
        [Key]
        public Guid uRankID { get; set; }

        [ForeignKey("Faction")]
        public Guid uFactionID { get; set; }

        public byte iRankNumber { get; set; }

        public short iRankPoints { get; set; }

        public string sRankName { get; set; }

        public virtual Faction Faction { get; set; }

    }

    public class Mission
    {
        [Key]
        public Guid uMissionID { get; set; }

        public string sMissionName { get; set; }

        public int iMissionPoints { get; set; }

        public bool bIsMissionQuest { get; set; }

        public int iMissionLevel { get; set; }

    }

    public class PlayerMission
    {
        [ForeignKey("Player")]
        public Guid uPlayerID { get; set; }

        [ForeignKey("Mission")]
        public Guid uMissionID { get; set; }

        [Key]
        public Guid uPlayerMissionID { get; set; }

        public virtual Player Player { get; set; }
        public virtual Mission Mission { get; set; }
    }

    public class MissionAchievement
    {
        [Key]
        public Guid uMissionAchievementID { get; set; }

        [ForeignKey("Achievement")]
        public Guid uAchievementID { get; set; }

        public int iMissionCount { get; set; }

        public virtual Achievement Achievement { get; set; }

    }

    public class GameProperty
    {
        [Key]
        public int iData_id { get; set; }

        public string sName { get; set; }

        public string sValue { get; set; }
    }

    public class OrlandiaDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Faction> Factions { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<PlayerMission> PlayerMissions { get; set; }
        public DbSet<PlayerAchievements> PlayerAchievements { get; set; }
        public DbSet<MissionAchievement> MissionAchievements { get; set; }
        public DbSet<GameProperty> GameProperties { get; set; }
    }

}