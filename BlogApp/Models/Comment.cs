using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class Comment
    {
        
        public int Id { get; set; }

        [Required]
        [DisplayName("コメント")]
        public string Body { get; set; }

        [DisplayName("投稿日")]
        public DateTime Create { get; set; }

        // 1 対 1(記事のIDの保持をする)
        public virtual Article Article { get; set; }

        [NotMapped]
        public int ArticleId { get; set; }
    }
}