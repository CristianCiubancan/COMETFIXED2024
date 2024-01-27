using System.Collections.Generic;

namespace Comet.Web.App.Areas.Profile.Models
{
    public class ProfileModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Title { get; set; }

        public string BackgroundImage = "~/assets/images/parallax/trojan1_1920x1200.jpg";
        public string ProfileImage = "~/assets/images/avatar.png";

        public int CommentCount { get; set; }
        public int Reputation { get; set; }

        public string Name { get; set; }

        public Dictionary<PersonalInfoType, string> PersonalInfo { get; set; } = new Dictionary<PersonalInfoType, string>();
        public Dictionary<ContactInfoType, string> ContactInfo { get; set; } = new Dictionary<ContactInfoType, string>();

        public bool LoadedByOwner { get; set; }
        public bool IsFriend { get; set; }
    }

    public enum PersonalInfoType
    {
        SocialName,
        Hometown,
        CurrentCity,
        RelationshipStatus,
        StudiedAt,
        Language
    }

    public enum ContactInfoType
    {
        Phone,
        MobilePhone,
        Instagram,
        Twitter,
        Facebook,
        Email,
        Website
    }

    public static class ContactInfoHelper
    {
        public static string ToClaimString(this PersonalInfoType e)
        {
            return $"Profile.{e}";
        }

        public static string ToClaimString(this ContactInfoType e)
        {
            return $"Profile.{e}";
        }
    }
}
