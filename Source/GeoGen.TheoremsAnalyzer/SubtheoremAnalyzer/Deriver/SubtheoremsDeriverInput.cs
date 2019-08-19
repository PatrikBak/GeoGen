using GeoGen.Constructor;
using GeoGen.Core;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents an input of the <see cref="ISubtheoremsDeriver"/>.
    /// </summary>
    public class SubtheoremsDeriverInput
    {
        /// <summary>
        /// Gets or sets the picture where we are trying to derive new theorems.
        /// </summary>
        public ContextualPicture Picture { get; set; }

        /// <summary>
        /// Gets or sets the list of theorems that are true in the picture.
        /// </summary>
        public TheoremsMap PictureTheorems { get; set; }

        /// <summary>
        /// Gets or sets the configuration where our template theorems hold.
        /// </summary>
        public Configuration TemplateConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the list of template theorems that are used to derive theorems in our picture.
        /// </summary>
        public TheoremsMap TemplateTheorems { get; set; }
    }
}
