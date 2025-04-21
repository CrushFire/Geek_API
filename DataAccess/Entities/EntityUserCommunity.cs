using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class EntityUserCommunity
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [ForeignKey(nameof(UserId))]
        public EntityUser User { get; set; }
        [Required]
        public int CommunityId { get; set; }
        [Required]
        [ForeignKey(nameof(CommunityId))]
        public EntityCommunity Community { get; set; }
    }
}
