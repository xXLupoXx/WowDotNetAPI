﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;
using WowDotNetAPI.Models;
using WowDotNetAPI.Utilities;
using System.Runtime.Serialization.Json;
using System.IO;
using WowDotNetAPI.Exceptions;

namespace WowDotNetAPI
{
    public enum Region
    {
        US,
        EU,
        KR,
        TW,
        CN
    }

    public enum Locale
    {
        None,
        en_US,
        es_MX,
        en_GB,
        es_ES,
        fr_FR,
        ru_RU,
        de_DE,
        ko_KR,
        zh_TW,
        zh_CN
    }

    [Flags]
    public enum CharacterOptions
    {
        None = 0,
        GetGuild = 1,
        GetStats = 2,
        GetTalents = 4,
        GetItems = 8,
        GetReputation = 16,
        GetTitles = 32,
        GetProfessions = 64,
        GetAppearance = 128,
        GetCompanions = 256,
        GetMounts = 512,
        GetPets = 1024,
        GetAchievements = 2048,
        GetProgression = 4096,
        GetEverything = GetGuild | GetStats | GetTalents | GetItems | GetReputation | GetTitles
        | GetProfessions | GetAppearance | GetCompanions | GetMounts | GetPets
        | GetAchievements | GetProgression
    }

    [Flags]
    public enum GuildOptions
    {
        None = 0,
        GetMembers = 1,
        GetAchievements = 2,
        GetEverything = GetMembers | GetAchievements
    }

    public class WowExplorer : IExplorer
    {
        public Region Region { get; set; }
        public Locale Locale { get; set; }

        private string publicAuthKey { get; set; }
        private string privateAuthKey { get; set; }

        private string BaseAPIurl { get; set; }

        public WowExplorer() : this(Region.US) { }

        public WowExplorer(Region region)
            : this(region, Locale.en_US)
        {
            Region = region;
            SetDefaultLocale();
        }

        public WowExplorer(Region region, Locale locale)
        {
            Region = region;
            BaseAPIurl = ExplorerUtility.GetBaseURL(Region);
            Locale = locale;
        }

        public WowExplorer(Region region, Locale locale, string publicKey, string privateKey)
            : this(region, locale)
        {
            publicAuthKey = publicKey;
            privateAuthKey = privateKey;
        }

        #region Locale
        public void SetDefaultLocale()
        {
            switch (Region)
            {
                case Region.US:
                    Locale = Locale.en_US;
                    break;
                case Region.EU:
                    Locale = Locale.en_GB;
                    break;
                case Region.KR:
                    Locale = Locale.ko_KR;
                    break;
                case Region.TW:
                    Locale = Locale.zh_TW;
                    break;
                case Region.CN:
                    Locale = Locale.zh_CN;
                    break;
                default:
                    break;
            }
        }

        public void SetLocale(Locale locale)
        {
            switch (locale)
            {
                case Locale.en_US:
                case Locale.es_MX:
                    if (Region == Region.US) { Locale = locale; }
                    else { throw new InvalidLocaleException(string.Format("The {0} locale is not associated with the {1} region", locale, Region), Region, Locale); }
                    break;
                case Locale.en_GB:
                case Locale.es_ES:
                case Locale.fr_FR:
                case Locale.ru_RU:
                case Locale.de_DE:
                    if (Region == Region.EU) { Locale = locale; }
                    else { throw new InvalidLocaleException(string.Format("The {0} locale is not associated with the {1} region", locale, Region), Region, Locale); }
                    break;
                case Locale.ko_KR:
                    if (Region == Region.KR) { Locale = locale; }
                    else { throw new InvalidLocaleException(string.Format("The {0} locale is not associated with the {1} region", locale, Region), Region, Locale); }
                    break;
                case Locale.zh_TW:
                    if (Region == Region.TW) { Locale = locale; }
                    else { throw new InvalidLocaleException(string.Format("The {0} locale is not associated with the {1} region", locale, Region), Region, Locale); }
                    break;
                case Locale.zh_CN:
                    if (Region == Region.CN) { Locale = locale; }
                    else { throw new InvalidLocaleException(string.Format("The {0} locale is not associated with the {1} region", locale, Region), Region, Locale); }
                    break;
                default:
                    Locale = Locale.None;
                    break;
            }
        }

        #endregion

        #region Character

