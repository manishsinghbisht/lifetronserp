using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Ef6;

namespace Lifetrons.Erp.Data
{
    [Table("AspNetUser")]
    [MetadataType(typeof(AspNetUserMetadata))]
    public partial class AspNetUser:Entity
    {

        [DataType(DataType.Text)]
        public string Name
        {
            get { return FirstName + " " + LastName; }
        }

        internal sealed class AspNetUserMetadata
        {
            [DataType(DataType.Text)]
            [MaxLength(400)]
            public string UserName { get; set; }
        }
    }
}
