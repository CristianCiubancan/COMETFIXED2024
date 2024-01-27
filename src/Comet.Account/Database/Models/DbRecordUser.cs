using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Comet.Account.Database.Models
{
    [Table("records_user")]
    public class DbRecordUser
    {
        [Key] public uint Id { get; set; }
        public uint ServerIdentity { get; set; }
        public uint UserIdentity { get; set; }
        public uint AccountIdentity { get; set; }
        public string Name { get; set; }
        public uint MateId { get; set; }
        public byte Level { get; set; }
        public ulong Experience { get; set; }
        public byte Profession { get; set; }
        public byte OldProfession { get; set; }
        public byte NewProfession { get; set; }
        public byte Metempsychosis { get; set; }
        public ushort Strength { get; set; }
        public ushort Agility { get; set; }
        public ushort Vitality { get; set; }
        public ushort Spirit { get; set; }
        public ushort AdditionalPoints { get; set; }
        public uint SyndicateIdentity { get; set; }
        public ushort SyndicatePosition { get; set; }
        public ulong NobilityDonation { get; set; }
        public byte NobilityRank { get; set; }
        public uint SupermanCount { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ulong Money { get; set; }
        public uint WarehouseMoney { get; set; }
        public uint ConquerPoints { get; set; }
        public uint FamilyIdentity { get; set; }
        public ushort FamilyRank { get; set; }

        public static async Task<DbRecordUser> GetAsync(uint idSyn, uint idServer)
        {
            await using ServerDbContext ctx = new ();
            return await ctx.RecordUsers.FirstOrDefaultAsync(x =>
                x.UserIdentity == idSyn && x.ServerIdentity == idServer);
        }

        public static async Task<List<DbRecordUser>> GetAsync(uint idServer, int limit, int from = 0)
        {
            await using ServerDbContext ctx = new ();
            if (idServer == 0)
                return await ctx.RecordUsers.Where(x => x.DeletedAt == null).Skip(from).Take(limit).ToListAsync();
            return await ctx.RecordUsers.Where(x => x.ServerIdentity == idServer && x.DeletedAt == null).Skip(from)
                .Take(limit).ToListAsync();
        }

        public static async Task<DbRecordUser> GetByIdAsync(uint idUser, uint idServer)
        {
            await using ServerDbContext ctx = new ();
            return await ctx.RecordUsers
                .FirstOrDefaultAsync(x => x.UserIdentity == idUser && x.ServerIdentity == idServer);
        }
    }
}
