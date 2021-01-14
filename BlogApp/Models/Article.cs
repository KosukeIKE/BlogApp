using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BlogApp.Models
{
    public class Article
    {
        //Property(POCOを用意)
        public int Id { get; set; }
        [Required]
        [DisplayName("タイトル")]
        public string Title { get; set; }
        [Required]
        [DisplayName("投稿文")]
        public string Body { get; set; }
        [DisplayName("投稿日")]
        public DateTime Created { get; set; }
        [DisplayName("投稿修正日")]
        public DateTime Modifyed { get; set; }

        public virtual Category Category { get; set; }

        //1 対 Nの実装
        public virtual ICollection<Comment> Comments { get; set; }

        [NotMapped]//データテーブルを作成しないアノテーション（属性）
        [DisplayName("カテゴリー")]
        //画面時の入力を保持する目的
        public string CategoryName { get; set; }
        
    }
}