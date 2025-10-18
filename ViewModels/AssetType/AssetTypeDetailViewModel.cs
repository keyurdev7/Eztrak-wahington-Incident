using Helpers.Color;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Shared;

namespace ViewModels
{
    public class AssetTypeDetailViewModel : BaseCrudViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public string DarkerColor
        {
            get
            {
                try
                {
                    return ColorHelper.DarkenHexColor(Color, 60);

                }
                catch
                {
                    return "#000000";
                }
            }
        }



        public DateTime CreatedOn { get; set; }


    }

}
