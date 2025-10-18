using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Services.ArcGis.Interface
{
    public interface IArcGisGeocodingService
    {
        Task<List<string>> GetSuggestionsAsync(string text);
       
        Task<(double lat, double lon, string address)?> GetCoordinatesAsync(string magicKey);
        Task<List<(string Text, double Lat, double Lng)>> GetSuggestionsAsynclat(string text);
        Task<List<(string Text, string MagicKey)>> GetSuggestionsAsyncWithMagicKey(string text);
    }
}
