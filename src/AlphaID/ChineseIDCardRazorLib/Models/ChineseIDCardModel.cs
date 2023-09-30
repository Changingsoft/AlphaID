using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseIDCardRazorLib.Models;
internal class ChineseIDCardModel
{
    public string Name { get; set; }

    public string Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Ethentity { get; set; }

    public string Address { get; set; }

    public string CardNumber { get; set; }

    public string Issuer { get; set; }

    public DateTime IssueDate { get; set; }

    public DateTime? Expires { get; set; }

    public string HeadImage { get; set; }
}
