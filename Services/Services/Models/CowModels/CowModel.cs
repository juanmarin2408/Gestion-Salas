

using Services.Models.MilkModels;
using Services.Models.UserModels;

namespace Services.Models.CowModels
{
    public class CowModel
    {
        public Guid Id { get; set; }
        public string Race { get; set; }
        public MilkModel Milk { get; set; }
    }


    public class CowsModel
    {
        public IList<CowModel> Cows { get; set; } = new List<CowModel>();
    }
}
