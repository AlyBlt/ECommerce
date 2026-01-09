using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ECommerce.Web.Mvc.Helpers
{
    // Bu taghelper <my-img /> etiketiyle çalışacak
    [HtmlTargetElement("my-img", Attributes = "file-name")]
    public class MyImageTagHelper : TagHelper
    {
        public string? FileName { get; set; }
        public string? Class { get; set; }
        public string? Style { get; set; }
        public string? Alt { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Etiketi <img> olarak değiştir
            output.TagName = "img";

            // ImageHelper'ı kullanarak URL'i oluştur
            string src = ImageHelper.GetUrl(FileName);

            // HTML özniteliklerini ekle
            output.Attributes.SetAttribute("src", src);

            if (!string.IsNullOrEmpty(Class))
                output.Attributes.SetAttribute("class", Class);

            if (!string.IsNullOrEmpty(Style))
                output.Attributes.SetAttribute("style", Style);

            if (!string.IsNullOrEmpty(Alt))
                output.Attributes.SetAttribute("alt", Alt);

            // Kendi kendine kapanan etiket yap ( <img /> )
            output.TagMode = TagMode.SelfClosing;
        }
    }
}
