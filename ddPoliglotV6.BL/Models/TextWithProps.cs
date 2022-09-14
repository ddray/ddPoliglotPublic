using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ddPoliglotV6.BL.Models
{
    public class TextWithProps
    {

        public string Text { get; set; }
        public TextProps TextProps { get; set; }

        public TextWithProps(string text) 
        {
            var start = text.IndexOf("{");
            var end = text.IndexOf("}");

            if (start < 0 || end < 0)
            {
                this.Text = text.Trim();
                this.TextProps = new TextProps();
            }
            else
            {
                this.Text = text.Substring(0, start).Trim();
                var json = text.Substring(start, (end - start) + 1);
                this.TextProps = JsonConvert.DeserializeObject<TextProps>(json);
            }
        }
    }

    public class TextProps
    {
        public string Pron { get; set; }
    }
}
