using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace LinkedIn_Integration.Entities
{
    public class Post
    {
        public string Author { get; set; }
        public string Commentary { get; set; }
        public string Visibility { get; set; }
        public Distribution Distribution { get; set; }
        public string LifeCycleState { get; set; }
        public bool IsReshareDisabledByAuthor { get; set; }

        public Media? Media { get; set; }
        public ReshareContext? ReshareContext { get; set; }

        public Content? Content { get; set; }

    }

    public class Distribution
    {
        public string FeedDistribution { get; set; }
        //public IList TargetEntities { get; set; }
        //public IList ThirdPartyDistributionChannels { get; set; }
    }

    public class Media
    {
        public string Title { get; set; }
        public string Id { get; set; }
    }

    public class ReshareContext
    {
        public string Parent { get; set; }
    }


    public class Content
    {
        public MultiImage MultiImage { get; set; }
    }
    public class MultiImage
    {
        public IList<Image> Images { get; set; }
    }

    public class Image
    {
        public string Id { get; set; }
        public string AltText { get; set; }
    }
    public class Comment
    {

    }
}
