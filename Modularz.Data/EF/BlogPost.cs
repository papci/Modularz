using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using UnitSense.Extensions.Extensions;

namespace Modularz.Data.EF;

public class BlogPost
{
    [Key]
    public int Id { get; set; }

    public DateTime DateCreated { get; set; }
    public DateTime DatePublished { get; set; }
    public DateTime DateUpdated { get; set; }
    public string MetaDescription { get; set; }
    public string Title { get; set; }
    public string SeoUrl { get; set; }
    public string Chapo { get; set; }
    public string Text { get; set; }

    public BlogState State { get; set; }
    
    public BlogUser? Author { get; set; }
    public enum BlogState
    {
        Draft,
        Prepub,
        Published
    }
    
    /// <summary>
        /// Crée une URL Seo friendly pour l'actu
        /// </summary>
        /// <returns></returns>
        public string CreateSeoUrl()
        {
            //Trim Start and End Spaces. 
            var strTitle = this.Title;

            //Trim "-" Hyphen 
            strTitle = strTitle.Trim('-');


            /*Remove some useless words*/
            List<string> UselessWords = new List<string>()
                {" les ", " le ", " la ", " unes ", " une ", " un ", " des ", " de ", " du ", "l'"};
            foreach (string uselessWord in UselessWords)
            {
                var starter = uselessWord.TrimStart(' ');
                var ender = uselessWord.TrimEnd(' ');
                if (strTitle.StartsWith(starter))
                    strTitle = strTitle.ReplaceFirst(starter, string.Empty);

                if (strTitle.EndsWith(ender))
                    strTitle = strTitle.ReplaceLast(ender, string.Empty);

                strTitle = strTitle.Replace(uselessWord, " ");
            }


            strTitle = strTitle.RemoveDiacritics();

            char[] chars = @"$%#@!*?;:~`+=()[]{}|\'<>,/^&"".".ToCharArray();

            //Replace . with - hyphen 
            strTitle = strTitle.Replace(".", "-");
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            strTitle = rgx.Replace(strTitle, "");
            //Replace Special-Characters 
            for (int i = 0; i < chars.Length; i++)
            {
                string strChar = chars.GetValue(i).ToString();
                if (strTitle.Contains(strChar))
                {
                    strTitle = strTitle.Replace(strChar, string.Empty);
                }
            }

            strTitle = strTitle.ToUrlFormat();
            //Replace all spaces with one "-" hyphen 
            strTitle = strTitle.Replace(" ", "-");

            //Replace multiple "-" hyphen with single "-" hyphen. 
            strTitle = strTitle.Replace("--", "-");
            strTitle = strTitle.Replace("---", "-");
            strTitle = strTitle.Replace("----", "-");
            strTitle = strTitle.Replace("-----", "-");
            strTitle = strTitle.Replace("----", "-");
            strTitle = strTitle.Replace("---", "-");
            strTitle = strTitle.Replace("--", "-");

            //Run the code again... 
            //Trim Start and End Spaces. 
            strTitle = strTitle.Trim();

            //Trim "-" Hyphen 
            strTitle = strTitle.Trim('-');


            String normalizedString = strTitle.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                    System.Globalization.UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }
}