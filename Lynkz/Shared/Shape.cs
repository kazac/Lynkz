using System.ComponentModel.DataAnnotations;

namespace Lynkz.Shared
{

    public class Shape
    {
        [Required]
        [StringLength(80, ErrorMessage = "Request is too long.")]
        public string? Request { get; set; }
   //     public string? Response { get; set; }
        public string? Msg { get; set; }
        public string? Type { get; set; }
        public string[]? Dims { get; set; }
        public int[]? Nums { get; set; }
    }


    public enum TokenType
    {
        NONE = 0,
        UNKNOWN = 1,
        FILLER = 2,
        SHAPE = 3,
        DIMENSION = 4,
        NUMBER = 5
    }

}