        public Character GetCharacter(string realm, string name)
        {
            return GetCharacter(Region, realm, name, CharacterOptions.None);
        }

        public Character GetCharacter(Region region, string realm, string name)
        {
            return GetCharacter(region, realm, name, CharacterOptions.None);
        }

        public Character GetCharacter(string realm, string name, CharacterOptions characterOptions)
        {
            return GetCharacter(Region, realm, name, characterOptions);
        }

        public Character GetCharacter(Region region, string realm, string name, CharacterOptions characterOptions)
        {

            return GetData<Character>(BaseAPIurl
                + string.Format(CharacterUtility.basePath + "{0}/{1}", realm, name)
                + GetLocaleQuery()
                + CharacterUtility.buildOptionalQuery(characterOptions));
        }

        private string GetLocaleQuery()
        {
            return "?locale=" + Locale;
        }

        #endregion

        #region Guild

        public Guild GetGuild(string realm, string name)
        {
            return GetGuild(Region, realm, name, GuildOptions.None);
        }

        public Guild GetGuild(Region region, string realm, string name)
        {
            return GetGuild(region, realm, name, GuildOptions.None);
        }

        public Guild GetGuild(string realm, string name, GuildOptions realmOptions)
        {
            return GetGuild(Region, realm, name, realmOptions);
        }

        public Guild GetGuild(Region region, string realm, string name, GuildOptions realmOptions)
        {
            return GetData<Guild>(BaseAPIurl
                + string.Format(GuildUtility.basePath + "{0}/{1}", realm, name)
                + GetLocaleQuery()
                + GuildUtility.buildOptionalQuery(realmOptions));
        }

        #endregion

        #region Realms
        public IEnumerable<Realm> GetRealms()
        {
            return GetRealms(Region);
        }

        public IEnumerable<Realm> GetRealms(Region region)
        {
            return GetData<RealmsData>(BaseAPIurl
                + RealmUtility.basePath
                + GetLocaleQuery()).Realms;
        }

        #endregion

        #region Auctions

        public Auctions GetAuctions(string realm)
        {
            return GetData<Auctions>(string.Format(BaseAPIurl
                + string.Format(AuctionUtility.basePath, realm.ToLower().Replace(' ', '-'))
                + GetLocaleQuery()));
        }

        #endregion

        #region Items

        public Item GetItem(string id)
        {
            return GetData<Item>(BaseAPIurl
                + string.Format(ItemUtility.basePath, id) + GetLocaleQuery());
        }

        #endregion

        #region Data
        public IEnumerable<CharacterRaceInfo> GetCharacterRaces()
        {
            return GetCharacterRaces(Region);
        }

        public IEnumerable<CharacterRaceInfo> GetCharacterRaces(Region region)
        {
            return GetData<CharacterRacesData>(BaseAPIurl
                + DataUtility.characterRacesPath + GetLocaleQuery()).Races;
        }

        public IEnumerable<CharacterClassInfo> GetCharacterClasses()
        {
            return GetCharacterClasses(Region);
        }

        public IEnumerable<CharacterClassInfo> GetCharacterClasses(Region region)
        {
            return GetData<CharacterClassesData>(BaseAPIurl
                + DataUtility.characterClassesPath + GetLocaleQuery()).Classes;
        }

        public IEnumerable<GuildRewardInfo> GetGuildRewards()
        {
            return GetGuildRewards(Region);
        }

        public IEnumerable<GuildRewardInfo> GetGuildRewards(Region region)
        {
            return GetData<GuildRewardsData>(BaseAPIurl
                + DataUtility.guildRewardsPath + GetLocaleQuery()).Rewards;
        }

        public IEnumerable<GuildPerkInfo> GetGuildPerks()
        {
            return GetGuildPerks(Region);
        }

        public IEnumerable<GuildPerkInfo> GetGuildPerks(Region region)
        {
            return GetData<GuildPerksData>(BaseAPIurl
                + DataUtility.guildPerksPath + GetLocaleQuery()).Perks;
        }

        #endregion

        private T GetData<T>(string url) where T : class
        {
            if (!string.IsNullOrEmpty(privateAuthKey) && !string.IsNullOrEmpty(publicAuthKey))
            {
                return JsonUtility.FromJSON<T>(url, publicAuthKey, privateAuthKey);
            }

            return JsonUtility.FromJSON<T>(url);
        }
    }
}
