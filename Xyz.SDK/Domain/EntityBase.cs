using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Xyz.SDK.Domain
{
    [Serializable]
    public abstract class EntityBase<TPk>
    {
        
        [Key]
        public TPk Id { get; set; }

        [DefaultValue(false)]
        public bool Deleted { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
