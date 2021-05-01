using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_datagrid.Models
{

// 顧客。本当はもっとたくさんフィールドが必要。
public class Customer: RecordBase
{
    public enum MembershipGrade {
        Silver = 1,
        Gold = 2,
        Platinum = 3,
    };

    // Combobox に表示する辞書は, 静的でもよい。
    static public Dictionary<MembershipGrade, string> GradeStr =
        new Dictionary<MembershipGrade, string>() { 
                {MembershipGrade.Silver, "Silver"},
                {MembershipGrade.Gold, "Gold"},
                {MembershipGrade.Platinum, "Platinum"}
            };

    [Key, Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; protected set; }

    // 苗字がない地域もあることに注意
    public string Surname { get; set; }

    [Column("given_name"), Required]
    public string GivenName { get; set; }

    [Column("ship_to"), Required]
    public string ShipTo { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public MembershipGrade Grade { get; set; }
} // class Customer

}
