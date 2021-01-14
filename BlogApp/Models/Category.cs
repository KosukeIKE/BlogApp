using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class Category
    {
        
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]//これを作るのは桁数を制限する必要がある
        [StringLength(255)]//IndexとStringLengthはセットで使用される
        [DisplayName("カテゴリー")]
        public string CategoryName { get; set; }

        [DisplayName("投稿数")]
        public int Count { get; set; }
        //1対Nの形にする
        public virtual ICollection<Article> Articles { get; set; }
    }
}